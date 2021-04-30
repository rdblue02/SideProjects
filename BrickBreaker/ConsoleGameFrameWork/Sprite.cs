using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ConsoleGameFrameWork
{
    public class Sprite
    {
        public Guid Id { get; } = Guid.NewGuid();
        public List<Pixel> DrawMap { get; set; }    
        public Pixel Top => DrawMap.OrderBy(x => x.Point.Y).FirstOrDefault();
        public Pixel Bottom => DrawMap.OrderBy(x => x.Point.Y).LastOrDefault();
        public Pixel Left => DrawMap.OrderBy(x => x.Point.X).FirstOrDefault();
        public Pixel Right => DrawMap.OrderBy(x => x.Point.X).LastOrDefault();
        public int Width => Right.Point.X - Left.Point.X + 1;
        public int Height => Bottom.Point.Y - Top.Point.Y + 1;
      
        /// <summary>
        /// Offsets all pixels in the DrawMap in the requested direction. 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public void Move(Direction direction, int pixels =1)
        {
            //this seems like an awful way to do this. Better than the giant switch statement from before.
            //Todo look into a faster/cleaner way.
            int yDelta = 0;
            int xDelta = 0;
          
            if((Left.Point.X - pixels > -1) 
                && direction == Direction.Left 
                || direction == Direction.UpLeft
                || direction == Direction.DownLeft)
            {
                xDelta = -1;
                if( direction == Direction.UpLeft && Top.Point.Y - pixels > -1)
                {
                    yDelta = -1;
                }
                if(direction == Direction.DownLeft && Bottom.Point.Y + pixels < Game.CurrentSettings.WindowHeight)
                {
                    yDelta = 1;
                }
            }
            if ((Right.Point.X + pixels < Game.CurrentSettings.WindowWidth)
                && direction == Direction.Right
                || direction == Direction.DownRight
                || direction == Direction.UpRight)
            {
                xDelta = 1;
                if (direction == Direction.UpRight && Top.Point.Y - pixels > -1)
                {
                    yDelta = -1;
                }
                if (direction == Direction.DownRight && Bottom.Point.Y + pixels < Game.CurrentSettings.WindowHeight)
                {
                    yDelta = 1;
                }
            }

            if(direction == Direction.Down && Bottom.Point.Y + pixels < Game.CurrentSettings.WindowHeight)
            {
                yDelta = 1;
            }
            if(direction == Direction.Up && Top.Point.Y - pixels > -1)
            {
                yDelta = -1;
            }
            if(xDelta !=0 || yDelta != 0)
            {
                for (int i = 0; i < DrawMap.Count; i++)
                {
                    DrawMap[i] = new Pixel() { Point = new Point(DrawMap[i].Point.X + pixels * xDelta, DrawMap[i].Point.Y + pixels * yDelta), Color = DrawMap[i].Color, Fill = DrawMap[i].Fill, ZIndex = DrawMap[i].ZIndex };
                }
            }
        }
 
        public override bool Equals(object obj)
        {
            if(obj is Sprite s)
            {
                return this.Id == s.Id;
            }
            else
            {
                return false;
            }
        }
     
        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
