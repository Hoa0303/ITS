using Microsoft.Extensions.Caching.Memory;

namespace ITS_BE.Services.Caching
{
    public class CachingService : ICachingService
    {
        private readonly IMemoryCache _memoryCache;
        public CachingService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        public T? Get<T>(string cacheKey)
        {
            _memoryCache.TryGetValue(cacheKey, out T? value);
            return value;
        }

        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public void Set<T>(string cacheKey, T value)
        {
            _memoryCache.Set(cacheKey, value, TimeSpan.FromHours(6));
        }

        public void Set<T>(string cacheKey, T value, MemoryCacheEntryOptions options)
        {
            _memoryCache.Set(cacheKey, value, options);
        }
    }
}
