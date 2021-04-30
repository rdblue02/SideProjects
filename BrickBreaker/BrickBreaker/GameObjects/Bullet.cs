using ConsoleGameFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker.GameObjects
{
    public class Bullet : InteractiveGameObject
    {
        public override bool IsActive { get; set; }

        public override void Draw(GameTime gameTime)
        {
            DrawEngine.QueueSpriteForDraw(this.Sprite);
        }
        public override void ProcessCollisions(InteractiveGameObject CollidingObject, GameTime gameTime)
        {
           if( CollidingObject is Brick)
           {
                this.IsActive = false;
           }
        }

        public override void Update(GameTime gameTime)
        {
            if(this.Sprite.Top.Point.Y <= 0)
            {
                IsActive = false;
            }
            else
            {
                this.Sprite.Move(Direction.Up);
                this.AllowCollisionCheck = true;
            }
        }
    }
}
