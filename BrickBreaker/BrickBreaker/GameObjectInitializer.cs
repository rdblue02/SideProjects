using BrickBreaker.GameObjects;
using ConsoleGameFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrickBreaker
{
    //todo look into making this methods generic. Maybe use a factory pattern?
    public class GameObjectInitializer
    {
        Dictionary<Type, List<PowerUpEffect>> PowerUpEffectMap { get;}
        public GameObjectInitializer()
        {
            PowerUpEffectMap = new Dictionary<Type, List<PowerUpEffect>>()
            {
              {
                typeof(Slider),SliderPowerUpEffects()
              },
              {
                typeof(Ball),BallPowerUpEffects()
              },
              {
                typeof(Brick),BrickPowerUpEffects()
              },
              {
                typeof(InteractiveGameObject), MiscPowerUpEffects()
              }
            };
        }
      
        public Ball GetBall()
        {
            float speed;
            if (CurrentGame.Level <= 4)
            {
                speed = 5f;
            }
            else if (CurrentGame.Level > 4 && CurrentGame.Level <= 6)
            {
                speed = 10f;
            }
            else
            {
                speed = 15f;
            }
            var ball = new Ball()
            {
                IsMainBall = true,
                IsActive = true,
                Speed = speed,
                Sprite = new Sprite() { DrawMap = new List<Pixel>()},
                CurrentDirection = CurrentGame.Randomizer.ByPercentage(50) ? Direction.DownLeft : Direction.DownRight
            };

            var startX = Game.CurrentSettings.WindowWidth / 2;
            var startY = Game.CurrentSettings.WindowHeight / 2-5;

            ball.Sprite.DrawMap.Add(new Pixel() { Point = new Point(startX, startY), Color = ConsoleColor.Yellow, Fill = ' ', ZIndex = 2 });
            ball.Sprite.DrawMap.Add(new Pixel() { Point = new Point(startX + 1, startY), Color = ConsoleColor.Yellow, Fill = ' ', ZIndex = 2 });
 
            return ball;
        }
       
        public Slider GetSlider()
        {
            var slider = new Slider() { Speed = 1, IsActive =true };
            slider.Sprite = new Sprite() { DrawMap = new List<Pixel>()};

            var startX = Game.CurrentSettings.WindowWidth / 2;
            var startY = Game.CurrentSettings.WindowHeight - Game.CurrentSettings.WindowHeight / 5;

            for (int i = 0; i < 10; i++)
            {
                slider.Sprite.DrawMap.Add(new Pixel() { Point = new Point(startX + i, startY), Color = ConsoleColor.Green, Fill = ' ', ZIndex = 1 });
            }

            return slider;
        }
 
        public List<Brick> GetBricks()
        {
            var maxBrickLevel = Brick.BrickColorMap.Keys.Max()>=CurrentGame.Level ? CurrentGame.Level: Brick.BrickColorMap.Keys.Max();
            int minBrickLevel;

            if(CurrentGame.Level <= 4)
            {
                minBrickLevel = 1;
            }            
            else if (CurrentGame.Level >4 && CurrentGame.Level <=6)
            {
                minBrickLevel = 2;
            }
            else
            {
                minBrickLevel = 3;
            }
             
            var bricks = new List<Brick>();

            int rows = 3;
            int brickWidth = 8;

            for (int y = 0; y < rows * 2; y += 2)
            {
                for (int x = 3; x + brickWidth - 1 < (Game.CurrentSettings.WindowWidth - 1); x += brickWidth + 1)
                {
                    var brick = new Brick() { Speed = 0, IsActive = true };
                    brick.BrickLevel = CurrentGame.Randomizer.RandomNumber(minBrickLevel, maxBrickLevel+1);

                    var brickColor = Brick.BrickColorMap[brick.BrickLevel];
                    for (int pointX = x; pointX < x + brickWidth; pointX++)
                    {
                        var point = new Point(pointX, y);
                        brick.Sprite.DrawMap.Add(new Pixel() { Point = point, Color = brickColor, Fill = ' ', FontColor = brickColor, ZIndex = 1 });
                    }

                    bricks.Add(brick);
                }
            }
            return bricks;
        }

     /// <summary>
     /// this is only used for testing.
     /// </summary>
     /// <typeparam name="T"></typeparam>
     /// <param name="x"></param>
     /// <param name="y"></param>
     /// <param name="power"></param>
     /// <returns></returns>
        public PowerUp<T> GetSpecificPower<T>(int x, int y, PowerUpEffectAction power) where T: InteractiveGameObject
        {
            var powerUp = new PowerUp<T>()
            {
                IsActive = true,
                Speed = 2
            };
            powerUp.PowerUpEffects.Add(PowerUpEffectMap[typeof(T)].FirstOrDefault(x => x.PowerUpEffectAction == power));
            powerUp.Sprite.DrawMap.Add(new Pixel() { Fill = ' ', Color = powerUp.EffectColor, FontColor = powerUp.EffectColor, Point = new Point(x, y), ZIndex = 1 });
            return powerUp;
        }
       
        public InteractiveGameObject GetRandomPowerUp(int x, int y)
        {
            //Only random between 0 and 2 because currently there are no Brick or MiscPowerUp.
            var powerUpType = CurrentGame.Randomizer.RandomNumber(0, 2);                 
            switch (powerUpType)
            {
                case 0:
                    return GetPowerUp<Slider>(x, y);
                case 1:
                    return GetPowerUp<Ball>(x, y);
                case 2:
                    return GetPowerUp<Brick>(x, y);                          
            }
            return GetPowerUp<InteractiveGameObject>(x, y);
        }       
    
        public Bullet GetBullet(int xl, int xr, int y)
        {
            var bullet = new Bullet() {
             IsActive= true ,
             Speed = 1000
            };
            bullet.Sprite.DrawMap.Add(new Pixel { Color = ConsoleColor.Black, Fill = ' ', Point = new Point(xl, y), ZIndex = 1 });
            bullet.Sprite.DrawMap.Add(new Pixel { Color = ConsoleColor.Black, Fill = ' ', Point = new Point(xr, y), ZIndex = 1 });
            return bullet;
        }
        public Bullet GetMegaBullet(int xl, int xr, int y)
        {
            var bullet = new Bullet()
            {
                IsActive = true,
                Speed = 1000
            };
            bullet.Sprite.DrawMap.Add(new Pixel { Color = ConsoleColor.Red, Fill = ' ', Point = new Point(xl, y), ZIndex = 1 });
            bullet.Sprite.DrawMap.Add(new Pixel { Color = ConsoleColor.Red, Fill = ' ', Point = new Point(xl + 1, y), ZIndex = 1 });
            bullet.Sprite.DrawMap.Add(new Pixel { Color = ConsoleColor.Red, Fill = ' ', Point = new Point(xr, y), ZIndex = 1 });
            bullet.Sprite.DrawMap.Add(new Pixel { Color = ConsoleColor.Red, Fill = ' ', Point = new Point(xr -1, y), ZIndex = 1 });
            return bullet;
        }
        public List<GameObject> GetMessageCollection()
        {
            var gameOver = new InfoMessage(MessageConstants.GameOver) { IsActive = false, Sprite = Helpers.ToMessageInfoSprite(MessageConstants.GameOver,  ConsoleColor.Black) };
            var pause = new InfoMessage(MessageConstants.Pause) { IsActive = false, Sprite = Helpers.ToMessageInfoSprite(MessageConstants.Pause,  ConsoleColor.Black) };
            var begin = new InfoMessage(MessageConstants.Begin) { IsActive = false, Sprite = Helpers.ToMessageInfoSprite(MessageConstants.Begin, ConsoleColor.Black) };
            var levelComplete = new InfoMessage(MessageConstants.LevelComplete) { IsActive = false, Sprite = Helpers.ToMessageInfoSprite(MessageConstants.LevelComplete, ConsoleColor.Black) };
            var gameComplete = new InfoMessage(MessageConstants.GameComplete) { IsActive = false, Sprite = Helpers.ToMessageInfoSprite(MessageConstants.GameComplete, ConsoleColor.Black) };
         
            var powerUpStats = new DynamicInfoMessage(()=> {
                var activePowerUps = new StringBuilder();          
                foreach (var effect in GameObjectCollection.ActivePowerUps)
                {
                    activePowerUps.Append(effect + ", ");
                }
                return MessageConstants.PowerUpStatus + activePowerUps.ToString().TrimEnd().TrimEnd(',');
            }, 0, CurrentGame.CurrentSettings.WindowHeight - 2) { IsActive =true };
            
            var gameStats = new DynamicInfoMessage(()=> {
                return $"Level: {CurrentGame.Level} Score: {CurrentGame.Score}";
            }, 0, CurrentGame.CurrentSettings.WindowHeight - 1) { IsActive = true };
         
            var infoMessages = new List<GameObject>() { gameOver, pause, begin,levelComplete,gameComplete, powerUpStats, gameStats };
            return infoMessages;
        }
        PowerUp<T> GetPowerUp<T>(int x, int y) where T : InteractiveGameObject
        {
            var availAblePowerUps = PowerUpEffectMap[typeof(T)].Where(x => x.MinLevelRequired <= CurrentGame.Level && 
                                                                !GameObjectCollection.ActivePowerUps.Contains(x.Name))
                                                                .ToList();
            if (availAblePowerUps.Any())
            {
                var powerUpEffect = availAblePowerUps[CurrentGame.Randomizer.RandomNumber(0, availAblePowerUps.Count)];

                var powerUp = new PowerUp<T>()
                {
                    IsActive = true,
                    Speed = 2
                };
                powerUp.PowerUpEffects.Add(powerUpEffect);
                powerUp.Sprite.DrawMap.Add(new Pixel() { Fill = ' ', Color = powerUp.EffectColor, FontColor = powerUp.EffectColor, Point = new Point(x, y), ZIndex = 1 });
                return powerUp;
            }
            else
            {
                return null;
            }
        }

        //Initialize all of the available Power Ups
        List<PowerUpEffect> SliderPowerUpEffects()
        {
            var effects = new List<PowerUpEffect>() {                
                new PowerUpEffect(){ Duration = TimeSpan.FromSeconds(30), PowerUpEffectAction = PowerUpEffectAction.Grow,MinLevelRequired=1 ,Name = "Grow Slider" },
                new PowerUpEffect(){ Duration = TimeSpan.FromSeconds(15), PowerUpEffectAction = PowerUpEffectAction.Shrink,MinLevelRequired=1, Name="Shrink Slider" },
                new PowerUpEffect(){ Duration = TimeSpan.FromSeconds(30), PowerUpEffectAction = PowerUpEffectAction.Slider_Guns,MinLevelRequired=1, Name="Slider Guns! (F to shoot)" },
                new PowerUpEffect(){ Duration = TimeSpan.FromSeconds(15), PowerUpEffectAction = PowerUpEffectAction.Mega_Slider_Guns,MinLevelRequired=3, Name="Mega Slider Guns! (F to shoot)" }
                };             
            return effects;
        }
        List<PowerUpEffect> BallPowerUpEffects()
        {
            var effects = new List<PowerUpEffect>() {
                new PowerUpEffect(){ Duration = TimeSpan.FromSeconds(30), PowerUpEffectAction = PowerUpEffectAction.Grow,MinLevelRequired=1, Name="Grow Ball"},
                new PowerUpEffect(){ Duration = TimeSpan.FromSeconds(15), PowerUpEffectAction = PowerUpEffectAction.Ball_MultiBrickBreak,MinLevelRequired=3, Name="Multi Brick Break"},
                new PowerUpEffect(){ Duration = TimeSpan.FromSeconds(20), PowerUpEffectAction = PowerUpEffectAction.Ball_Split,MinLevelRequired=2, Name="Ball Split"},
                new PowerUpEffect(){ Duration = TimeSpan.FromSeconds(15), PowerUpEffectAction = PowerUpEffectAction.DecreaseSpeed,MinLevelRequired=1 , Name="Ball Speed Decrease"},
                new PowerUpEffect(){ Duration = TimeSpan.FromSeconds(15), PowerUpEffectAction = PowerUpEffectAction.IncreaseSpeed,MinLevelRequired=1, Name="Ball Speed Increase" }, 
                };
            return effects;
        }
        List<PowerUpEffect> BrickPowerUpEffects()
        {
            return new List<PowerUpEffect>();
        }
        List<PowerUpEffect> MiscPowerUpEffects()
        {
            return new List<PowerUpEffect>();
        }
    }
}
