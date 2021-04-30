using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker.Events
{
    public class GameEndEvent
    {
        public GameEndReason GameEndReason { get; }
        public GameEndEvent(GameEndReason gameEndReason)
        {
            this.GameEndReason = gameEndReason;
        }
    }
}
