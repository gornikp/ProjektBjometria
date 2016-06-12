using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektBjometria
{
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
}
