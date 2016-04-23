using ProjektBjometria;
using System;
using System.Collections.Generic;
using System.Drawing;

public class MinutiaFinder
{
    private Bitmap bitmap;
    public Bitmap result { get; }
    private CustomBitmapProcessing processing;
    private int imgWidth;
    private int imgHeight;
    private int blackPointCounter;
    private List<Point[]> minutias;
    private bool[,] isPixelSet;

    public MinutiaFinder(Bitmap bitmap)
    {
        this.bitmap = bitmap;
        this.processing = new CustomBitmapProcessing(bitmap);
        this.imgHeight = bitmap.Height;
        this.imgWidth = bitmap.Width;
        this.isPixelSet = new bool[imgWidth, imgHeight];
        this.minutias = new List<Point[]>();
        this.result = new Bitmap(imgWidth, imgHeight);
    }

    public void findCrosscuts()
    {
        for (int x = 0; x < imgWidth; x++)
        {
            for (int y = 0; y < imgHeight; y++)
            {
                blackPointCounter = 0;
                if (!isPixelSet[x, y])
                {
                    if (isBlack(x, y))
                    {
                        checkTestFields(x, y);
                    }
                }
                
            }
        }
    }

    private void checkTestFields(int x, int y)
    {
        checkTestField(x, y, 5);
        if (isCrosscut())
        {
            checkTestField(x, y, 9);

            if (isCrosscut())
            {
                markMinutia(x, y);
                return;
            }
        }
        
        result.SetPixel(x, y, bitmap.GetPixel(x, y));
        
    }

    private void markMinutia(int x, int y)
    {
        int envX, envY;
        int fieldWidth = 9;
        int counterDir = 4;
        int counter = 0;
        markPixelIfExists(x, y, x, y);

        for (envX = x - counterDir, envY = y - counterDir; counter < fieldWidth; counter++, envY++)
        {
            markPixelIfExists(x, y, envX, envY);
        }

        counter = 0;

        for (envX = x + counterDir, envY = y - counterDir; counter < fieldWidth; counter++, envY++)
        {
            markPixelIfExists(x, y, envX, envY);
        }

        counter = 0;

        for (envX = x - counterDir + 1, envY = y - counterDir; counter < (fieldWidth - 2);
            counter++, envX++)
        {
            markPixelIfExists(x, y, envX, envY);
        }

        counter = 0;

        for (envX = x - counterDir + 1, envY = y + counterDir; counter < (fieldWidth - 2);
           counter++, envX++)
        {
            markPixelIfExists(x, y, envX, envY);
        }
    }

    private void markPixelIfExists(int x, int y, int envX, int envY)
    {
        if (isPixelExists(envX, envY))
        {
            result.SetPixel(envX, envY, Color.FromArgb(255, 0, 0));
            isPixelSet[envX, envY] = true;
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

        for (envX = x - counterDir, envY = y - counterDir; counter < fieldWidth; counter++, envY++)
        {
            incrementCounterIfPixelIsCorrect(envX, envY);
        }

        counter = 0;

        for (envX = x + counterDir, envY = y - counterDir; counter < fieldWidth; counter++, envY++)
        {
            incrementCounterIfPixelIsCorrect(envX, envY);
        }

        counter = 0;

        for (envX = x - counterDir + 1, envY = y - counterDir; counter < (fieldWidth - 2);
            counter++, envX++)
        {
            incrementCounterIfPixelIsCorrect(envX, envY);
        }

        counter = 0;

        for (envX = x - counterDir + 1, envY = y + counterDir; counter < (fieldWidth - 2);
           counter++, envX++)
        {
            incrementCounterIfPixelIsCorrect(envX, envY);
        }
    }

    private bool isBlack(int x, int y)
    {
        Color color = this.bitmap.GetPixel(x, y);
        return color.R == 0 && color.G == 0 && color.B == 0 ;
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

    private bool isPixelExists(int envX, int envY)
    {
        return envX >= 0 && envY >= 0 && envX < imgWidth && envY < imgHeight;
    }
}
