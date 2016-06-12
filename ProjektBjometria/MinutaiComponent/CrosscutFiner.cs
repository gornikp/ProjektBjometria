using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektBjometria
{
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
}
