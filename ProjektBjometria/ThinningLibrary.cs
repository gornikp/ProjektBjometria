using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ProjektBjometria
{
    public class ThinningLibrary
    {
        private int BinaryValidator(int a)
        {
            if (a == 255) return 0;
            if (a == 1) return 0;
            if (a == 0) return 1;
            else return 1;
        }
        private int BinaryValidator2(int a)
        {
            if (a != 0) return 0;
            else return 255;
        }
        private Bitmap ApplyThreshold(Bitmap inputBitmap, int threshold)
        {
            CustomBitmapProcessing data = new CustomBitmapProcessing(inputBitmap);
            data.LockBits();
            for (int i = 0; i < data.Height; i++)
            {
                for (int j = 0; j < data.Width; j++)
                {
                    if ((data.GetPixel(i, j) & 0xFF) >= threshold)
                    {
                        int rgb = 255 + (255 << 8) + (255 << 16);
                        data.SetPixel(i, j, rgb);
                    }
                    else
                    {
                        data.SetPixel(i, j, 0);
                    }
                }
            }
            data.UnlockBits();
            return inputBitmap;
        }
        private ulong[] CreateHistogram(Bitmap inputBitmap)
        {
            ulong[] histogram = new ulong[256];
            CustomBitmapProcessing data = new CustomBitmapProcessing(inputBitmap);
            data.LockBits();
            for (int i = 0; i < data.Height; i++)
            {
                for (int j = 0; j < data.Width; j++)
                {
                    int gray = (data.GetPixel(i, j) & 0xFF);
                    histogram[gray]++;
                }
            }
            data.UnlockBits();
            return histogram;
        }
        private Bitmap ApplyGrayScale(Bitmap inputBitmap)
        {
            CustomBitmapProcessing data = new CustomBitmapProcessing(inputBitmap);
            data.LockBits();
            for (int i = 0; i < data.Height; i++)
            {
                for (int j = 0; j < data.Width; j++)
                {
                    int rgb = data.GetPixel(i, j);
                    byte grey = (byte)(Math.Ceiling((decimal)(0.299 * (rgb & 0xFF) + 0.587 * ((rgb >> 8) & 0xFF) + 0.114 * ((rgb >> 16) & 0xFF))));
                    int rgbOut = grey + (grey << 8) + (grey << 16);
                    data.SetPixel(i, j, rgbOut);
                }
            }
            data.UnlockBits();
            return inputBitmap;
        }
        public Bitmap TransformOtsu(Bitmap inputBitmap)
        {
            Bitmap renderedImage = ApplyGrayScale(inputBitmap);
            ulong[] histogram = CreateHistogram(renderedImage);
            double[] variancies = new double[256];
            for (uint group = 0; group < 256; group++)
            {
                double objectDepth = 0, backgrDepth = 0;
                for (uint i = 0; i < group; i++)
                {
                    objectDepth += histogram[i];
                }
                for (uint i = group; i < 256; i++)
                {
                    backgrDepth += histogram[i];
                }
                double objectMiddleDepth = 0, backgrMiddleDepth = 0;
                for (uint i = 0; i < group; i++)
                {
                    objectMiddleDepth += (histogram[i] * i) / objectDepth;
                }
                for (uint i = group; i < 256; i++)
                {
                    backgrMiddleDepth += (histogram[i] * i) / backgrDepth;
                }

                variancies[group] = Math.Sqrt(objectDepth * backgrDepth * Math.Pow(objectMiddleDepth - backgrMiddleDepth, 2));
            }
            int boundary = Array.IndexOf(variancies, variancies.Max());
            return ApplyThreshold(renderedImage, boundary);
        }
        private Bitmap originalImage;
        private Bitmap filteredImage;

        int[,] imageM;
        int width;
        int height;


        public Bitmap processImage(Bitmap image)
        {
            originalImage = (Bitmap)image.Clone();
            width = originalImage.Width;
            height = originalImage.Height;
            filteredImage = new Bitmap(width, height, originalImage.PixelFormat);
            CustomBitmapProcessing data = new CustomBitmapProcessing(originalImage);
            CustomBitmapProcessing dataOut = new CustomBitmapProcessing(filteredImage);
            data.LockBits();
            dataOut.LockBits();

            imageM = new int[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    int col = (data.GetPixel(i, j) & 0xFF);
                    imageM[i, j] = BinaryValidator(col);
                }
            }
            while (true)
            {

                int[,] start = new int[height, width];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        start[i, j] = imageM[i, j];
                    }
                }

                thiningIteration(0);
                thiningIteration(1);

                bool same = true;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (start[i, j] != imageM[i, j])
                        {
                            same = false;
                            //break;
                            goto MainforLoop;
                        }

                    }
                }
                MainforLoop:
                if (same)
                {
                    break;
                }

            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int col;
                    col = BinaryValidator2(imageM[i, j]);
                    int rgb = col + (col << 8) + (col << 16);
                    dataOut.SetPixel(i, j, rgb);
                }
            }
            data.UnlockBits();
            dataOut.UnlockBits();
            return filteredImage;
        }

        public void thiningIteration(int iter)
        {
            int[,] marker = new int[height, width];

            for (int i = 1; i < height - 1; i++)
            {
                for (int j = 1; j < width - 1; j++)
                {

                    int p2 = imageM[i - 1, j];
                    int p3 = imageM[i - 1, j + 1];
                    int p4 = imageM[i, j + 1];
                    int p5 = imageM[i + 1, j + 1];
                    int p6 = imageM[i + 1, j];
                    int p7 = imageM[i + 1, j - 1];
                    int p8 = imageM[i, j - 1];
                    int p9 = imageM[i - 1, j - 1];
                    int c1 = 0; //p2 == 0 && p3 == 1
                    int c2 = 0; //p3 == 0 && p4 == 1
                    int c3 = 0; //p4 == 0 && p5 == 1
                    int c4 = 0; //p5 == 0 && p6 == 1
                    int c5 = 0; //p6 == 0 && p7 == 1
                    int c6 = 0; //p7 == 0 && p8 == 1
                    int c7 = 0; //p8 == 0 && p9 == 1
                    int c8 = 0; //p9 == 0 && p2 == 1

                    if (p2 == 0 && p3 == 1)
                    {
                        c1 = 1;
                    }
                    if (p3 == 0 && p4 == 1)
                    {
                        c2 = 1;
                    }
                    if (p4 == 0 && p5 == 1)
                    {
                        c3 = 1;
                    }
                    if (p5 == 0 && p6 == 1)
                    {
                        c4 = 1;
                    }
                    if (p6 == 0 && p7 == 1)
                    {
                        c5 = 1;
                    }
                    if (p7 == 0 && p8 == 1)
                    {
                        c6 = 1;
                    }
                    if (p8 == 0 && p9 == 1)
                    {
                        c7 = 1;
                    }
                    if (p9 == 0 && p2 == 1)
                    {
                        c8 = 1;
                    }

                    int A = c1 + c2 + c3 + c4 + c5 + c6 + c7 + c8;
                    int B = p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9;

                    int m1 = iter == 0 ? (p2 * p4 * p6) : (p2 * p4 * p8);
                    int m2 = iter == 0 ? (p4 * p6 * p8) : (p2 * p6 * p8);

                    if (A == 1 && (B >= 2 && B <= 6) && m1 == 0 && m2 == 0)
                    {
                        marker[i, j] = 1;
                    }

                }
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    int tmp = 1 - marker[i, j];
                    if (imageM[i, j] == tmp && imageM[i, j] == 1)
                    {
                        imageM[i, j] = 1;
                    }
                    else
                    {
                        imageM[i, j] = 0;
                    }

                }
            }

        }


        int[,] N = { { 32, 64, 128 }, { 16, 0, 1 }, { 8, 4, 2 } };
        int[] A0 = { 3, 6, 7, 12, 14, 15, 24, 28, 30, 31, 48, 56, 60, 62, 63, 96, 112, 120, 124, 126, 127, 129, 131, 135, 143, 159, 191, 192, 193, 195, 199, 207, 223, 224, 225, 227, 231, 239, 240, 241, 243, 247, 248, 249, 251, 252, 253, 254 };
        int[] A1 = { 7, 14, 28, 56, 112, 131, 193, 224 };
        int[] A2 = { 7, 14, 15, 28, 30, 56, 60, 112, 120, 131, 135, 193, 195, 224, 225, 240 };
        int[] A3 = { 7, 14, 15, 28, 30, 31, 56, 60, 62, 112, 120, 124, 131, 135, 143, 193, 195, 199, 224, 225, 227, 240, 241, 248 };
        int[] A4 = { 7, 14, 15, 28, 30, 31, 56, 60, 62, 63, 112, 120, 124, 126, 131, 135, 143, 159, 193, 195, 199, 207, 224, 225, 227, 231, 240, 241, 243, 248, 249, 252 };
        int[] A5 = { 7, 14, 15, 28, 30, 31, 56, 60, 62, 63, 112, 120, 124, 126, 131, 135, 143, 159, 191, 193, 195, 199, 207, 224, 225, 227, 231, 239, 240, 241, 243, 248, 249, 251, 252, 254 };
        int[] A1pix = { 3, 6, 7, 12, 14, 15, 24, 28, 30, 31, 48, 56, 60, 62, 63, 96, 112, 120, 124, 126, 127, 129, 131, 135, 143, 159, 191, 192, 193, 195, 199, 207, 223, 224, 225, 227, 231, 239, 240, 241, 243, 247, 248, 249, 251, 252, 253, 254 };

        private Bitmap Thinner(Bitmap inputImage, int[] weights)
        {
            for (int x = 1; x < inputImage.Height - 1; x++)
            {
                for (int y = 0; y < inputImage.Width - 1; y++)
                {
                    int weight = 0;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            weight += N[i + 1, j + 1] * BinaryValidator(inputImage.GetPixel(x + i, y + j).B);
                        }
                    }
                    if (weights.Contains(weight))
                    {
                        inputImage.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return inputImage;
        }

        private int[,] Border(Bitmap inputImage, int[] weights)
        {
            int[,] B = new int[inputImage.Height, inputImage.Width];
            for (int x = 1; x < inputImage.Height; x++)
            {
                for (int y = 1; y < inputImage.Width; y++)
                {
                    int bit = BinaryValidator(inputImage.GetPixel(x, y).R);
                    if (bit == 0) continue;
                    int weight = 0;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            weight += N[i + 1, j + 1] * BinaryValidator(inputImage.GetPixel(x+i, y+j).R);
                        }
                    }
                    if (weights.Contains(weight))
                    {
                        B[x, y] = 1;
                    }
                }
            }

            return B;
        }

    }
}
