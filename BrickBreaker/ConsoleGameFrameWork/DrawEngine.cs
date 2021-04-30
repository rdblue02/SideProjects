using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ConsoleGameFrameWork
{
    public static class DrawEngine
    {
        static Dictionary<Point, Pixel> screenBuffer; 
        static Dictionary<Point, Pixel> screen;  
        public static void InitializeScreen()
        {
            screenBuffer = new Dictionary<Point, Pixel>();
            screen = new Dictionary<Point, Pixel>();

            var screenWidth = Game.CurrentSettings.WindowWidth;
            var screenHeight = Game.CurrentSettings.WindowHeight;

            for(int x=0;x<screenWidth;x++)
            {
                for (int y = 0; y < screenHeight; y++)
                {
                    var pixel = Pixel.Reset(x, y);
                    screen.Add(pixel.Point,pixel);
                    screenBuffer.Add(pixel.Point, pixel);
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
                if(pixel.ZIndex > screenBuffer[pixel.Point].ZIndex)
                {
                    screenBuffer[pixel.Point] = pixel; 
                }
            }
        }
       
       public static void DrawScreen()
       {     
            foreach(var point in screenBuffer.Keys)
            {
                //Dont bother drawing pixels that haven't changed.
                if (!screenBuffer[point].Equals(screen[point]))
                {
                    //No buffer exists to keep the scroll bar from displaying.
                    //Make sure we don't throw an out of range exception.
                    if(point.X<Console.BufferWidth  && 
                       point.X > -1 &&
                       point.Y<Console.BufferHeight  && 
                       point.Y>-1
                       )
                    {
                        //Swallow the IO exception that happens when a user resizes the screen at the 
                        //same time we are writing.
                        try
                        {
                            Console.SetCursorPosition(point.X, point.Y);
                            Console.BackgroundColor = screenBuffer[point].Color;
                            Console.ForegroundColor = screenBuffer[point].FontColor;
                            Console.Write(screenBuffer[point].Fill);
                            screen[point] = screenBuffer[point];
                        }
                        finally 
                        {

                        }
                       
                    }
                               
                }
               
            }
            Console.BackgroundColor = Game.CurrentSettings.BackgroundColor;
            Console.ForegroundColor = Game.CurrentSettings.BackgroundColor;
            FlushScreenBuffer();
        }

        static void FlushScreenBuffer()
        {
            foreach (var point in screenBuffer.Keys)
            {
                screenBuffer[point] = Pixel.Reset(point);
            }
        }
    }
}
