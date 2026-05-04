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

            return Math.Abs(distance - Radius) <= ShapeHelpers.HitRadius;
        }
    }
}
