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
    private List<Point[]> minutias;
    private bool[,] isPixelSet;
    private int minutiaCounter = 0;

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
                        blackPointCounter = 0;
                        checkTestEndings(x, y);
                    }
                }
                
            }
        }
        Console.WriteLine("MinutiaCounter: " + minutiaCounter);
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
                minutiaCounter++;
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

    ////////Zakończenia

    private void checkTestEndings(int x, int y)
    {
        checkEndings(x, y);
        if (isEnding())
        {
            if (filterEndings(x, y))
            {
                markEndings(x, y);
                minutiaCounter++;
                return;
            }
        }
        result.SetPixel(x, y, bitmap.GetPixel(x, y));
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

        //int counterX = 0, counterY = 0;
        //for (envX = x - 3; counterX < 7; envX++, counterX++)
        //{
        //    for (envY = y - 3, counterY = 0; counterY < 7; envY++, counterY++)
        //    {
        //        markPixelIfExistsEndings(envX, envY, false);
        //    }
        //}
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
