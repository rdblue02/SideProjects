using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.Cache
{
    public interface ICacheManager
    {
        
        Task InitializationTask { get; }
        Task Initialize();
        public Task<bool> GetObjectFromCache<T>(string keyName, out Task<T> requestedObejct) where T : JsonCachedItem;
        public Task<bool> GetObjectFromCache<T>(T tObj, out Task<T> requestedObejct) where T : JsonCachedItem;
        Task UpdateOrAddCachedItem(JsonCachedItem obj);
        Task DeleteItemFromCache<T>(T obj) where T : JsonCachedItem;
    }
}
