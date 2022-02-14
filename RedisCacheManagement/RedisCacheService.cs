using System;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;


namespace RedisCacheManagement
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private int _defaultCacheTime = 10;
 
        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
 
        public T Get<T>(string key)
        {
            var value = _cache.GetString(key);
 
            if (value != null)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
 
            return default;
        }
        
        public T Set<T>(string key, T value)
        {
            var timeOut = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_defaultCacheTime)
            };
            _cache.SetString(key,  JsonConvert.SerializeObject(value), timeOut);
 
            return value;
        }
 
    }
}