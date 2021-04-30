using BrickBreaker.Events;
using ConsoleGameFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker.GameObjects
{
    public class PowerUp<T> : InteractiveGameObject where T: InteractiveGameObject
    {
        public override bool IsActive { get; set; }
        public TimeSpan PowerUpDuration { get; set; }
        public ConsoleColor EffectColor { get; }
    
       //todo look into allowing different colors for different types. 
        static readonly Dictionary<Type, ConsoleColor> PowerUpColorMap = new Dictionary<Type, ConsoleColor>() {
            {typeof(Slider),ConsoleColor.DarkGreen},
            {typeof(Ball),ConsoleColor.DarkYellow },
            {typeof(Brick),ConsoleColor.DarkGray},
            {typeof(InteractiveGameObject), ConsoleColor.DarkMagenta },
        };
       
        public PowerUp()
        {
            this.EffectColor = PowerUpColorMap[typeof(T)]; 
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalElapsedTime - this.TimeSinceLastMove > this.MillisecondsBeforeMoveAllowed / Speed)//todo a better way to calculate speed. 0 would throw an exception
            {
                this.Sprite.Move(Direction.Down);
                this.TimeSinceLastMove = gameTime.TotalElapsedTime;
                this.AllowCollisionCheck = true;
            }
           
            if (this.Sprite.Bottom.Point.Y >= CurrentGame.CurrentSettings.WindowHeight - 2)
            {
                this.IsActive = false;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            DrawEngine.QueueSpriteForDraw(Sprite);
        }
          
        public override void ProcessCollisions(InteractiveGameObject CollidingObject,GameTime gameTime)
        {
            if(CollidingObject is Slider)
            {
                //Handover whatever power up effect we have to the objects impacted by it.
                var effectedObjects = GameObjectCollection.GetActive<T>();
                foreach(var obj in effectedObjects)
                {
                    foreach(var effect in this.PowerUpEffects)
                    {
                        if (obj.PowerUpEffects.All(x=>x.PowerUpEffectAction != effect.PowerUpEffectAction))
                        {
                            obj.PowerUpEffects.Add(effect);
                            CurrentGame.Score += 25;
                        }

                    }
                }
                 
                this.IsActive = false;
            }
        } 
    }
}
