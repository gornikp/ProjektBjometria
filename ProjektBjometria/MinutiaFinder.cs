using ProjektBjometria;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ProjektBjometria
{
    public class MinutiaManager
    {
        private CrosscutFinder crosscutFinder;
        private EndingFinder endingFinder;

        private Bitmap bitmap;

        private int imgWidth, imgHeight;
        

        public MinutiaManager(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            this.imgHeight = bitmap.Height;
            this.imgWidth = bitmap.Width;
            this.crosscutFinder = new CrosscutFinder(bitmap);
            this.endingFinder = new EndingFinder(bitmap);
        }

        public Bitmap findAndMarkMinutias()
        {
            Bitmap result = new Bitmap(imgWidth, imgHeight);
            for (int x = 0; x < imgWidth; x++)
            {
                for (int y = 0; y < imgHeight; y++)
                {
                    Pixel pixel = new Pixel()
                    {
                        point = new Point(x, y),
                        color = this.bitmap.GetPixel(x, y)
                    };

                    if (isBlack(pixel.color))
                    {
                        crosscutFinder.checkTestField(pixel);
                        endingFinder.checkTestField(pixel);
                    }
                    result.SetPixel(x, y, bitmap.GetPixel(x, y));
                }
            }
            result = crosscutFinder.getImageWithMarkMinutias(result);
            result = endingFinder.getImageWithMarkMinutias(result);
            return result;
        }

        private bool isBlack(int x, int y)
        {
            Color color = this.bitmap.GetPixel(x, y);
            return isBlack(color);
        }

        private bool isBlack(Color color)
        {
            return color.R == 0 && color.G == 0 && color.B == 0;
        }
    }

    

    abstract class MinutiaFinder
    {
        protected Bitmap bitmap;
        public Bitmap result;
        protected int imgWidth;
        protected int imgHeight;

        protected List<Pixel> minutias;

        protected int blackPointCounter;

        protected Color minutiaColor;
        
        

        protected MinutiaFinder(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            this.imgHeight = bitmap.Height;
            this.imgWidth = bitmap.Width;
            this.minutias = new List<Pixel>();
        }

        public abstract void checkTestField(Pixel pixel);

        public virtual Bitmap getImageWithMarkMinutias(Bitmap result)
        {
            this.result = result;
            foreach (Pixel pixel in minutias)
            {
                mark(pixel.point);
            }
            return result;
        }

        private void mark(Point point)
        {
            int x = point.X, y = point.Y;
            int envX, envY;
            int fieldWidth = 9;
            int counterDir = 4;
            int counter = 0;
            markPixelIfExists(x, y, false);

            for (envX = x - counterDir, envY = y - counterDir; counter < fieldWidth; counter++, envY++)
            {
                markPixelIfExists(envX, envY, true);
            }

            counter = 0;

            for (envX = x + counterDir, envY = y - counterDir; counter < fieldWidth; counter++, envY++)
            {
                markPixelIfExists(envX, envY, true);
            }

            counter = 0;

            for (envX = x - counterDir + 1, envY = y - counterDir; counter < (fieldWidth - 2);
                counter++, envX++)
            {
                markPixelIfExists(envX, envY, true);
            }

            counter = 0;

            for (envX = x - counterDir + 1, envY = y + counterDir; counter < (fieldWidth - 2);
               counter++, envX++)
            {
                markPixelIfExists(envX, envY, true);
            }

            int counterX = 0, counterY = 0;
            for (envX = x - 3; counterX < 7; envX++, counterX++)
            {
                for (envY = y - 3, counterY = 0; counterY < 7; envY++, counterY++)
                {
                    markPixelIfExists(envX, envY, false);
                }
            }
        }

        private void markPixelIfExists(int envX, int envY, Boolean setPixel)
        {
            if (isPixelExists(envX, envY))
            {
                if (setPixel)
                {
                    result.SetPixel(envX, envY, minutiaColor);
                }
                else
                {
                    result.SetPixel(envX, envY, bitmap.GetPixel(envX, envY));
                }
            }
        }
        

        protected bool isBlack(int x, int y)
        {
            Color color = this.bitmap.GetPixel(x, y);
            return isBlack(color);
        }

        protected bool isBlack(Color color)
        {
            return color.R == 0 && color.G == 0 && color.B == 0;
        }

        protected bool isNotBlack(int envX, int envY)
        {
            return !isBlack(envX, envY);
        }

        protected bool isPixelExists(int envX, int envY)
        {
            return envX >= 0 && envY >= 0 && envX < imgWidth && envY < imgHeight;
        }

        protected bool isWhite(int x, int y)
        {
            Color color = this.bitmap.GetPixel(x, y);
            return color.R == 255 && color.G == 255 && color.B == 255;
        }        

    }

    class CrosscutFinder : MinutiaFinder
    {
        private Color neighbour;

        public CrosscutFinder(Bitmap bitmap) : base(bitmap)
        {
            this.minutiaColor = Color.FromArgb(255, 0, 0);
        }

        public override void checkTestField(Pixel pixel)
        {
            checkTestField(pixel, 5);
            if (isCrosscut())
            {
                checkTestField(pixel, 9);

                if (isCrosscut())
                {
                    minutias.Add(pixel);
                    return;
                }
            }
        }

        private void checkTestField(Pixel pixel, int fieldWidth)
        {
            blackPointCounter = 0;
            Point point = pixel.point;
            int x = point.X;
            int y = point.Y;
            int envX, envY;
            int counterDir = (fieldWidth - 1) / 2;
            int counter = 0;
            neighbour = Color.FromArgb(255, 255, 255);

            for (envX = x - counterDir, envY = y - counterDir; counter < fieldWidth; counter++, envY++)
            {
                incrementCrosscutCounterIfPixelIsCorrect(envX, envY);
            }

            counter = 0;

            for (envX = x - counterDir + 1, envY = y + counterDir; counter < (fieldWidth - 2);
               counter++, envX++)
            {
                incrementCrosscutCounterIfPixelIsCorrect(envX, envY);
            }

            counter = 0;

            for (envX = x + counterDir, envY = y + counterDir; counter < fieldWidth; counter++, envY--)
            {
                incrementCrosscutCounterIfPixelIsCorrect(envX, envY);
            }

            counter = 0;

            for (envX = x + counterDir - 1, envY = y - counterDir; counter < (fieldWidth - 2);
                counter++, envX--)
            {
                incrementCrosscutCounterIfPixelIsCorrect(envX, envY);
            }

            counter = 0;
        }

        private void incrementCrosscutCounterIfPixelIsCorrect(int envX, int envY)
        {
            if (isPixelExists(envX, envY))
            {
                if (isNotBlack(envX, envY))
                {
                    if (isBlack(neighbour))
                    {
                        blackPointCounter++;
                    }
                    neighbour = Color.FromArgb(255, 255, 255);
                }
                else
                {
                    neighbour = Color.FromArgb(0, 0, 0);
                }
            }
        }

        private bool isCrosscut()
        {
            return blackPointCounter == 3;
        }

        public override Bitmap getImageWithMarkMinutias(Bitmap result)
        {
            filterMinutias();
            return base.getImageWithMarkMinutias(result);
        }

        private void filterMinutias()
        {
            int size = minutias.Count;
            for (int i = 0; i < minutias.Count; i++)
            {
                Pixel firstPoint = minutias[i];
                removePointIfExistsSimilarOne(firstPoint);
            }
        }

        private void removePointIfExistsSimilarOne(Pixel firstPoint)
        {
            for (int j = 0; j < minutias.Count; j++)
            {
                Pixel secondPoint = minutias[j];
                if (!firstPoint.Equals(secondPoint))
                {
                    double distance = countDistance(firstPoint.point, secondPoint.point);
                    if (distance < 9.0)
                    {
                        minutias.Remove(secondPoint);
                        removePointIfExistsSimilarOne(firstPoint);
                        return;
                    }
                }
            }
        }

        private double countDistance(Point firstPoint, Point secondPoint)
        {
            double distance = 0;
            double xPart = Math.Pow(secondPoint.X - firstPoint.X, 2);
            double yPart = Math.Pow(secondPoint.Y - firstPoint.Y, 2);

            distance = Math.Abs(Math.Sqrt(xPart + yPart));

            return distance;
        }

    }

    class EndingFinder : MinutiaFinder
    {
        public EndingFinder(Bitmap bitmap) : base(bitmap)
        {
            this.minutiaColor = Color.FromArgb(0, 0, 255);
        }

        public override void checkTestField(Pixel pixel)
        {
            Point point = pixel.point;
            blackPointCounter = 0;
            checkEndings(point);
            if (isEnding())
            {
                if (filterEndings(point))
                {
                    minutias.Add(pixel);
                    return;
                }
            }
        }

        private void checkEndings(Point point)
        {
            int x = point.X, y = point.Y;
            int envX, envY;
            int counter = 0;

            for (envX = x - 1, envY = y - 1; counter < 3; counter++, envY++)
            {
                incrementCounterIfPixelIsCorrect(envX, envY);
            }
            counter = 0;

            for (envX = x + 1, envY = y - 1; counter < 3; counter++, envY++)
            {
                incrementCounterIfPixelIsCorrect(envX, envY);
            }
            counter = 0;

            incrementCounterIfPixelIsCorrect(envX = x, envY = y - 1);


            incrementCounterIfPixelIsCorrect(envX = x, envY = y + 1);

        }

        private void incrementCounterIfPixelIsCorrect(int envX, int envY)
        {
            if (isPixelExists(envX, envY))
            {
                if (isBlack(envX, envY))
                {
                    blackPointCounter++;
                }
            }
        }
   
        private bool isEnding()
        {
            return blackPointCounter == 1;
        }

        private bool filterEndings(Point point)
        {
            int x = point.X, y = point.Y;
            int counter = 0, counter2 = 0;
            for (int i = x; i < imgWidth; i++)
            {
                if (isWhite(i, y))
                {
                    counter++;
                }
            }
            for (int i = x; i >= 0; i--)
            {
                if (isWhite(i, y))
                {
                    counter2++;
                }

            }
            if (counter2 == x || counter == (imgWidth - x - 1))
            {
                return false;
            }
            else return true;
        }

    }

    class Pixel
    {
        public Point point { get; set; }

        public Color color { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Pixel)
            {
                Pixel pi = (Pixel)obj;
                Point p = pi.point;
                return (p.X == this.point.X) && (p.Y == this.point.Y);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}