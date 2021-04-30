using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleGameFrameWork
{
    /// <summary>
    /// Represents a pixel on screen
    /// </summary>
    public struct Pixel
    {
        public Point Point { get; set; }
        public char Fill { get; set; }
        public ConsoleColor Color { get; set; }
        public ConsoleColor FontColor { get; set; }
        public int ZIndex { get; set; }
          
        public override bool Equals(object obj)
        {
            if (obj is Pixel comparePixel)
            {
                return Point == comparePixel.Point && 
                       Fill == comparePixel.Fill && 
                       Color == comparePixel.Color && 
                       comparePixel.FontColor == FontColor && 
                       ZIndex == comparePixel.ZIndex;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Point,Fill,Color);
        }
        public static Pixel From(Pixel pixel, int? x = null, int? y = null, char? fill = null, ConsoleColor? color = null, ConsoleColor? fontColor = null)
        {
            return new Pixel() 
            {
                Point = new Point(x ?? pixel.Point.X, y ?? pixel.Point.Y), 
                Color = color ?? pixel.Color, 
                Fill = fill ?? pixel.Fill, 
                FontColor= fontColor ?? pixel.FontColor , 
                ZIndex = pixel.ZIndex 
            };
        }
        public static Pixel Reset(int x,int y)
        {
            return new Pixel() { Color = Game.CurrentSettings.BackgroundColor, Fill = ' ', Point = new Point(x,y), ZIndex = 0 };
        }
        public static Pixel Reset(Point point)
        {
            return new Pixel() { Color = Game.CurrentSettings.BackgroundColor, Fill = ' ', Point = point, ZIndex = 0 };
        }
    }
}
