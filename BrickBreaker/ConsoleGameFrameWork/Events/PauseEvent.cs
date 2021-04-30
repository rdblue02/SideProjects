using System;


namespace ConsoleGameFrameWork.Events
{ 
    public class PauseEvent:EventArgs
    {
        public bool Paused { get; }

        public PauseEvent(bool isPaused)
        {
            this.Paused = isPaused;
        }
    }
}
