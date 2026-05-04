using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VectorGraphicsEditor.Models
{
    public sealed class LineShape : IShape
    {

        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        [JsonIgnore]
        public Color BaseColor 
        { 
            get => Color.FromArgb(_argb); 
            set => _argb = value.ToArgb(); 
        }

        [JsonInclude]
        [JsonPropertyName("baseColor")]
        private int _argb { get; set; } = Color.Black.ToArgb();

        private int _thickness = 1;
        public int Thickness 
        { 
            get => _thickness;
            set => _thickness = ShapeHelpers.ToOdd(value);
        }

        public bool HitTest(int x, int y)
        {
            double distSq = ShapeHelpers.PointToSegmentDistSq(
                x, y,
                StartPoint.X, StartPoint.Y,
                EndPoint.X, EndPoint.Y);

            return distSq <= ShapeHelpers.HitRadius * ShapeHelpers.HitRadius;
        }

        public void Move(int dx, int dy)
        {
            StartPoint = new Point(StartPoint.X + dx, StartPoint.Y + dy);
            EndPoint = new Point(EndPoint.X + dx, EndPoint.Y + dy);
        }

        public int HitTestHandle(int x, int y)
        {
            double d0 = ShapeHelpers.DistSq(x, y, StartPoint.X, StartPoint.Y);
            double d1 = ShapeHelpers.DistSq(x, y, EndPoint.X, EndPoint.Y);
            double r2 = ShapeHelpers.HitRadius * ShapeHelpers.HitRadius;

            if (d0 <= r2 && (d0 <= d1 || d1 > r2)) return 0;
            if (d1 <= r2) return 1;
            return -1;
        }

        public void MoveHandle(int index, Point newPos)
        {
            if (index == 0) StartPoint = newPos;
            else if (index == 1) EndPoint = newPos;
        }

        public Point[] GetHandles() => new[] { StartPoint, EndPoint };

        public int HitTestEdge(int x, int y)
        {
            // For a line, the "edge" is the line itself.
            return HitTest(x, y) ? 0 : -1;
        }

        public void MoveEdge(int index, int dx, int dy) => Move(dx, dy);
    }
}
