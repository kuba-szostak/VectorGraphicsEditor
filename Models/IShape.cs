using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace VectorGraphicsEditor.Models
{
    [JsonDerivedType(typeof(LineShape), typeDiscriminator: "line")]
    [JsonDerivedType(typeof(CircleShape), typeDiscriminator: "circle")]
    [JsonDerivedType(typeof(PolygonShape), typeDiscriminator: "polygon")]
    public interface IShape
    {
        Color BaseColor { get; set; }

        int Thickness { get; set; }

        bool HitTest(int x, int y);
    }


    internal static class ShapeHelpers
    {
        public static double DistSq(int x1, int y1, int x2, int y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }

        public static int ToOdd(int value)
        {
            if (value < 1) value = 1;
            return value % 2 == 0 ? value + 1 : value;
        }

        public static double PointToSegmentDistSq(int px, int py,
                                                    int ax, int ay, 
                                                    int bx, int by)
        {
            double dx = bx - ax, dy = by - ay;
            double lenSq = dx * dx + dy * dy;

            if(lenSq < 1e-10) { return DistSq(px, py, ax, ay); }

            double t = ((px - ax) * dx + (py - ay) * dy) / lenSq;

            t = t < 0 ? 0 : (t > 1 ? 1 : t);

            double closestX = ax + t * dx;
            double closestY = ay + t * dy;
            double ex = px - closestX, ey = py - closestY;

            return ex * ex + ey * ey;
        }

        public const int HitRadius = 5;
    }
}
