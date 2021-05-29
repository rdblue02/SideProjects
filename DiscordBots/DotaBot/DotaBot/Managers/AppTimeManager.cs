using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotaBot.Managers
{
    public class AppTimeManager
    {
        public DateTime InitializedDate { get; }
        public TimeSpan ElapsedTime => timer.Elapsed;

        private Stopwatch timer;
        public AppTimeManager()
        {
            this.InitializedDate = DateTime.Now;
            this.timer = new Stopwatch();
            this.timer.Start();           
        }     
        public Task ResetTimer()
        {
            this.timer.Restart();
            return Task.CompletedTask;
        }
    }
}
