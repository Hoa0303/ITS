using Microsoft.Extensions.Caching.Memory;

namespace ITS_BE.Services.Caching
{
    public interface ICachingService
    {
        T? Get<T>(string cacheKey);
        void Set<T>(string cacheKey, T value);
        void Set<T>(string cacheKey, T value, MemoryCacheEntryOptions options);
        void Remove(string cacheKey);
    }
}
