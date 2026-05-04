using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorGraphicsEditor.Models
{
    internal class PolygonShape : IShape
    {
        public List<Point> Vertices { get; set; } = new List<Point>();

        public bool IsClosed { get; set; } = false;

        private int _argb { get; set; } = Color.Black.ToArgb();
        private int _thickness = 1;

        public Color BaseColor
        {
            get => Color.FromArgb(_argb);
            set => _argb = value.ToArgb();
        }
        public int Thickness
        {
            get => _thickness;
            set => ShapeHelpers.ToOdd(value);
        }

        public bool HitTest(int x, int y)
        {
            int count = Vertices.Count;

            if(count < 2) return false; 

            int limit = IsClosed ? count : count - 1;

            for (int i = 0; i < limit; i++)
            {
                Point p1 = Vertices[i];
                Point p2 = Vertices[(i + 1) % count];
                double distSq = ShapeHelpers.PointToSegmentDistSq(
                    x, y,
                    p1.X, p1.Y,
                    p2.X, p2.Y);
                if (distSq <= ShapeHelpers.HitRadius * ShapeHelpers.HitRadius)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
