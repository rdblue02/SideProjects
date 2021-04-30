using ConsoleGameFrameWork.Input;
using System;
using System.Collections.Generic;

namespace ConsoleGameFrameWork.Events
{
    public class KeyPressEvent:EventArgs
    {
        public Queue<Keys> PressedKeys { get; }
        public KeyPressEvent(Queue<Keys> pressedKeys)
        {
            this.PressedKeys = pressedKeys;
        }
    }
}
