using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFrameWork
{
    /// <summary>
    /// Wrapper class for System.Random
    /// </summary>
     public class Randomizer
    {
        private Random random = new Random((int)DateTime.Now.Ticks);
        public TimeSpan? RefreshRate { get; set; }
        public TimeSpan ActivationTime { get; set; }
        
        /// <summary>
        /// Checks if we should update the current Random object with a new seed.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (RefreshRate != null)
            {
                if (gameTime.TotalElapsedTime - ActivationTime > RefreshRate)
                {
                    Refresh(gameTime);
                }
            }      
        }
        /// <summary>
        /// Has a chance to return true based off the passed in percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public bool ByPercentage(int percent)
        {
           return random.Next(0, 100) < percent;
        }
        /// <summary>
        /// returns a random number inbetween from and to 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int RandomNumber(int from, int to)
        {          
            if (from == to)
            {
                return from;
            }
            //we don't care which direction the user is expecting
            //from and to. We only care about the numbers inbetween.
            if (from > to)
            {
                var temp = to;
                to = from;
                from = temp;
            }         

            return random.Next(from, to);
        }
        /// <summary>
        /// Update the current Random object with a new random object from a different seed.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Refresh(GameTime gameTime)
        {
            this.ActivationTime = gameTime.TotalElapsedTime;
            random = new Random((int)DateTime.Now.Ticks);
        }
    }
}
