using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjektBjometria
{
    public class CustomBitmapProcessing
    {
        Bitmap source = null;
        IntPtr Iptr = IntPtr.Zero;
        BitmapData bitmapData = null;

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Step { get; private set; }

        public CustomBitmapProcessing(Bitmap source)
        {
            this.source = source;
        }

        public void LockBits()
        {
            try
            {
                Width = source.Width;
                Height = source.Height;
                Rectangle rect = new Rectangle(0, 0, Width, Height);

                Depth = Image.GetPixelFormatSize(source.PixelFormat);
                Step = Depth / 8;
                if (Step == 0) Step = 1;
                if (Depth != 8 && Depth != 24 && Depth != 32 && Depth != 1)
                    throw new ArgumentException("Only 1, 8, 24 and 32 bpp images are supported.");
                bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite, source.PixelFormat);
                int PixelCount = bitmapData.Stride * Height;
                Pixels = new byte[PixelCount];
                Iptr = bitmapData.Scan0;
                Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UnlockBits()
        {
            try
            {
                Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);
                source.UnlockBits(bitmapData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetPixel(int x, int y)
        {
            int rgb = 0;
            int i = (x * bitmapData.Stride) + y * Step;

            if (i > Pixels.Length - Step)
                throw new IndexOutOfRangeException();
            if (Depth == 32 || Depth == 24)
            {
                int b = Pixels[i];
                int g = Pixels[i + 1];
                int r = Pixels[i + 2];
                rgb = r + (g << 8) + (b << 16);
                return rgb;
            }
            if (Depth == 8 || Depth == 1)
            {
                int c = Pixels[i];
                rgb = c + (c << 8) + (c << 16);
                return rgb;
            }
            return 0;
        }

        public void SetPixel(int x, int y, int rgb)
        {
            int i = (x * bitmapData.Stride) + y * Step;

            if (Depth == 32)
            {
                Pixels[i + 2] = (byte)(rgb & 0x0000FF);
                Pixels[i + 1] = (byte)((rgb >> 8) & 0x0000FF);
                Pixels[i] = (byte)((rgb >> 16) & 0x0000FF);
            }
            if (Depth == 24)
            {
                Pixels[i] = (byte)(rgb & 0x0000FF);
                Pixels[i + 1] = (byte)((rgb >> 8) & 0x0000FF);
                Pixels[i + 2] = (byte)((rgb >> 16) & 0x0000FF);
            }
            if (Depth == 8 || Depth == 1)
            {
                Pixels[i] = (byte)(rgb & 0x0000FF);
            }
        }
    }
}
