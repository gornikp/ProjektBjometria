using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
        public int[,] BitmapToTable(Bitmap inputBitmap)
        {
            CustomBitmapProcessing data = new CustomBitmapProcessing(inputBitmap);
            data.LockBits();
            int[,] outputTab = new int[data.Height, data.Width];
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    int rgb = data.GetPixel(60, 50);
                    rgb = data.GetPixel(y, x);
                    outputTab[y, x] = BinaryValidator(rgb & 0xFF);
                }
            }
            data.UnlockBits();
            return outputTab;
        }
        public Bitmap TableToBitmap(int[,] inputTabe)
        {
            Bitmap newBitmap = new Bitmap(inputTabe.GetLength(1), inputTabe.GetLength(0),PixelFormat.Format8bppIndexed);
            CustomBitmapProcessing data = new CustomBitmapProcessing(newBitmap);
            data.LockBits();          
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    int rgb = BinaryValidator2(inputTabe[y, x]) + (BinaryValidator2(inputTabe[y, x]) << 8) + (BinaryValidator2(inputTabe[y, x]) << 16);
                    data.SetPixel(y, x, rgb);
                }
            }
            data.UnlockBits();
            return newBitmap;
        }
        public int[,] doZhangSuenThinning(int[,] givenImage, bool changeGivenImage)
        {
            Bitmap elo = new Bitmap(100, 100, PixelFormat.Format1bppIndexed);
            int[,] binaryImage;
            if (changeGivenImage)
            {
                binaryImage = givenImage;
            }
            else
            {
                binaryImage = (int[,])givenImage.Clone();
            }
            int a, b;
            List<Point> pointsToChange = new List<Point>();
            bool hasChange;
            int Width = binaryImage.GetLength(1);
            int Height = binaryImage.GetLength(0);
            do
            {
                hasChange = false;
                for (int y = 1; y + 1 <Height; y++)
                {
                    for (int x = 1; x + 1 < Width; x++)
                    {
                        a = getA(binaryImage, y, x);
                        b = getB(binaryImage, y, x);
                        if (binaryImage[y,x] == 1 && 2 <= b && b <= 6 && a == 1
                                && (binaryImage[y - 1,x] * binaryImage[y,x + 1] * binaryImage[y + 1,x] == 0)
                                && (binaryImage[y,x + 1] * binaryImage[y + 1,x] * binaryImage[y,x - 1] == 0))
                        {
                            pointsToChange.Add(new Point(x, y));
                            //binaryImage[y][x] = 0;
                            hasChange = true;
                        }
                    }
                }
                foreach (Point point in pointsToChange)
                {
                    binaryImage[point.Y,point.X] = 0;
                }
                pointsToChange.Clear();
                for (int y = 1; y + 1 < Height; y++)
                {
                    for (int x = 1; x + 1 < Width; x++)
                    {
                        a = getA(binaryImage, y, x);
                        b = getB(binaryImage, y, x);
                        if (binaryImage[y,x] == 1 && 2 <= b && b <= 6 && a == 1
                                && (binaryImage[y - 1,x] * binaryImage[y,x + 1] * binaryImage[y,x - 1] == 0)
                                && (binaryImage[y - 1,x] * binaryImage[y + 1,x] * binaryImage[y,x - 1] == 0))
                        {
                            pointsToChange.Add(new Point(x, y));
                            hasChange = true;
                        }
                    }
                }
                foreach (Point point in pointsToChange)
                {
                    binaryImage[point.Y,point.X] = 0;
                }
                pointsToChange.Clear();
            } while (hasChange);
            return binaryImage;
        }

        private int getA(int[,] binaryImage, int y, int x)
        {
            int Width = binaryImage.GetLength(1);
            int Height = binaryImage.GetLength(0);
            int count = 0;
            //p2 p3
            if (y - 1 >= 0 && x + 1 < binaryImage.GetLength(1) && binaryImage[y - 1,x] == 0 && binaryImage[y - 1,x + 1] == 1)
            {
                count++;
            }
            //p3 p4
            if (y - 1 >= 0 && x + 1 < binaryImage.GetLength(1) && binaryImage[y - 1,x + 1] == 0 && binaryImage[y,x + 1] == 1)
            {
                count++;
            }
            //p4 p5
            if (y + 1 < binaryImage.GetLength(0) && x + 1 < binaryImage.GetLength(1) && binaryImage[y,x + 1] == 0 && binaryImage[y + 1,x + 1] == 1)
            {
                count++;
            }
            //p5 p6
            if (y + 1 < binaryImage.GetLength(0) && x + 1 < binaryImage.GetLength(1) && binaryImage[y + 1,x + 1] == 0 && binaryImage[y + 1,x] == 1)
            {
                count++;
            }
            //p6 p7
            if (y + 1 < binaryImage.GetLength(0) && x - 1 >= 0 && binaryImage[y + 1,x] == 0 && binaryImage[y + 1,x - 1] == 1)
            {
                count++;
            }
            //p7 p8
            if (y + 1 < binaryImage.GetLength(0) && x - 1 >= 0 && binaryImage[y + 1,x - 1] == 0 && binaryImage[y,x - 1] == 1)
            {
                count++;
            }
            //p8 p9
            if (y - 1 >= 0 && x - 1 >= 0 && binaryImage[y,x - 1] == 0 && binaryImage[y - 1,x - 1] == 1)
            {
                count++;
            }
            //p9 p2
            if (y - 1 >= 0 && x - 1 >= 0 && binaryImage[y - 1,x - 1] == 0 && binaryImage[y - 1,x] == 1)
            {
                count++;
            }
            return count;
        }

        private int getB(int[,] binaryImage, int y, int x)
        {
            int Width = binaryImage.GetLength(1);
            int Height = binaryImage.GetLength(0);
            return binaryImage[y - 1,x] + binaryImage[y - 1,x + 1] + binaryImage[y,x + 1]
                    + binaryImage[y + 1,x + 1] + binaryImage[y + 1,x] + binaryImage[y + 1,x - 1]
                    + binaryImage[y,x - 1] + binaryImage[y - 1,x - 1];
        }
    }
}
