using ConsoleGameFrameWork;
using ConsoleGameFrameWork.Input;
using System;


namespace BrickBreaker.GameObjects
{
    public class Slider : InteractiveGameObject
    {
        public override bool IsActive { get; set; }     
        bool hasGuns = false;
        bool hasMegaGuns = false;
        TimeSpan fireRate;
        TimeSpan lastTimeFired;
 
        public override void Update(GameTime gameTime)
        { 
            HandlePowerUps(gameTime);

            if (CommandManager.IsKeyDown(Keys.Left))
            {
                this.Sprite.Move(Direction.Left, (int)Speed);
            }
            if(CommandManager.IsKeyDown(Keys.Right))
            {
                this.Sprite.Move(Direction.Right, (int)Speed);
            }
           if((hasGuns || hasMegaGuns) 
                && gameTime.TotalElapsedTime - lastTimeFired > fireRate 
                && CommandManager.IsKeyDown(Keys.F))
            {
                ShootGun(); 
                lastTimeFired = gameTime.TotalElapsedTime;
            }
        }  

        public override void Draw(GameTime gameTime)
        {
            DrawEngine.QueueSpriteForDraw(this.Sprite );
        }
        private void HandlePowerUps(GameTime gameTime)
        {
            for (int i = 0; i < PowerUpEffects.Count; i++)
            {       
                if (!PowerUpEffects[i].Processed)
                {                  
                    ProcessPower(PowerUpEffects[i]);
                    PowerUpEffects[i] = PowerUpEffect.From(PowerUpEffects[i], PowerUpEffects[i].Name, null, gameTime.TotalElapsedTime, true);
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
                    Grow(3);
                    break;
                case PowerUpEffectAction.Shrink:
                    Shrink(2);
                    break;
                case PowerUpEffectAction.Slider_Guns:
                    //only allow one set of guns at a time
                    if (!hasMegaGuns)
                    {
                        hasGuns = true;
                        fireRate = TimeSpan.FromMilliseconds(1000);
                        AddGuns(ConsoleColor.Black);
                    }
                    break;
                case PowerUpEffectAction.Mega_Slider_Guns:
                    hasMegaGuns = true;
                    //remove normal guns if they exist
                    RemoveGuns(ConsoleColor.Black);
                    hasGuns = false;
                    fireRate = TimeSpan.FromMilliseconds(100);
                    
                    AddGuns(ConsoleColor.Red);
                    break;
            }
            GameObjectCollection.ActivePowerUps.Add(power.ToString());
        }
        private void UndoPower(PowerUpEffect power)
        {
            switch (power.PowerUpEffectAction)
            {
                case PowerUpEffectAction.Grow:
                    Shrink(3);
                    break;
                case PowerUpEffectAction.Shrink:
                    Grow(2);
                    break;
                case PowerUpEffectAction.Slider_Guns:
                    hasGuns = false;
                    RemoveGuns(ConsoleColor.Black);
                    break;
                case PowerUpEffectAction.Mega_Slider_Guns:
                    hasMegaGuns = false;                    
                    RemoveGuns(ConsoleColor.Red);
                    break;
            }
            this.PowerUpEffects.Remove(power);
            GameObjectCollection.ActivePowerUps.Remove(power.ToString());
        }
        private void Grow(int times)
        {
            int lX = Sprite.Left.Point.X;
            int rX = Sprite.Right.Point.X;
            int y = Sprite.Left.Point.Y;               
     
            for(int i = 1; i < times + 1; i++)
            {
                if (lX - i > 0)
                {
                    this.Sprite.DrawMap.Add(Pixel.From(Sprite.Left,lX - i, y));
                }
                else
                {
                    this.Sprite.DrawMap.Add(Pixel.From(Sprite.Right,rX + i, y ));
                }
                if (rX + i < CurrentGame.CurrentSettings.WindowWidth - 1)
                {
                    this.Sprite.DrawMap.Add(Pixel.From(Sprite.Right,rX + i, y));
                }
                else
                {
                    this.Sprite.DrawMap.Add(Pixel.From(Sprite.Left,lX - i, y));
                }
            } 
            //make sure the guns stay at the edge
            if (hasMegaGuns)
            {
                RemoveGuns(ConsoleColor.Red);
                AddGuns(ConsoleColor.Red);
            }
            if (hasGuns)
            {
                RemoveGuns(ConsoleColor.Black);
                AddGuns(ConsoleColor.Black);
            }
        }
       
        private void Shrink(int times)
        {          
            for(int i = 0;i< times; i++)
            {
                this.Sprite.DrawMap.Remove(Sprite.Left);
                this.Sprite.DrawMap.Remove(Sprite.Right);
            }
            //make sure the guns stay at the edge
            if (hasMegaGuns)
            {
                RemoveGuns(ConsoleColor.Red);
                AddGuns(ConsoleColor.Red);
            }
            if (hasGuns)
            {
                RemoveGuns(ConsoleColor.Black);
                AddGuns(ConsoleColor.Black);
            }
        }
       
        private void AddGuns(ConsoleColor color)
        {
            var leftPixel = this.Sprite.Left;
            var y = leftPixel.Point.Y - 1;
            var leftX = leftPixel.Point.X;
            var rightX = this.Sprite.Right.Point.X;

            var gunPixelL = new Pixel {
             Point =new Point(leftX,y),
             Color = color,
             Fill =' ',
             ZIndex = 1
            };
            var gunPixelR = new Pixel
            {
                Point = new Point(rightX, y),
                Color = color,
                Fill = ' ',
                ZIndex = 1
            };
            this.Sprite.DrawMap.Add(gunPixelL);
            this.Sprite.DrawMap.Add(gunPixelR);
        }
        private void RemoveGuns(ConsoleColor color)
        {
            this.Sprite.DrawMap.RemoveAll(x => x.Color == color);
        }
        private void ShootGun()
        {
            var bullet = hasMegaGuns ? 
                CurrentGame.GameObjectInitializer.GetMegaBullet(Sprite.Left.Point.X, Sprite.Right.Point.X, Sprite.Top.Point.Y) :
                CurrentGame.GameObjectInitializer.GetBullet(Sprite.Left.Point.X, Sprite.Right.Point.X, Sprite.Top.Point.Y);
            
            GameObjectCollection.Inject(bullet);        
        }
        public override void ProcessCollisions(InteractiveGameObject CollidingObject,GameTime gameTime)
        {
        //this got annoying really fast
          //if(CollidingObject is Ball)
          //{
          //      Console.Beep();
          //}
        }
    }
}
