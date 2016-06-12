using ProjektBjometria;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ProjektBjometria
{

    abstract class  MinutiaFinder
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


    
}