using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private bool isBlack(Color color)
        {
            return color.R == 0 && color.G == 0 && color.B == 0;
        }
    }
}
