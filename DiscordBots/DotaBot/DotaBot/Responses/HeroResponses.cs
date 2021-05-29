using DotaBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot.Responses
{
    public class HeroResponses
    {
        

        public class HeroListResponse 
        {
            public List<Hero> Heroes { get; set; }
            public HeroListResponse()
            {
                Heroes = new List<Hero>();
            }
        }
     
        public class HeroDAndHResponse 
        {
            public string HeroName { get; set; }
            public string DamagerPerMinute { get; set; }
            public string TowerDamagePerMinute { get; set; }
            public string HealingPerMinute { get; set; }
        }
        public class EconomyResponse 
        {
            public string HeroName { get; set; }
            public string GPM { get; set; }
            public string XPM { get; set; }
        }
        public class WinRateResponse 
        {
            public string HeroName { get; set; }
            public string WinRate { get; set; }
            public string KDRatio { get; set; }
            public string PickRate { get; set; }
        }

      
    }
    
}
