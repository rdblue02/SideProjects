using BrickBreaker.Events;
using BrickBreaker.GameObjects;
using ConsoleGameFrameWork;
using ConsoleGameFrameWork.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
 
namespace BrickBreaker
{
    public class CurrentGame : Game
    {
        public static int Level { get; private set; } = 1;
        public static long Score { get; set; }
        public static Randomizer Randomizer = new Randomizer();
        //We make this static so all the game objects can access it.
        //Todo find a better way than this. Maybe pass it into the objects it creates?
        public static GameObjectInitializer GameObjectInitializer { get; } = new GameObjectInitializer();
        public const int MaxLevel = 10;

        GameState currentState = GameState.Starting;
        TimeSpan timeSinceLastRandomPowerUp = TimeSpan.FromSeconds(0);
        TimeSpan waitTimeForRandomPowerUp = TimeSpan.FromSeconds(2);

        Dictionary<GameState, string> stateMessageMap = new Dictionary<GameState, string>() {
            { GameState.Starting, MessageConstants.Begin},
            { GameState.Paused, MessageConstants.Pause},
            { GameState.GameOver, MessageConstants.GameOver},
            { GameState.Active, null},
            { GameState.GameComplete, MessageConstants.GameComplete},
            { GameState.LevelCompleted, MessageConstants.LevelComplete},
        };
        public CurrentGame()
        {          
            Randomizer.RefreshRate = TimeSpan.FromMinutes(10);
        }
        //Set up the objects
        public override void Initialize()
        {
            
            var ball = GameObjectInitializer.GetBall();
            ball.OnGameEnd += CurrentGame_OnGameEnd;

            var slider = GameObjectInitializer.GetSlider();
            var bricks = GameObjectInitializer.GetBricks();
                bricks.ForEach(x => x.OnGameEnd += CurrentGame_OnGameEnd);

            var messages = GameObjectInitializer.GetMessageCollection();

            var toAdd = new List<GameObject>() { ball, slider };
                toAdd.AddRange(bricks);
                toAdd.AddRange(messages);

           GameObjectCollection.Initialize(toAdd);

        }       
        public override void Update(GameTime gameTime)
        {
            Randomizer.Update(gameTime);           
            switch (currentState)
            {
                case GameState.Starting :
                    HandleStart(gameTime);
                    break;
                case GameState.Active:                    
                    HandleActive(gameTime);
                    break;
                case GameState.Paused:
                    HandlePause(gameTime);
                    break;
                case GameState.LevelCompleted:
                    HandleLevelComplete(gameTime);
                    break;
                case GameState.GameOver:
                    HandleGameOver(gameTime);
                    break;
                case GameState.GameComplete:
                    HandleGameComplete(gameTime);
                    break;
            }
            GameObjectCollection.ProcessPending();           
        }
        public override void Draw(GameTime gameTime)
        {
            GameObjectCollection.DrawCollection(gameTime);
        }

        private void HandleStart(GameTime gameTime)
        {           
            if (CommandManager.IsKeyUp(Keys.Space))
            { 
                this.currentState = GameState.Active;
                this.timeSinceLastRandomPowerUp = gameTime.TotalElapsedTime;
            }
            SetScreenMessageFromState();
        }
        private void HandleActive(GameTime gameTime)
        {
            if (CheckForPause())
            {
                return;
            }
            //process all the collisions first so we don't have to worry about the order things are processed in.
            //this can probably get changed to a better way now that all objects handle their own collision logic.
            GameObjectCollection.CheckCollisions(gameTime);
            GameObjectCollection.UpdateCollection(gameTime);
            
            //todo replace this magic number with a const. Does a limit of 50 even make sense?
            //run some tests to find out if the size of this collection could ever become a problem.
            if (GameObjectCollection.CountInactive<InteractiveGameObject>() > 50)
            {
                GameObjectCollection.DestroyInactive();
            }

            //If the user hasn't seen a  power up in a while, give them one for free.    
            if(gameTime.TotalElapsedTime - timeSinceLastRandomPowerUp > waitTimeForRandomPowerUp)
            {
                if(GameObjectCollection.ActivePowerUps.Any())
                {
                    timeSinceLastRandomPowerUp = gameTime.TotalElapsedTime;
                }
                else
                {
                    var xMin = CurrentSettings.WindowWidth / 4;
                    var xMax = CurrentSettings.WindowWidth - CurrentSettings.WindowWidth / 4;
                    var y = CurrentSettings.WindowHeight / 3;
                    var power = GameObjectInitializer.GetRandomPowerUp(Randomizer.RandomNumber(xMin,xMax),y);
                    GameObjectCollection.Inject(power);
                    timeSinceLastRandomPowerUp = gameTime.TotalElapsedTime;
                } 
            }
        }
        private void HandlePause(GameTime gameTime)
        {
            if (CommandManager.IsKeyUp(Keys.Enter))
            {
                currentState = GameState.Active;              
            }
            SetScreenMessageFromState();
        }      
        private void HandleGameOver(GameTime gameTime)
        {   
            if(CommandManager.IsKeyUp(Keys.Space))
            {
                Level = 1;
                Initialize();
                this.currentState = GameState.Active;                              
            }
            SetScreenMessageFromState();
        }
        private void HandleLevelComplete(GameTime gameTime)
        {       
            if (CommandManager.IsKeyUp(Keys.Space))
            {
                Level++;
                Initialize();
                currentState = GameState.Active;          
            }
            SetScreenMessageFromState();
        }
        private void HandleGameComplete(GameTime gameTime)
        {
            SetScreenMessageFromState();
        }
        private bool CheckForPause()
        {
            //use key up or we keep pausing and unpausing
            if (CommandManager.IsKeyUp(Keys.Enter))
            {
                if (currentState == GameState.Active)
                { 
                    currentState = GameState.Paused;
                    SetScreenMessageFromState();
                    return true;
                }
            }
            return false;
        }

        private void SetScreenMessageFromState()
        {
            if(currentState != GameState.Active)
            {
                var message = GameObjectCollection.Collection.FirstOrDefault(x => x is InfoMessage infoMessage &&
                                                                                   infoMessage.MessageId == stateMessageMap[currentState]);
               
                if (message != null)
                {
                    message.IsActive = true;
                }
            }
            else
            {
                GameObjectCollection.GetActive<InfoMessage>().ForEach(x => x.IsActive = false);
            }
        }     
        private void CurrentGame_OnGameEnd(object sender, GameEndEvent e)
        {
            switch (e.GameEndReason)
            {
                case GameEndReason.GameOver:
                    this.currentState = GameState.GameOver;
                    break;
                case GameEndReason.AllBricksBroken:
                    this.currentState = Level + 1 > MaxLevel ? GameState.GameComplete : GameState.LevelCompleted;
                    if(currentState == GameState.LevelCompleted)
                    {
                        Score += 100;
                    }
                    break;
               
            }
        }
    }
}
