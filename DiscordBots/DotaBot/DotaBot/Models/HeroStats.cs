using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot.Models
{
    public class HeroStats
    {
        public string DamagerPerMinute { get; set; }
        public string TowerDamagePerMinute { get; set; }
        public string HealingPerMinute { get; set; }
        public string GoldPerMinute { get; set; }
        public string XPPerMinute { get; set; }
        public string KDRatio { get; set; }
        public string WinRate { get; set; }
        public string PickRate { get; set; }
    }     
}
