using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker
{ 
  public enum GameEndReason
    {
        GameOver = 1,
        AllBricksBroken =2
    }
  public enum GameState
    {
        Starting,
        Active,
        Paused,
        LevelCompleted,
        GameOver,
        GameComplete
    }
   public enum PowerUpEffectAction
   {
        None,
        IncreaseSpeed,
        DecreaseSpeed,
        Grow,
        Shrink,
        Slider_Guns,
        Mega_Slider_Guns,
        Ball_Split,
        Ball_MultiBrickBreak    
    }
}
