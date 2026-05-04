using System;
using System.Drawing;
using VectorGraphicsEditor.Models;
using VectorGraphicsEditor.Rendering;
using VectorGraphicsEditor.State;

namespace RasterEditor.Rendering
{
    /// <summary>
    /// Core rendering engine. Implements exactly the algorithms from Lecture 5:
    ///
    ///   Line    → Symmetric Midpoint Line      (slide 12)
    ///   Thick   → Copying pixels in rows/cols  (slide 19)
    ///   Circle  → Alternative Midpoint Circle  (slide 17)  ← additions only
    ///   AA Line → Xiaolin Wu Line              (slide 30)
    ///   AA Circ → Xiaolin Wu Circle            (slide 32)
    /// </summary>
    public sealed class Rasterizer
    {
        private readonly Scene _scene;
        private DirectBitmap _db;

        public Color BackgroundColor { get; set; } = Color.White;

        public Rasterizer(Scene scene, int width, int height)
        {
            _scene = scene;
            _db = new DirectBitmap(width, height);
        }

        // ── Public API ───────────────────────────────────────────────────────

        public void Resize(int width, int height)
        {
            _db.Dispose();
            _db = new DirectBitmap(width, height);
        }

        public Bitmap Render()
        {
            _db.Clear(BackgroundColor);

            foreach (IShape shape in _scene.Shapes)
            {
                switch (shape)
                {
                    case LineShape line: DrawLine(line); break;
                    case CircleShape circle: DrawCircle(circle); break;
                    case PolygonShape polygon: DrawPolygon(polygon); break;
                }

                if (ReferenceEquals(shape, _scene.SelectedShape))
                    DrawSelectionIndicator(shape);
            }

            return _db.Bitmap;
        }

        public DirectBitmap DirectBitmap => _db;

        // ── Shape dispatchers ────────────────────────────────────────────────

        public void DrawLine(LineShape line)
        {
            int x1 = line.StartPoint.X, y1 = line.StartPoint.Y;
            int x2 = line.EndPoint.X, y2 = line.EndPoint.Y;

            if (_scene.AntiAliasingEnabled)
            {
                // AA ignores thickness per spec
                DrawXiaolinWuLine(x1, y1, x2, y2, line.BaseColor);
            }
            else if (line.Thickness > 1)
            {
                DrawThickLine(x1, y1, x2, y2, line.BaseColor, line.Thickness);
            }
            else
            {
                DrawSymmetricMidpointLine(x1, y1, x2, y2, line.BaseColor);
            }
        }

        public void DrawCircle(CircleShape circle)
        {
            if (_scene.AntiAliasingEnabled)
                DrawXiaolinWuCircle(circle.Radius,
                                    circle.Center.X, circle.Center.Y,
                                    circle.BaseColor);
            else
                DrawAlternativeMidpointCircle(circle.Radius,
                                              circle.Center.X, circle.Center.Y,
                                              circle.BaseColor);
        }

        public void DrawPolygon(PolygonShape polygon)
        {
            var verts = polygon.Vertices;
            if (verts.Count < 2) return;

            int limit = polygon.IsClosed ? verts.Count : verts.Count - 1;

            for (int i = 0; i < limit; i++)
            {
                Point a = verts[i];
                Point b = verts[(i + 1) % verts.Count];

                // Re-use the full line pipeline so AA / thick routing is inherited
                var edge = new LineShape
                {
                    StartPoint = a,
                    EndPoint = b,
                    BaseColor = polygon.BaseColor,
                    Thickness = polygon.Thickness
                };
                DrawLine(edge);
            }
        }

        // ── Core pixel writer ────────────────────────────────────────────────

        /// <summary>
        /// The ONLY method allowed to write an opaque pixel.
        /// Every algorithm below calls exclusively this or BlendPixel.
        /// </summary>
        public void PutPixel(int x, int y, Color color)
            => _db.SetPixel(x, y, color);

        // ════════════════════════════════════════════════════════════════════
        // 1. SYMMETRIC MIDPOINT LINE  (Lecture 5, slide 12)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Draws from BOTH endpoints toward the centre simultaneously,
        /// sharing a single decision variable d.
        ///
        /// Handles all octants by choosing the dominant axis and
        /// normalising direction before entering the loop.
        /// </summary>
        public void DrawSymmetricMidpointLine(int x1, int y1, int x2, int y2,
                                               Color color)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);

            // ── Shallow line: |slope| <= 1, step in X ────────────────────────
            if (dx >= dy)
            {
                // Normalise so that x1 < x2 (the algorithm steps left → right)
                if (x1 > x2)
                {
                    Swap(ref x1, ref x2);
                    Swap(ref y1, ref y2);
                }

                // y-direction sign after normalisation
                int sy = (y2 >= y1) ? 1 : -1;
                dy = Math.Abs(y2 - y1);   // recalculate after possible swap

                // Initial decision parameter  d = 2dy - dx  (slide 10)
                int d = 2 * dy - dx;
                int dE = 2 * dy;           // ΔdE  = 2dy
                int dNE = 2 * (dy - dx);    // ΔdNE = 2dy - 2dx

                // Two cursors starting at opposite ends
                int xf = x1, yf = y1;
                int xb = x2, yb = y2;

                PutPixel(xf, yf, color);
                PutPixel(xb, yb, color);

                while (xf < xb)
                {
                    // Advance both cursors by one step in x
                    ++xf;
                    --xb;

                    if (d < 0)          // move East
                    {
                        d += dE;
                        // y does NOT change for either cursor
                    }
                    else                // move North-East
                    {
                        d += dNE;
                        yf += sy;       // forward  cursor: y moves in +sy
                        yb -= sy;       // backward cursor: y moves in -sy
                    }

                    PutPixel(xf, yf, color);
                    PutPixel(xb, yb, color);
                }
            }
            // ── Steep line: |slope| > 1, step in Y ───────────────────────────
            else
            {
                // Normalise so that y1 < y2
                if (y1 > y2)
                {
                    Swap(ref x1, ref x2);
                    Swap(ref y1, ref y2);
                }

                int sx = (x2 >= x1) ? 1 : -1;
                dx = Math.Abs(x2 - x1);

                // Mirror of the shallow case: swap roles of dx ↔ dy
                int d = 2 * dx - dy;
                int dN = 2 * dx;           // ΔdN  (equivalent of dE)
                int dNE = 2 * (dx - dy);    // ΔdNE

                int xf = x1, yf = y1;
                int xb = x2, yb = y2;

                PutPixel(xf, yf, color);
                PutPixel(xb, yb, color);

                while (yf < yb)
                {
                    ++yf;
                    --yb;

                    if (d < 0)          // move North (no x change)
                    {
                        d += dN;
                    }
                    else                // move North-East
                    {
                        d += dNE;
                        xf += sx;
                        xb -= sx;
                    }

                    PutPixel(xf, yf, color);
                    PutPixel(xb, yb, color);
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // 2. THICK LINE — copying pixels  (Lecture 5, slide 19)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Achieves thickness by copying the spine line into parallel offsets.
        ///
        /// Shallow lines (|dx| >= |dy|): copies are stacked vertically
        ///   (i.e. the same x-column is replicated at y ± offset).
        /// Steep lines  (|dy|  > |dx|): copies are stacked horizontally.
        ///
        /// Only odd thickness values reach here (enforced by the model).
        /// half = thickness / 2  gives the symmetric spread either side.
        /// </summary>
        public void DrawThickLine(int x1, int y1, int x2, int y2,
                                   Color color, int thickness)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int half = thickness / 2;   // e.g. thickness=5 → half=2  → offsets -2,-1,0,1,2

            if (dx >= dy)
            {
                // Shallow: stack rows  (copy in the y direction)
                for (int offset = -half; offset <= half; offset++)
                    DrawSymmetricMidpointLine(x1, y1 + offset,
                                              x2, y2 + offset, color);
            }
            else
            {
                // Steep: stack columns  (copy in the x direction)
                for (int offset = -half; offset <= half; offset++)
                    DrawSymmetricMidpointLine(x1 + offset, y1,
                                              x2 + offset, y2, color);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // 3. ALTERNATIVE MIDPOINT CIRCLE  (Lecture 5, slides 16-17)
        //    — additions only inside the loop, no multiplications —
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Uses two incremental delta accumulators dE and dSE so that
        /// updating d never requires a multiplication:
        ///
        ///   Initial values:
        ///     d   = 1 - R
        ///     dE  = 3
        ///     dSE = 5 - 2R
        ///
        ///   Each iteration:
        ///     if d &lt; 0  → move E:  d += dE;  dE += 2; dSE += 2
        ///     else      → move SE: d += dSE; dE += 2; dSE += 4; --y
        ///     ++x
        ///
        /// Computes 1/8 of the circle and mirrors into all 8 octants.
        /// </summary>
        public void DrawAlternativeMidpointCircle(int r, int xc, int yc, Color color)
        {
            int d = 1 - r;        // initial decision parameter
            int dE = 3;            // ΔdE  initial value
            int dSE = 5 - 2 * r;    // ΔdSE initial value

            int x = 0;
            int y = r;

            PlotCircleOctants(xc, yc, x, y, color);

            while (y > x)
            {
                if (d < 0)          // midpoint is inside → move East
                {
                    d += dE;
                    dE += 2;       // both deltas shift by +2 when moving E
                    dSE += 2;
                }
                else                // midpoint is outside → move South-East
                {
                    d += dSE;
                    dE += 2;       // dE  shifts by +2
                    dSE += 4;       // dSE shifts by +4 (extra -2y term vanishes)
                    --y;
                }
                ++x;
                PlotCircleOctants(xc, yc, x, y, color);
            }
        }

        /// <summary>Mirrors one first-octant point (x, y) into all 8 octants.</summary>
        private void PlotCircleOctants(int xc, int yc, int x, int y, Color color)
        {
            PutPixel(xc + x, yc + y, color);
            PutPixel(xc - x, yc + y, color);
            PutPixel(xc + x, yc - y, color);
            PutPixel(xc - x, yc - y, color);
            PutPixel(xc + y, yc + x, color);
            PutPixel(xc - y, yc + x, color);
            PutPixel(xc + y, yc - x, color);
            PutPixel(xc - y, yc - x, color);
        }

        // ════════════════════════════════════════════════════════════════════
        // 4. XIAOLIN WU LINE  (Lecture 5, slide 30)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// For each x column the algorithm paints a PAIR of vertically
        /// adjacent pixels:
        ///
        ///   c1 = L*(1 - frac(y)) + B*frac(y)   ← upper pixel
        ///   c2 = L*frac(y)       + B*(1-frac(y)) ← lower pixel
        ///
        /// where frac(y) is the fractional distance of the ideal y from the
        /// upper grid line.  The steep case is handled by transposing x ↔ y.
        /// </summary>
        public void DrawXiaolinWuLine(int x1, int y1, int x2, int y2, Color color)
        {
            bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);

            // Transpose for steep lines so we always step in the major axis
            if (steep) { Swap(ref x1, ref y1); Swap(ref x2, ref y2); }
            if (x1 > x2) { Swap(ref x1, ref x2); Swap(ref y1, ref y2); }

            float dx = x2 - x1;
            float dy = y2 - y1;
            float m = (dx == 0) ? 1.0f : dy / dx;   // slope

            float y = y1;

            for (int x = x1; x <= x2; x++)
            {
                float frac = Frac(y);   // fractional part of y

                // c1: pixel closer to the line  (weight = 1 - frac)
                // c2: pixel further from line   (weight = frac)
                Color c1 = LerpColor(color, BackgroundColor, frac);
                Color c2 = LerpColor(color, BackgroundColor, 1.0f - frac);

                int iy = (int)Math.Floor(y);

                if (steep)
                {
                    // Transposed back: (x,y) → (y,x)
                    _db.BlendPixel(iy, x, c1);
                    _db.BlendPixel(iy + 1, x, c2);
                }
                else
                {
                    _db.BlendPixel(x, iy, c1);
                    _db.BlendPixel(x, iy + 1, c2);
                }

                y += m;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // 5. XIAOLIN WU CIRCLE  (Lecture 5, slides 31-32)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// For each y row the algorithm computes the exact arc x-position
        /// P = sqrt(R² - y²) and paints two horizontally adjacent pixels:
        ///
        ///   T  = D(R,y) = ceil(P) - P       (fractional overshoot)
        ///   c2 = L*(1-T) + B*T              ← outer pixel  (P2 = ceil)
        ///   c1 = L*T     + B*(1-T)          ← inner pixel  (P1 = floor)
        ///
        /// Computed for the first octant and mirrored into all 8.
        /// </summary>
        public void DrawXiaolinWuCircle(int r, int xc, int yc, Color color)
        {
            int x = r;
            int y = 0;

            // Top / bottom / left / right cardinal points
            _db.BlendPixel(xc + r, yc, color);
            _db.BlendPixel(xc - r, yc, color);
            _db.BlendPixel(xc, yc + r, color);
            _db.BlendPixel(xc, yc - r, color);

            while (x > y)
            {
                y++;

                // Exact arc x for this y
                double exactX = Math.Sqrt((double)r * r - (double)y * y);

                // T = ceil(exactX) - exactX  (the fractional "overshoot")
                float T = (float)(Math.Ceiling(exactX) - exactX);

                int xi = (int)Math.Floor(exactX);   // P1 = floor → inner
                int xi1 = xi + 1;                     // P2 = ceil  → outer

                // Inner pixel gets intensity T (closer to outside → dimmer)
                // Outer pixel gets intensity 1-T (closer to arc    → brighter)
                Color c1 = LerpColor(color, BackgroundColor, 1.0f - T); // inner
                Color c2 = LerpColor(color, BackgroundColor, T);         // outer

                PlotWuCircleOctants(xc, yc, y, xi, c1);
                PlotWuCircleOctants(xc, yc, y, xi1, c2);
            }
        }

        /// <summary>
        /// Mirrors one Wu-circle pixel pair into all 8 octants.
        /// (a, b) are the scan offsets in the first octant: a steps, b is arc.
        /// </summary>
        private void PlotWuCircleOctants(int xc, int yc, int a, int b, Color c)
        {
            _db.BlendPixel(xc + b, yc + a, c);
            _db.BlendPixel(xc - b, yc + a, c);
            _db.BlendPixel(xc + b, yc - a, c);
            _db.BlendPixel(xc - b, yc - a, c);
            _db.BlendPixel(xc + a, yc + b, c);
            _db.BlendPixel(xc - a, yc + b, c);
            _db.BlendPixel(xc + a, yc - b, c);
            _db.BlendPixel(xc - a, yc - b, c);
        }

        // ── Selection indicator ──────────────────────────────────────────────

        private void DrawSelectionIndicator(IShape shape)
        {
            Rectangle bounds = GetShapeBounds(shape);
            bounds.Inflate(4, 4);
            Color dash = Color.FromArgb(180, 0, 120, 215);

            for (int x = bounds.Left; x <= bounds.Right; x++)
            {
                if ((x & 1) == 0) continue;
                _db.BlendPixel(x, bounds.Top, dash);
                _db.BlendPixel(x, bounds.Bottom, dash);
            }
            for (int y = bounds.Top; y <= bounds.Bottom; y++)
            {
                if ((y & 1) == 0) continue;
                _db.BlendPixel(bounds.Left, y, dash);
                _db.BlendPixel(bounds.Right, y, dash);
            }
        }

        private static Rectangle GetShapeBounds(IShape shape) => shape switch
        {
            LineShape l => Rectangle.FromLTRB(
                Math.Min(l.StartPoint.X, l.EndPoint.X),
                Math.Min(l.StartPoint.Y, l.EndPoint.Y),
                Math.Max(l.StartPoint.X, l.EndPoint.X),
                Math.Max(l.StartPoint.Y, l.EndPoint.Y)),

            CircleShape c => new Rectangle(
                c.Center.X - c.Radius, c.Center.Y - c.Radius,
                c.Radius * 2, c.Radius * 2),

            PolygonShape p when p.Vertices.Count > 0 => GetPolygonBounds(p),
            _ => Rectangle.Empty
        };

        private static Rectangle GetPolygonBounds(PolygonShape p)
        {
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;
            foreach (var v in p.Vertices)
            {
                if (v.X < minX) minX = v.X; if (v.Y < minY) minY = v.Y;
                if (v.X > maxX) maxX = v.X; if (v.Y > maxY) maxY = v.Y;
            }
            return Rectangle.FromLTRB(minX, minY, maxX, maxY);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        /// <summary>
        /// Linear interpolation between two colours.
        /// t=0 → a,  t=1 → b
        /// Matches the slide formula: L*(1-t) + B*t
        /// </summary>
        private static Color LerpColor(Color a, Color b, float t)
        {
            t = t < 0f ? 0f : t > 1f ? 1f : t;
            int R = (int)(a.R * (1f - t) + b.R * t);
            int G = (int)(a.G * (1f - t) + b.G * t);
            int B = (int)(a.B * (1f - t) + b.B * t);
            return Color.FromArgb(255, R, G, B);
        }

        /// <summary>Fractional part of f  (always in [0, 1)).</summary>
        private static float Frac(float f) => f - (float)Math.Floor(f);

        private static void Swap<T>(ref T a, ref T b) { T t = a; a = b; b = t; }
    }
}