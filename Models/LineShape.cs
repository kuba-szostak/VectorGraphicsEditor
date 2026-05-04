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
    }
}
