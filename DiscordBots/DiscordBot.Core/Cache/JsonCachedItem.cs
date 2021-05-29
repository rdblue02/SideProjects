using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public abstract class JsonCachedItem
    {
        public const string cacheFileSufix = "-cache.txt";
        public DateTime InitializedDate { get; }
        public string CacheKey { get; }
        public JsonCachedItem()
        {
            this.InitializedDate = DateTime.UtcNow;
            this.CacheKey = this.GetType().ToString() + cacheFileSufix;
        }
       
    }
}
