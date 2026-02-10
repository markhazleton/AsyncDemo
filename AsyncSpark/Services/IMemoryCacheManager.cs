namespace AsyncSpark.Services;

/// <summary>
/// Interface for managing memory cache operations with key tracking and expiration.
/// </summary>
public interface IMemoryCacheManager
{
    /// <summary>
    /// Clears all cached items from the memory cache.
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Disposes of the cache manager and releases all resources.
    /// </summary>
    void Dispose();
    
    /// <summary>
    /// Gets a cached item by key, or acquires and caches it if not present.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="acquire">Function to acquire the value if not cached.</param>
    /// <param name="cacheTime">Optional cache time in minutes. If null, uses default.</param>
    /// <returns>The cached or newly acquired item.</returns>
    T Get<T>(string key, Func<T> acquire, int? cacheTime = null);
    
    /// <summary>
    /// Gets all cache keys currently in use.
    /// </summary>
    /// <returns>A list of all cache keys.</returns>
    IList<string> GetKeys();
    
    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    /// <param name="key">The cache key to check.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    bool IsSet(string key);
    
    /// <summary>
    /// Performs an action with a distributed lock based on the specified key.
    /// </summary>
    /// <param name="key">The lock key.</param>
    /// <param name="expirationTime">The lock expiration time.</param>
    /// <param name="action">The action to perform while holding the lock.</param>
    /// <returns>True if the lock was acquired and action executed, false otherwise.</returns>
    bool PerformActionWithLock(string key, TimeSpan expirationTime, Action action);
    
    /// <summary>
    /// Removes a cached item by key.
    /// </summary>
    /// <param name="key">The cache key to remove.</param>
    void Remove(string key);
    
    /// <summary>
    /// Sets a value in the cache with the specified key and expiration time.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="data">The data to cache.</param>
    /// <param name="cacheTime">Cache time in minutes.</param>
    void Set(string key, object data, int cacheTime);
}
