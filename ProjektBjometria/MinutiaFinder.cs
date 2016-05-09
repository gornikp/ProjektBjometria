using ProjektBjometria;
using System;
using System.Collections.Generic;
using System.Drawing;

public class MinutiaFinder
{
    private Bitmap bitmap;
    public Bitmap result { get; set; }
    private CustomBitmapProcessing processing;
    private int imgWidth;
    private int imgHeight;
    private int blackPointCounter;
    private List<Point> crosscuts;
    private List<Point> endings;
    private bool[,] isPixelSet;
    private int minutiaCounter = 0;
    private Color neighbour;

    public MinutiaFinder(Bitmap bitmap)
    {
        this.bitmap = bitmap;
        this.processing = new CustomBitmapProcessing(bitmap);
        this.imgHeight = bitmap.Height;
        this.imgWidth = bitmap.Width;
        this.isPixelSet = new bool[imgWidth, imgHeight];
        this.crosscuts = new List<Point>();
        this.endings = new List<Point>();
        this.result = new Bitmap(imgWidth, imgHeight);
    }

    public void findCrosscuts()
    {
        for (int x = 0; x < imgWidth; x++)
        {
            for (int y = 0; y < imgHeight; y++)
            {
                blackPointCounter = 0;
                if (isBlack(x, y))
                {
                    checkTestFields(x, y);
                    blackPointCounter = 0;
                    checkTestEndings(x, y);
                }
                result.SetPixel(x, y, bitmap.GetPixel(x, y));
            }
        }
        Console.WriteLine("MinutiaCounter: " + minutiaCounter);
        filterMinutias();
        markAllEndings();
        Console.WriteLine("MinutiaCounter: " + crosscuts.Count);
    }

    private void markAllEndings()
    {
        int size = endings.Count;

        for (int i = 0; i < endings.Count; i++)
        {
            Point p = endings[i];

            markEndings(p.X, p.Y);
        }
    }

    private void filterMinutias()
    {
        int size = crosscuts.Count;
        for (int i = 0; i < crosscuts.Count; i++)
        {
            Point firstPoint = crosscuts[i];
            for (int j = 0; j < crosscuts.Count; j++)
            {
                Point secondPoint = crosscuts[j];
                if (!firstPoint.Equals(secondPoint))
                {
                    double distance = countDistance(firstPoint, secondPoint);
                    if (distance < 20)
                    {
                        crosscuts.Remove(secondPoint);
                    }
                }
            }
        }

        foreach (Point point in crosscuts)
        {
            markCrosscut(point.X, point.Y);
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

    private void checkTestFields(int x, int y)
    {
        checkTestField(x, y, 5);
        if (isCrosscut())
        {
            checkTestField(x, y, 9);

            if (isCrosscut())
            {
                crosscuts.Add(new Point(x, y));
                minutiaCounter++;
                return;
            }
        }
    }

    private void markCrosscut(int x, int y)
    {
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
            isPixelSet[envX, envY] = true;
            if (setPixel)
            {
                result.SetPixel(envX, envY, Color.FromArgb(255, 0, 0));
            }
            else
            {
                result.SetPixel(envX, envY, bitmap.GetPixel(envX, envY));
            }
        }
    }

    private bool isCrosscut()
    {
        return blackPointCounter == 3;
    }

    private void checkTestField(int x, int y, int fieldWidth)
    {
        blackPointCounter = 0;
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

    private bool isBlack(int x, int y)
    {
        Color color = this.bitmap.GetPixel(x, y);
        return isBlack(color);
    }

    private bool isBlack(Color color)
    {
        return color.R == 0 && color.G == 0 && color.B == 0;
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

    private bool isNotBlack(int envX, int envY)
    {
        return !isBlack(envX, envY);
    }

    private bool isPixelExists(int envX, int envY)
    {
        return envX >= 0 && envY >= 0 && envX < imgWidth && envY < imgHeight;
    }

    ////////Zakończenia

    private void checkTestEndings(int x, int y)
    {
        checkEndings(x, y);
        if (isEnding())
        {
            if (filterEndings(x, y))
            {
                //markEndings(x, y);
                endings.Add(new Point(x, y));
                minutiaCounter++;
                return;
            }
        }
        //result.SetPixel(x, y, bitmap.GetPixel(x, y));
    }

    private bool isWhite(int x, int y)
    {
        Color color = this.bitmap.GetPixel(x, y);
        return color.R == 255 && color.G == 255 && color.B == 255;
    }

    private bool filterEndings(int x, int y)
    {
        int counter = 0,counter2 = 0;
        for(int i = x; i<imgWidth; i++){
            if(isWhite(i,y)){
                counter++;
                }
            }
        for(int i = x; i>=0; i--){
            if(isWhite(i,y)){
                counter2++;
                }

            }
        if (counter2 == x || counter == (imgWidth -x-1))
        {
            return false;
        }
        else return true;
    }
       

    private void markEndings(int x, int y)
    {
        int envX, envY;
        int fieldWidth = 9;
        int counterDir = 4;
        int counter = 0;
        markPixelIfExistsEndings(x, y, false);

        for (envX = x - counterDir, envY = y - counterDir; counter < fieldWidth; counter++, envY++)
        {
            markPixelIfExistsEndings(envX, envY, true);
        }

        counter = 0;

        for (envX = x + counterDir, envY = y - counterDir; counter < fieldWidth; counter++, envY++)
        {
            markPixelIfExistsEndings(envX, envY, true);
        }

        counter = 0;

        for (envX = x - counterDir + 1, envY = y - counterDir; counter < (fieldWidth - 2);
            counter++, envX++)
        {
            markPixelIfExistsEndings(envX, envY, true);
        }

        counter = 0;

        for (envX = x - counterDir + 1, envY = y + counterDir; counter < (fieldWidth - 2);
           counter++, envX++)
        {
            markPixelIfExistsEndings(envX, envY, true);
        }

        int counterX = 0, counterY = 0;
        for (envX = x - 3; counterX < 7; envX++, counterX++)
        {
            for (envY = y - 3, counterY = 0; counterY < 7; envY++, counterY++)
            {
                markPixelIfExistsEndings(envX, envY, false);
            }
        }
    }
    private void markPixelIfExistsEndings(int envX, int envY, Boolean setPixel)
    {
        if (isPixelExists(envX, envY))
        {
            isPixelSet[envX, envY] = true;
            if (setPixel)
            {
                result.SetPixel(envX, envY, Color.FromArgb(0, 0, 255));
            }
        }
    }

    private bool isEnding()
    {
        return blackPointCounter == 1;
    }

    private void checkEndings(int x, int y)
    {
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



}
