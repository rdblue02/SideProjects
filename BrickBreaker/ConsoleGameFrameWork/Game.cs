using System;
using System.Diagnostics;
using ConsoleGameFrameWork.Events;
using ConsoleGameFrameWork.Input;

namespace ConsoleGameFrameWork
{
    public abstract class Game
    {
        public abstract void Initialize();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);

        public static event EventHandler<WindowResizedEvent> OnWindowResize;
        public static event EventHandler<PauseEvent> onPauseKeyPressed;
        public static Settings CurrentSettings { get; private set; } = Settings.GetDefaultSettings();
        protected bool IsPaused { get; set; }
        public bool IsRunning { get; private set; }
        Stopwatch gameStopwatch;
        GameTime gameTime;      
        TimeSpan targetFrameTime;//how long each frame should last
        TimeSpan previousTime;


        public Game()
        {
            gameTime = new GameTime()
            {            
                DeltaTime = new TimeSpan(),
                TargetFPS = 60,              
                CurrentFrame = 0,
                TotalElapsedTime = TimeSpan.FromSeconds(0)
            };

            targetFrameTime = TimeSpan.FromSeconds(1) / 60;
            this.IsPaused = false;
            this.IsRunning = true;           
            this.gameStopwatch = new Stopwatch();
            CurrentSettings.Apply();
        }

        /// <summary>
        /// run Initialize logic, start gametime, and start the main loop
        /// </summary>
        public void Run()
        {
            gameStopwatch.Start();
            DrawEngine.InitializeScreen();
            Initialize();
            GameLoop();
        }

       
        private void GameLoop()
        {
            TimeSpan frameCycleStartTime = gameStopwatch.Elapsed;           
            while (IsRunning)
            {
                gameTime.DeltaTime = gameStopwatch.Elapsed - previousTime;
                gameTime.CurrentFrame++;
                gameTime.TotalElapsedTime = gameStopwatch.Elapsed;
                gameTime.PassedFrameTime = gameStopwatch.Elapsed - frameCycleStartTime;
              
                if ( gameTime.PassedFrameTime > targetFrameTime * gameTime.TargetFPS)
                {
                    if (CurrentSettings.DisplayFps)
                    {
                        Helpers.WriteAt("FPS: " + gameTime.CurrentFrame, new Point(0, CurrentSettings.WindowHeight - 1));
                        Helpers.WriteAt("DELTA: " + gameTime.DeltaTime, new Point(0, CurrentSettings.WindowHeight - 2));
                    }
                    frameCycleStartTime = gameStopwatch.Elapsed;
                    gameTime.CurrentFrame = 1;
                }
                if (CurrentSettings.DisplayFps)
                {
                    Helpers.WriteAt("Frame: " + gameTime.CurrentFrame, new Point(0, CurrentSettings.WindowHeight - 3));
                    Helpers.WriteAt("Time: " + gameTime.TotalElapsedTime, new Point(0, CurrentSettings.WindowHeight - 4));
                }
                
                CheckForResize();
                CommandManager.GetInput();
                UpdateInternal(gameTime);

                if(gameTime.DeltaTime < targetFrameTime)
                {
                    while (gameTime.DeltaTime < targetFrameTime)
                    {
                        gameTime.DeltaTime = gameStopwatch.Elapsed - previousTime;
                        DrawInternal(gameTime);
                    }
                }
                else
                {
                    DrawInternal(gameTime);
                }               
                previousTime = gameStopwatch.Elapsed;
            }
        }

             
        private void UpdateInternal(GameTime gameTimeForUpdate)
        {
            Update(gameTimeForUpdate);
        }
        
        private void DrawInternal(GameTime gameTimeForDraw)
        {
            Draw(gameTimeForDraw);
            DrawEngine.DrawScreen();
        }
        //Can't figure out a way to prevent the user from changing the window size. 
        //If we don't allow resize, just reset the size back where we want it.
        private void CheckForResize()
        {            
            if(Console.WindowWidth != CurrentSettings.WindowWidth || Console.WindowHeight != CurrentSettings.WindowHeight)
            {
                if (CurrentSettings.AllowResize)
                {
                    CurrentSettings.WindowWidth = Console.WindowWidth;
                    CurrentSettings.WindowHeight = Console.WindowHeight;
                    CurrentSettings.Apply();
                    OnWindowResize?.Invoke(this,new WindowResizedEvent());
                }
                else
                {
                    CurrentSettings.Apply();
                }
                DrawEngine.InitializeScreen();
            }           
        }

        /// <summary>
        /// Allow user to Apply new settings
        /// </summary>
        /// <param name="settings"></param>
        protected void ApplySettings(Settings settings)
        {
            CurrentSettings = settings;
            CurrentSettings.Apply();
        }     
    }
}
