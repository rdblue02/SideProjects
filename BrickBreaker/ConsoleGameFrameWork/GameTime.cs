using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ConsoleGameFrameWork
{
    /// <summary>
    /// Track information regarding passed time.
    /// </summary>
    public struct GameTime
    {
        public TimeSpan DeltaTime{ get; set; }        
        public TimeSpan PassedFrameTime { get; set; }
        public TimeSpan TotalElapsedTime { get; set; }
        public int CurrentFrame { get; set; }
        public int TargetFPS { get; set; }
    }
}
