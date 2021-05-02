using ConsoleGameFrameWork.Input;
using System;
using System.Collections.Generic;
using System.Text;
namespace ConsoleGameFrameWork
{
   public class Settings
    {
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }       
        public ConsoleColor BackgroundColor { get; set; }  
        public bool AllowResize { get; set; }
        public bool CurserVisble { get; set; }
        public bool DisplayFps { get; set; }
        public Keys PauseKey { get; set; }
        public bool SettingsAppliedSuccessfully { get; private set; }

        public void Apply()
        {
            try
            {
                Console.BackgroundColor = BackgroundColor;
                Console.ForegroundColor = BackgroundColor;
                Console.CursorVisible = CurserVisble;
                Console.Clear();

                if (WindowWidth > Console.BufferWidth)
                {
                    Console.BufferWidth = WindowWidth;
                    Console.WindowWidth = WindowWidth;
                }
                else
                {
                    Console.WindowWidth = WindowWidth;
                    Console.BufferWidth = WindowWidth;
                }

                if (WindowHeight > Console.BufferHeight)
                {
                    Console.BufferHeight = WindowHeight;
                    Console.WindowHeight = WindowHeight;
                }
                else
                {
                    Console.WindowHeight = WindowHeight;
                    Console.BufferHeight = WindowHeight;
                }
                this.SettingsAppliedSuccessfully = true;
            }
            //Todo we should decide which errors are okay.
            //Put a limit on how many times we try to apply.
            catch(Exception e)
            {
                Console.Write(e.Message);
            }           
        }
       
        public static Settings GetDefaultSettings()
        {
            return new Settings()
            {
                WindowHeight = 25,
                WindowWidth = 93,
                BackgroundColor = ConsoleColor.Cyan,
                AllowResize = false,
                DisplayFps = false,
                CurserVisble = false,
                PauseKey = Keys.Enter
            };        
        }      
    }
}
