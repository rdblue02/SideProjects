using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ConsoleGameFrameWork
{
    public static class DrawEngine
    {
        static Dictionary<Point, int> pointIndexLookUp;
        static List<Pixel> screenBuffer; 
        static List<Pixel> screen;  
        public static void InitializeScreen()
        {
            screenBuffer = new List<Pixel>();
            screen = new List<Pixel>();
            pointIndexLookUp = new Dictionary<Point, int>();

            var screenWidth = Game.CurrentSettings.WindowWidth;
            var screenHeight = Game.CurrentSettings.WindowHeight;
            var index = 0;

            for(int x=0;x<screenWidth;x++)
            {
                for (int y = 0; y < screenHeight; y++)
                {
                    var pixel = Pixel.Reset(x, y);
                    pointIndexLookUp.Add(pixel.Point, index);
                    screen.Add(pixel);
                    screenBuffer.Add(pixel);
                    index++;
                }
            }
        }
        /// <summary>
        /// Queue up a sprite to be dawn on the screen. 
        /// </summary>
        /// <param name="sprite"></param>
        public static void QueueSpriteForDraw(Sprite sprite)
        {
            foreach(var pixel in sprite.DrawMap)
            {
                //If two sprites overlap, we use the ZIndex to decide which pixel to draw.
                if (pixel.ZIndex > screenBuffer[pointIndexLookUp[pixel.Point]].ZIndex)
                {
                    screenBuffer[pointIndexLookUp[pixel.Point]] = pixel; 
                }
            }
        }
       
       public static void DrawScreen()
       {    
            for(int i = 0; i < screenBuffer.Count; i++)
            {
                //Dont bother drawing pixels that haven't changed.
                if (!screenBuffer[i].Equals(screen[i]))
                {
                    //No buffer exists to keep the scroll bar from displaying.
                    //Make sure we don't throw an out of range exception.
                    if (screenBuffer[i].Point.X < Console.BufferWidth &&
                       screenBuffer[i].Point.X > -1 &&
                       screenBuffer[i].Point.Y < Console.BufferHeight &&
                       screenBuffer[i].Point.Y > -1
                       )
                    {
                        //Swallow the IO exception that happens when a user resizes the screen at the 
                        //same time we are writing.
                        try
                        {
                            Console.SetCursorPosition(screenBuffer[i].Point.X, screenBuffer[i].Point.Y);
                            Console.BackgroundColor = screenBuffer[i].Color;
                            Console.ForegroundColor = screenBuffer[i].FontColor;
                            Console.Write(screenBuffer[i].Fill);
                            screen[i] = screenBuffer[i];
                        }
                        finally
                        {

                        }

                    }
                }
            }
            FlushScreenBuffer();
        }

        static void FlushScreenBuffer()
        {
            for(int i = 0; i < screenBuffer.Count; i++)
            {
                screenBuffer[i]= Pixel.Reset(screenBuffer[i].Point);
            }     
        }
    }
}
