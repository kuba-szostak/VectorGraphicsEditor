using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VectorGraphicsEditor.Models
{
    public class PolygonShape : IShape
    {
        public List<Point> Vertices { get; set; } = new List<Point>();

        public bool IsClosed { get; set; } = false;

        [JsonInclude]
        [JsonPropertyName("baseColor")]
        private int _argb { get; set; } = Color.Black.ToArgb();
        private int _thickness = 1;

        [JsonIgnore]
        public Color BaseColor
        {
            get => Color.FromArgb(_argb);
            set => _argb = value.ToArgb();
        }
        public int Thickness
        {
            get => _thickness;
            set => _thickness = ShapeHelpers.ToOdd(value);
        }

        public bool HitTest(int x, int y)
        {
            if (HitTestEdge(x, y) != -1) return true;
            if (IsClosed && IsPointInPolygon(x, y)) return true;
            return false;
        }

        private bool IsPointInPolygon(int x, int y)
        {
            if (Vertices.Count < 3) return false;
            bool inside = false;
            for (int i = 0, j = Vertices.Count - 1; i < Vertices.Count; j = i++)
            {
                if (((Vertices[i].Y > y) != (Vertices[j].Y > y)) &&
                    (x < (Vertices[j].X - Vertices[i].X) * (y - Vertices[i].Y) / (double)(Vertices[j].Y - Vertices[i].Y) + Vertices[i].X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        private Point GetCenter()
        {
            if (Vertices.Count == 0) return Point.Empty;
            int cx = (int)Vertices.Average(v => v.X);
            int cy = (int)Vertices.Average(v => v.Y);
            return new Point(cx, cy);
        }

        public void Move(int dx, int dy)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new Point(Vertices[i].X + dx, Vertices[i].Y + dy);
            }
        }

        public int HitTestHandle(int x, int y)
        {
            int bestIdx = -1;
            double minDistSq = double.MaxValue;
            double r2 = ShapeHelpers.HitRadius * ShapeHelpers.HitRadius;

            // Check vertices
            for (int i = 0; i < Vertices.Count; i++)
            {
                double d2 = ShapeHelpers.DistSq(x, y, Vertices[i].X, Vertices[i].Y);
                if (d2 <= r2)
                {
                    if (d2 < minDistSq)
                    {
                        minDistSq = d2;
                        bestIdx = i;
                    }
                }
            }

            // Check center handle
            if (Vertices.Count > 0)
            {
                Point center = GetCenter();
                double d2 = ShapeHelpers.DistSq(x, y, center.X, center.Y);
                if (d2 <= r2 && d2 < minDistSq)
                {
                    bestIdx = Vertices.Count;
                }
            }

            return bestIdx;
        }

        public void MoveHandle(int index, Point newPos)
        {
            if (index >= 0 && index < Vertices.Count)
            {
                Vertices[index] = newPos;
            }
            else if (index == Vertices.Count)
            {
                Point center = GetCenter();
                Move(newPos.X - center.X, newPos.Y - center.Y);
            }
        }

        public Point[] GetHandles()
        {
            var handles = new List<Point>(Vertices);
            if (Vertices.Count > 0)
                handles.Add(GetCenter());
            return handles.ToArray();
        }

        public int HitTestEdge(int x, int y)
        {
            int count = Vertices.Count;
            if (count < 2) return -1;
            int limit = IsClosed ? count : count - 1;

            for (int i = 0; i < limit; i++)
            {
                Point p1 = Vertices[i];
                Point p2 = Vertices[(i + 1) % count];
                double distSq = ShapeHelpers.PointToSegmentDistSq(x, y, p1.X, p1.Y, p2.X, p2.Y);
                if (distSq <= ShapeHelpers.HitRadius * ShapeHelpers.HitRadius)
                    return i;
            }
            return -1;
        }

        public void MoveEdge(int index, int dx, int dy)
        {
            if (index < 0 || index >= Vertices.Count) return;
            int next = (index + 1) % Vertices.Count;

            Vertices[index] = new Point(Vertices[index].X + dx, Vertices[index].Y + dy);
            // Only move next vertex if it exists (for non-closed polygons)
            if (IsClosed || index < Vertices.Count - 1)
            {
                Vertices[next] = new Point(Vertices[next].X + dx, Vertices[next].Y + dy);
            }
        }
    }
}
