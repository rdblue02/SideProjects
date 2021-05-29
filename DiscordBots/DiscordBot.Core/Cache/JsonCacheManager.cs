using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.Cache
{
    public class JsonCacheManager:ICacheManager
    {
        private static string cacheDirectoryPath => Directory.GetCurrentDirectory() + "//Cache//";
        private Dictionary<string,string> _cache = new Dictionary<string, string>();
        public Task InitializationTask { get; }

        public JsonCacheManager()
        {
            this.InitializationTask = Initialize();
        }
        public async Task Initialize()
        {
            if (this.InitializationTask?.IsCompleted ?? false)
            {
                return;
            }

            if (!Directory.Exists(cacheDirectoryPath))
            {
                Directory.CreateDirectory(cacheDirectoryPath);
                Console.WriteLine($"Unable to find cache directory {cacheDirectoryPath}");
                return;
            }

            List<Task> loadTasks = new List<Task>();
            foreach (var key in GetCacheKeys())
            {
                Console.WriteLine($"Loading cache for key {key}");
                loadTasks.Add(LoadCachedItem(key));
            }

             await Task.WhenAll(loadTasks);
        }

        public Task<bool> GetObjectFromCache<T>(string keyName, out Task<T> requestedObejct) where T: JsonCachedItem
        {
            requestedObejct = null;
            try
            {
                if (_cache.ContainsKey(keyName))
                { 
                    var objFromJson = JsonConvert.DeserializeObject<T>(_cache[keyName]);
                    requestedObejct = Task.FromResult(objFromJson);
                }             
            }
            catch (Exception e)
            {
                Console.WriteLine(e);               
            }
            return Task.FromResult(requestedObejct != null);
        }

        public Task<bool> GetObjectFromCache<T>(T tObj, out Task<T> requestedObejct) where T : JsonCachedItem
        {
            requestedObejct = null;
            try
            {
                if (_cache.ContainsKey(tObj.CacheKey))
                {
                    var objFromJson = JsonConvert.DeserializeObject<T>(_cache[tObj.CacheKey]);
                    requestedObejct = Task.FromResult(objFromJson);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Task.FromResult(requestedObejct != null);
        }
        public async Task UpdateOrAddCachedItem(JsonCachedItem obj)
        {
           
            if (!_cache.Keys.Contains(obj.CacheKey))
            {
                _cache.Add(obj.CacheKey,JsonConvert.SerializeObject(obj));
            }
            else
            {
                _cache[obj.CacheKey] = JsonConvert.SerializeObject(obj);    
            }

            await SaveCache(obj.CacheKey);
        }
       
        public Task DeleteItemFromCache<T>(T obj) where T : JsonCachedItem
        {
           if(_cache.ContainsKey(obj.CacheKey))
           {
                _cache.Remove(obj.CacheKey);
                File.Delete(cacheDirectoryPath + obj.CacheKey);
           }
         
            return Task.CompletedTask;
        }
        public Task DeleteItemFromCache(string key)
        {
            if (_cache.ContainsKey(key))
            {
                _cache.Remove(key);
                File.Delete(cacheDirectoryPath + key);
            }

            return Task.CompletedTask;
        }
        private List<string> GetCacheKeys()=>
        new DirectoryInfo(cacheDirectoryPath).GetFiles()
                                             .Where(x=>x.Name.ToLower().Contains(JsonCachedItem.cacheFileSufix))
                                             .Select(x => x.Name)
                                             .ToList();
  
        private async Task LoadCachedItem(string cacheKey)
        {
            if (_cache.ContainsKey(cacheKey))
            {
                throw new Exception($"Duplicate Cache Files Found for cache key {cacheKey}");
            }

            var savedData = await File.ReadAllTextAsync(cacheDirectoryPath + cacheKey);
               _cache[cacheKey] = savedData;
        } 

        private async Task SaveCache(string cacheKey) 
        {
            if(_cache.ContainsKey(cacheKey))
            await File.WriteAllTextAsync(cacheDirectoryPath + cacheKey, _cache[cacheKey]);
        }
       
    }
}
