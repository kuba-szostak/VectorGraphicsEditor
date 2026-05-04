using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VectorGraphicsEditor.Rendering
{
    public sealed class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; }
        public int Width { get; }
        public int Height { get; }


        private readonly int[] _pixels;
        private readonly GCHandle _handle;
        private bool _disposed;

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;

            _pixels = new int[width * height];

            // Pin the array so the GC never moves it while GDI+ holds a pointer
            _handle = GCHandle.Alloc(_pixels, GCHandleType.Pinned);

            Bitmap = new Bitmap(
                width, height,
                width * 4,  // stride (4 bytes per pixel)
                PixelFormat.Format32bppArgb,
                _handle.AddrOfPinnedObject()); // pointer to the pixel data
        }

        public void SetPixel(int x, int y, Color color)
        {
            if ((uint)x >= (uint)Width || (uint)y >= (uint)Height) return;
            _pixels[y * Width + x] = color.ToArgb();
        }

        public void BlendPixel(int x, int y, Color color)
        {
            if ((uint)x >= (uint)Width || (uint)y >= (uint)Height) return;
            int index = y * Width + x;
            int existingArgb = _pixels[index];
            Color existingColor = Color.FromArgb(existingArgb);
            // Simple alpha blending
            float alpha = color.A / 255f;
            int r = (int)(color.R * alpha + existingColor.R * (1 - alpha));
            int g = (int)(color.G * alpha + existingColor.G * (1 - alpha));
            int b = (int)(color.B * alpha + existingColor.B * (1 - alpha));
            _pixels[index] = Color.FromArgb(255, r, g, b).ToArgb();
        }

        public void Clear(Color color)
        {
            int argb = color.ToArgb();
            Array.Fill(_pixels, argb);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Bitmap.Dispose();
            _handle.Free();
        }
    }
}
