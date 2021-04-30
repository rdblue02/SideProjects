using System;

namespace ConsoleGameFrameWork.Events
{
    /// <summary>
    /// Event fired every time the user changes the console height or width.
    /// </summary>
    public class WindowResizedEvent:EventArgs
    {
        /// <summary>
        /// The new width of the console
        /// </summary>
        public int UpdatedWindowWidth { get; }
        /// <summary>
        /// The new height of the console
        /// </summary>
        public int UpdatedWindowHeight { get; }
        public WindowResizedEvent()
        {
            UpdatedWindowWidth = Console.WindowWidth;
            UpdatedWindowHeight = Console.WindowHeight;
        }
    }
}
