using DiscordBot.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot.Models
{
    public class HeroIndex : JsonCachedItem
    {
        public Dictionary<string, Hero> Heroes { get; set; }

        public HeroIndex()
        {
            this.Heroes = new Dictionary<string, Hero>();
        }

    }
}
