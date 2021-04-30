using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ConsoleGameFrameWork
{
    public static class Helpers
    {

        /// <summary>
        /// Only used for testing. Using this method will write to the console outside of our Draw Engine.
        /// Output is not cleared by the standard loop
        /// </summary>
        /// <param name="val"></param>
        /// <param name="point"></param>
        public static void WriteAt(object val, Point point)
        {
            
            Console.ForegroundColor = Game.CurrentSettings.BackgroundColor == ConsoleColor.Black ? ConsoleColor.White : ConsoleColor.Black;
            Console.SetCursorPosition(point.X, point.Y);
            Console.Write(val);
            Console.ForegroundColor = Game.CurrentSettings.BackgroundColor;
        }
 
        public static List<Pixel> ToText(this string s, int startX, int startY, ConsoleColor fontColor)
        {
            var pixels = new List<Pixel>();

            for(int i = 0; i < s.Length; i++)
            {
                var point = new Point(startX+i,startY);
                pixels.Add(new Pixel() { Color= Game.CurrentSettings.BackgroundColor, Fill = s[i], FontColor= fontColor, Point = point , ZIndex=999});
            }
            return pixels;
        }
        public static Sprite ToMessageInfoSprite(string message, ConsoleColor fontColor)
        {
            var sprite = new Sprite();
            sprite.DrawMap = new List<Pixel>();
            if(string.IsNullOrEmpty(message))
            {
                return null;
            }

            int alignX = Game.CurrentSettings.WindowWidth / 2 - message.Length / 2;
            int alignY = Game.CurrentSettings.WindowHeight / 2;

            for (int charIndex = 0; charIndex < message.Length; charIndex++)
            {
                alignX = Game.CurrentSettings.WindowWidth / 2 - message.Length / 2;
                var point = new Point(alignX + charIndex, alignY);
                sprite.DrawMap.Add(new Pixel() { Color = Game.CurrentSettings.BackgroundColor, Fill = message[charIndex], FontColor = fontColor, Point = point, ZIndex = 999 });
            }
            return sprite;
        }

        public static List<T> EnumValuesAsList<T>() where T:Enum
        {
            return  Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
