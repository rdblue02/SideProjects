using System;

namespace ConsoleGameFrameWork
{
    public struct Point { 
        public int X { get; }
        public int Y { get; }

        public Point(int x,int y)
        {
            this.X = x;
            this.Y = y;
        }
        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }
      
        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }
        public override bool Equals(object obj)
        {
            if(obj is Point other)
            {
                return this.X == other.X && this.Y == other.Y;
            }
            else
            {
                return false;
            }        
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(X,Y);
        }
    }
}