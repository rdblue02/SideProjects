using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot.Models
{
    public class Hero
    {
        public string Name { get; set; }
        public string DotaBuffSanatizedName { get; set; }
        public HeroStats HeroStats { get; set; } 
        public List<HeroMatchUp> HeroMatchUp { get; set; }
        public List<Item> ItemStats { get; set; }
    }
}
