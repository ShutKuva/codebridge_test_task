using Dogs.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Dogs.Caches
{
    public class MyMemoryCache : ICache<string>
    {
        private readonly IDistributedCache _cache;

        public MyMemoryCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public Task AddAsync<TValue>(string key, TValue value)
        {
            if (value == null)
            {
                return Task.CompletedTask;
            }

            return _cache.SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }

        public async Task<TValue> GetAsync<TValue>(string key, TValue defaultValue)
        {
            byte[]? bytesOfKey = await _cache.GetAsync(key);

            if (bytesOfKey == null)
            {
                return defaultValue;
            }

            TValue? result = JsonConvert.DeserializeObject<TValue>(Encoding.UTF8.GetString(bytesOfKey));

            if (result == null)
            {
                return defaultValue;
            }

            return result;
        }

        public Task RemoveAsync(string key)
        {
            return Task.Run(() => _cache.Remove(key));
        }

        public Task SetAsync<TValue>(string key, TValue value)
        {
            return Task.Run(() =>
            {
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                };

                _cache.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), options);
            });
        }
    }
}
