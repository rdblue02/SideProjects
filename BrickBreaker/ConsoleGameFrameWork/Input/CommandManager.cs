using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleGameFrameWork.Input
{ 
   public static class CommandManager
    { 
        [DllImport("user32.dll")]
        static extern int GetKeyState(int key);
        static Dictionary<int, bool> currentKeyBoardState;
        static Dictionary<int, bool> previousKeyBoardState;

        static CommandManager()
        {
            currentKeyBoardState = new Dictionary<int, bool>();
            previousKeyBoardState = new Dictionary<int, bool>();

            foreach (var key in Helpers.EnumValuesAsList<Keys>())
            {
                if (!currentKeyBoardState.ContainsKey((int)key))
                {
                    currentKeyBoardState.Add((int)key, false);
                    previousKeyBoardState.Add((int)key, false);
                }
            }

        }
        public static void GetInput()
        {
            foreach (var key in currentKeyBoardState.Keys)
            {
                previousKeyBoardState[key] = currentKeyBoardState[key];

                // Not sure I'm using this right. It works though.
                currentKeyBoardState[key] = GetKeyState(key) > 1;
            }
        }
        /// <summary>
        /// returns true if the requested key is being pessed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyDown(Keys key)
        {
            return currentKeyBoardState[(int)key];
        }
        /// <summary>
        /// returns true if the requested key was down and is being released.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyUp(Keys key)
        {
            return previousKeyBoardState[(int)key] && !currentKeyBoardState[(int)key];
        }
        /// <summary>
        /// returns a list of all keys currently being pressed.
        /// </summary>
        /// <returns></returns>
        public static List<Keys> GetPressedKeys()
        {
            return currentKeyBoardState.Where(x => x.Value).Select(x => (Keys)x.Key).ToList();
        }


    }
}
