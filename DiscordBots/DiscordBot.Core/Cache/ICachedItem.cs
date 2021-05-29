using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.Cache
{
    public interface ICachedItem
    {
        FileExtensions FileType { get; }
        public DateTime InitializedDate { get; }
        public abstract string CacheName { get; set; }
        public string CacheKey { get => $"{CacheName}.{FileType}"; }
    }
}
