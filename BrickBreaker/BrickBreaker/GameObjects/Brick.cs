using BrickBreaker.Events;
using ConsoleGameFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker.GameObjects
{
    public class Brick : InteractiveGameObject
    {
        public override bool IsActive { get; set; }
        public int BrickLevel { get; set; }
        public event EventHandler<GameEndEvent> OnGameEnd;

        public override void Update(GameTime gameTime)
        {
            if (BrickLevel <= 0)
            {
                this.IsActive = false;
                if (GameObjectCollection.CountActive<Brick>() == 0)
                {
                    this.OnGameEnd?.Invoke(this, new GameEndEvent(GameEndReason.AllBricksBroken));
                }
            }
        }
        public override void Draw(GameTime gameTime)
        {         
           DrawEngine.QueueSpriteForDraw(this.Sprite);       
        }

        public override void ProcessCollisions(InteractiveGameObject CollidingObject,GameTime gameTime)
        {
            if(CollidingObject is Bullet )
            {
                BrickLevel--;
                CollisionLogic();
                return;
            }
            if(CollidingObject is Ball ball)
            {
                 //multi break should destroy the brick regardless of level
                if (ball.MultiBrickBreak)
                {
                    for (int i = BrickLevel;i>0;i--)
                    {
                        CurrentGame.Score += 10;
                    }
                    BrickLevel = 0; 

                }
                else
                {
                    CurrentGame.Score += 10;
                    BrickLevel--;                             
                }
                CollisionLogic();
            }
        }
        private void CollisionLogic()
        {
            if (BrickLevel > 0)
            {
                //update the break color.
                for (int i = 0; i < Sprite.DrawMap.Count; i++)
                {
                    Sprite.DrawMap[i] = Pixel.From(Sprite.DrawMap[i], color: BrickColorMap[BrickLevel]);
                }
            }
            //todo we should assign each power up its own chance to happen.
            //at the moment evey power up has the same chance to fire.
            var createPowerUp = CurrentGame.Randomizer.ByPercentage(25);
            if (createPowerUp)
            {              
                var powerUpX = this.Sprite.Left.Point.X + this.Sprite.Width / 2;
                var powerUpY = this.Sprite.Bottom.Point.Y;
              
                 var power = CurrentGame.GameObjectInitializer.GetRandomPowerUp(powerUpX,powerUpY);
                GameObjectCollection.Inject(power);
            }
        }
        public static Dictionary<int, ConsoleColor> BrickColorMap = new Dictionary<int, ConsoleColor>()
        {
            { 1,ConsoleColor.White},
            { 2,ConsoleColor.Blue},
            { 3,ConsoleColor.DarkBlue},
            { 4,ConsoleColor.DarkMagenta},
            { 5,ConsoleColor.DarkGray},
        };
    }
}
