namespace ITS_BE.Services.Caching
{
    public interface ICachingService
    {
        T? Get<T>(string cacheKey);
        void Set<T>(string cacheKey, T value);
        void Remove(string cacheKey);
    }
}
