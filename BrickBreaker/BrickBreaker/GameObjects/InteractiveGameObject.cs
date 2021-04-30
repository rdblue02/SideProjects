using BrickBreaker.Events;
using ConsoleGameFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker.GameObjects
{
    public abstract class InteractiveGameObject:GameObject
    { 
        //control how fast the object moves.
        // we aren't taking delta into account at the moment.
        public TimeSpan TimeSinceLastMove { get; set; } = TimeSpan.FromMilliseconds(0);
        public TimeSpan MillisecondsBeforeMoveAllowed { get;} = TimeSpan.FromMilliseconds(1000);
        public float Speed { get; set; } //todo this logic is split between the child and parent at the moment. 
                                        // look into simpler way.
        public List<PowerUpEffect> PowerUpEffects { get; set; } = new List<PowerUpEffect>();
        protected bool AllowCollisionCheck { get; set; } = true;
        public void CheckForCollisions(GameTime gameTime)
        {
            //prevent us from processing a collision multiple times if the object hasn't moved yet.
            if (AllowCollisionCheck)
            {
                foreach (var otherObject in GameObjectCollection.GetActive<InteractiveGameObject>(x => x != this))
                {
                    var points = Sprite.DrawMap.Select(x => x.Point).ToList();
                    var otherPoints = otherObject.Sprite.DrawMap.Select(x => x.Point).ToList();
                    var isCollision = points.Any(point => otherPoints.Any(otherPoint => otherPoint.X == point.X && otherPoint.Y == point.Y));

                    if (isCollision)
                    {
                        otherObject.AllowCollisionCheck = false;
                        otherObject.ProcessCollisions(this,gameTime);
                        ProcessCollisions(otherObject, gameTime);
                    }
                }
                AllowCollisionCheck = false;
            }
                 
        }
        public abstract void ProcessCollisions(InteractiveGameObject CollidingObject,GameTime gameTime);      
    }
    public struct PowerUpEffect
    {
        public string Name { get; set; }
        public PowerUpEffectAction PowerUpEffectAction { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan ActivationTime { get; set; }
        public bool Processed { get; set; }
        public int MinLevelRequired { get; set; }

        public static PowerUpEffect From(PowerUpEffect power,string name,TimeSpan? duration=null,TimeSpan? activationTime=null, bool? processed=null)
        {
            return new PowerUpEffect()
            {
             Duration = duration ?? power.Duration,
             ActivationTime = activationTime ?? power.ActivationTime,
             PowerUpEffectAction = power.PowerUpEffectAction,
             Processed = processed ?? power.Processed,
             Name = name
            };
        }
        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
