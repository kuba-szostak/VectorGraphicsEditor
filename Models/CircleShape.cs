using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VectorGraphicsEditor.Models
{
    public sealed class CircleShape : IShape
    {
        public Point Center { get; set; }
        public int Radius { get; set; }

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
            double dx = x - Center.X;
            double dy = y - Center.Y;

            double distance = System.Math.Sqrt(dx * dx + dy * dy);

            // Hit test includes the interior for easier grabbing
            return distance <= Radius + ShapeHelpers.HitRadius;
        }

        public void Move(int dx, int dy)
        {
            Center = new Point(Center.X + dx, Center.Y + dy);
        }

        public int HitTestHandle(int x, int y)
        {
            double d0 = ShapeHelpers.DistSq(x, y, Center.X, Center.Y);
            // Handle for radius at (Center.X + Radius, Center.Y)
            double d1 = ShapeHelpers.DistSq(x, y, Center.X + Radius, Center.Y);
            double r2 = ShapeHelpers.HitRadius * ShapeHelpers.HitRadius;

            if (d0 <= r2 && (d0 <= d1 || d1 > r2)) return 0;
            if (d1 <= r2) return 1;
            return -1;
        }

        public void MoveHandle(int index, Point newPos)
        {
            if (index == 0) Center = newPos;
            else if (index == 1)
            {
                Radius = (int)Math.Sqrt(ShapeHelpers.DistSq(Center.X, Center.Y, newPos.X, newPos.Y));
            }
        }

        public Point[] GetHandles() => new[] { Center, new Point(Center.X + Radius, Center.Y) };

        public int HitTestEdge(int x, int y) => HitTest(x, y) ? 0 : -1;

        public void MoveEdge(int index, int dx, int dy) => Move(dx, dy);
    }
}
