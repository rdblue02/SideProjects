using BrickBreaker.Events;
using ConsoleGameFrameWork;
using System;
using System.Collections.Generic;


namespace BrickBreaker.GameObjects
{
    public class Ball : InteractiveGameObject
    {
        public event EventHandler<GameEndEvent> OnGameEnd;
        public override bool IsActive { get; set; }        
        public bool IsMainBall { get; set; } //used to tell the difference between the real ball on ball split
        public Direction CurrentDirection { get; set; }
        public bool MultiBrickBreak { get; private set; } = false;
        public override void Update(GameTime gameTime)
        {
            HandlePowerUps(gameTime);
            if (gameTime.TotalElapsedTime-this.TimeSinceLastMove > this.MillisecondsBeforeMoveAllowed/Speed)//todo a better way to calculate speed. 0 would throw an exception
            {
                if(this.Sprite.Top.Point.Y == 0)
                {
                    if(this.CurrentDirection == Direction.Up)
                    {
                        this.CurrentDirection = Direction.Down;
                    }
                    else if(this.CurrentDirection == Direction.UpRight)
                    {
                        this.CurrentDirection = Direction.DownRight;
                    }
                    else
                    {
                        this.CurrentDirection = Direction.DownLeft;
                    }                    
                }

                if(this.Sprite.Right.Point.X == Game.CurrentSettings.WindowWidth-1 || this.Sprite.Left.Point.X ==1)
                {
                    switch (this.CurrentDirection) 
                    {
                        case  Direction.DownRight:
                            this.CurrentDirection = Direction.DownLeft;
                            break;
                        case Direction.UpRight:
                            this.CurrentDirection = Direction.UpLeft;
                            break;
                        case Direction.DownLeft:
                            this.CurrentDirection = Direction.DownRight;
                            break;
                        case Direction.UpLeft:
                            this.CurrentDirection = Direction.UpRight;
                            break;
                    }                   
                }

                if(this.Sprite.Bottom.Point.Y >= Game.CurrentSettings.WindowHeight-1)
                {
                    if (this.IsMainBall)
                    {
                        OnGameEnd.Invoke(this, new GameEndEvent(GameEndReason.GameOver));
                        this.IsActive = false;
                    }
                    else
                    {
                        this.IsActive = false;
                    }
                }

                this.Sprite.Move(this.CurrentDirection);
                this.AllowCollisionCheck = true;
                this.TimeSinceLastMove = gameTime.TotalElapsedTime;
            }        
        }

        public override void Draw(GameTime gameTime)
        {
            DrawEngine.QueueSpriteForDraw(this.Sprite);
        }

        private void HandlePowerUps(GameTime gameTime)
        {
            for (int i = 0; i < PowerUpEffects.Count; i++)
            {
                if (!PowerUpEffects[i].Processed && IsMainBall)
                {  
                    ProcessPower(PowerUpEffects[i]);
                    PowerUpEffects[i] = PowerUpEffect.From(PowerUpEffects[i], PowerUpEffects[i].Name, null, gameTime.TotalElapsedTime,true);
                }
                else
                {
                    if (gameTime.TotalElapsedTime - PowerUpEffects[i].ActivationTime > PowerUpEffects[i].Duration)
                    {
                        UndoPower(PowerUpEffects[i]);
                    }
                }
            }
        }
        private void ProcessPower(PowerUpEffect power)
        {
            switch (power.PowerUpEffectAction)
            {
                case PowerUpEffectAction.Grow:
                    Grow();
                    break;
                case PowerUpEffectAction.Ball_Split: 
                    Split();
                    break;
                case PowerUpEffectAction.DecreaseSpeed:
                    Speed /= 2;
                    break;
                case PowerUpEffectAction.IncreaseSpeed:
                    Speed *= 2;
                    break;
                case PowerUpEffectAction.Ball_MultiBrickBreak:
                    MultiBrickBreak = true;
                    break;
            }
            GameObjectCollection.ActivePowerUps.Add(power.ToString());
        }
        private void UndoPower(PowerUpEffect power)
        {
            switch (power.PowerUpEffectAction)
            {
                case PowerUpEffectAction.Grow:
                    ResetSize();
                    break;
                case PowerUpEffectAction.DecreaseSpeed:
                    Speed *= 2;
                    break;
                case PowerUpEffectAction.IncreaseSpeed:
                    Speed /= 2;
                    break;
                case PowerUpEffectAction.Ball_Split:
                    GameObjectCollection.GetActive<Ball>(x => !x.IsMainBall).ForEach(x => x.IsActive = false);
                    break;
                case PowerUpEffectAction.Ball_MultiBrickBreak:
                    MultiBrickBreak = false;
                    
                    break;
            }
            this.PowerUpEffects.Remove(power);
            GameObjectCollection.ActivePowerUps.Remove(power.ToString());
        }
        private void Grow()
        {
            var y = this.Sprite.Top.Point.Y;
            var growRight = this.Sprite.Right.Point.X + 2 < Game.CurrentSettings.WindowWidth - 1;
            var x =  growRight ? this.Sprite.Right.Point.X : this.Sprite.Left.Point.X;
           
            if(growRight)
            {
                this.Sprite.DrawMap.Add(Pixel.From(this.Sprite.Top,x + 1, y));
                this.Sprite.DrawMap.Add(Pixel.From(this.Sprite.Top,x + 2, y));
            }
            else
            {
                this.Sprite.DrawMap.Add(Pixel.From(this.Sprite.Top,x - 1, y));
                this.Sprite.DrawMap.Add(Pixel.From(this.Sprite.Top,x - 2, y));
            }
           

            y = this.Sprite.Bottom.Point.Y +1;
            x = this.Sprite.Left.Point.X;
            var width = this.Sprite.Width;
         
            for (int i = 0; i < width; i++)
            {
                this.Sprite.DrawMap.Add(Pixel.From(this.Sprite.Left, x + i, y ));
            }
        }
        private void ResetSize()
        {
            var x = Sprite.Left.Point.X;
            var y = Sprite.Left.Point.Y;
            Sprite.DrawMap.Clear();

            Sprite.DrawMap.Add(new Pixel() { Point = new Point(x, y), Color = ConsoleColor.Yellow, Fill = ' ', ZIndex = 2 });
            Sprite.DrawMap.Add(new Pixel() { Point = new Point(x + 1,y), Color = ConsoleColor.Yellow, Fill = ' ', ZIndex = 2 });
        }
    
        private void Split()
        {
            //todo use the ObjectInitializer to create these instead. 
            var miniBallOne = new Ball() { 
             IsActive = true,
             IsMainBall = false,
             Speed = this.Speed,
             Sprite = new Sprite() { DrawMap = new List<Pixel>() },
             CurrentDirection = this.CurrentDirection != Direction.UpRight ? Direction.UpRight : Direction.DownRight             
            };

            var miniBallTwo = new Ball()
            {
                IsActive = true,
                IsMainBall = false,
                Speed = this.Speed,
                Sprite =new Sprite() { DrawMap = new List<Pixel>() },
                CurrentDirection = this.CurrentDirection != Direction.UpLeft ? Direction.UpLeft : Direction.DownLeft
            };
            foreach (var pixel in this.Sprite.DrawMap)
            {
                miniBallOne.Sprite.DrawMap.Add(Pixel.From(pixel,color:ConsoleColor.Red));
                miniBallTwo.Sprite.DrawMap.Add(Pixel.From(pixel, color: ConsoleColor.Red));
            }
            GameObjectCollection.Inject(miniBallOne);
            GameObjectCollection.Inject(miniBallTwo);
        }
       
        public override void ProcessCollisions(InteractiveGameObject CollidingObject,GameTime gameTime)
        {
            if (CollidingObject is Slider slider)
            {
                var sliderPartLength = slider.Sprite.Width / 3;
                var ballLeft = this.Sprite.Left.Point.X;
                var ballRight = this.Sprite.Right.Point.X;
                var sliderLeft = slider.Sprite.Left.Point.X;
                var sliderRight = slider.Sprite.Right.Point.X;

                if (ballRight >= sliderLeft && ballLeft < sliderLeft + sliderPartLength)
                {
                    this.CurrentDirection = Direction.UpLeft;
                }
                else if (ballLeft <=sliderRight && ballRight> sliderRight - sliderPartLength)
                {
                    this.CurrentDirection = Direction.UpRight;
                }
                else
                {
                    this.CurrentDirection = Direction.Up;
                }
            }
            if (CollidingObject is Brick && !MultiBrickBreak)
            {
                switch (this.CurrentDirection)
                {
                    case Direction.Up:
                        this.CurrentDirection = Direction.Down;
                        break;
                    case Direction.UpLeft:
                        this.CurrentDirection = Direction.DownLeft;
                        break;
                    case Direction.UpRight:
                        this.CurrentDirection = Direction.DownRight;
                        break;
                    case Direction.Down:
                        this.CurrentDirection = Direction.Up;
                        break;
                    case Direction.DownRight:
                        this.CurrentDirection = Direction.UpRight;
                        break;
                    case Direction.DownLeft:
                        this.CurrentDirection = Direction.UpLeft;
                        break;
                }
            }
        }
    }
}
