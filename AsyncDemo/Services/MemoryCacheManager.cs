﻿#nullable enable
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;

namespace AsyncDemo.Services;

public class MemoryCacheManager : IMemoryCacheManager
{

    /// <summary>
    /// All keys of cache
    /// </summary>
    /// <remarks>Dictionary value indicating whether a key still exists in cache</remarks> 
    protected static readonly ConcurrentDictionary<string, bool> _allKeys;
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Cancellation token for clear cache
    /// </summary>
    protected CancellationTokenSource _cancellationTokenSource;

    static MemoryCacheManager()
    {
        _allKeys = new ConcurrentDictionary<string, bool>();
    }

    public MemoryCacheManager(IMemoryCache cache)
    {
        _cache = cache;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Remove all keys marked as not existing
    /// </summary>
    private static void ClearKeys()
    {
        foreach (var key in _allKeys.Where(p => !p.Value).Select(p => p.Key).ToList())
        {
            RemoveKey(key);
        }
    }

    /// <summary>
    /// Post eviction
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <param name="value">Value of cached item</param>
    /// <param name="reason">Eviction reason</param>
    /// <param name="state">State</param>
    private void PostEviction(object? key, object? value, EvictionReason reason, object? state)
    {
        //if cached item just change, then nothing doing
        if (reason == EvictionReason.Replaced)
            return;

        //try to remove all keys marked as not existing
        ClearKeys();

        //try to remove this key from dictionary
        TryRemoveKey(key?.ToString() ?? string.Empty);
    }

    /// <summary>
    /// Add key to dictionary
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <returns>Itself key</returns>
    protected static string AddKey(string key)
    {
        _allKeys.TryAdd(key, true);
        return key;
    }


    /// <summary>
    /// Create entry options to item of memory cache
    /// </summary>
    /// <param name="cacheTime">Cache time</param>
    protected MemoryCacheEntryOptions GetMemoryCacheEntryOptions(TimeSpan cacheTime)
    {
        var options = new MemoryCacheEntryOptions()
            // add cancellation token for clear cache
            .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token))
            //add post eviction callback
            .RegisterPostEvictionCallback(PostEviction);

        //set cache time
        options.AbsoluteExpirationRelativeToNow = cacheTime;

        return options;
    }

    /// <summary>
    /// Remove key from dictionary
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <returns>Itself key</returns>
    protected static string RemoveKey(string key)
    {
        TryRemoveKey(key);
        return key;
    }


    /// <summary>
    /// Try to remove a key from dictionary, or mark a key as not existing in cache
    /// </summary>
    /// <param name="key">Key of cached item</param>
    protected static void TryRemoveKey(string key)
    {
        //try to remove key from dictionary
        if (!_allKeys.TryRemove(key, out _))
            //if not possible to remove key from dictionary, then try to mark key as not existing in cache
            _allKeys.TryUpdate(key, false, true);
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    public virtual void Clear()
    {
        foreach (var key in _allKeys.Keys.ToList())
        {
            _cache.Remove(key);
        }
        _allKeys.Clear(); _allKeys.Clear();

        //send cancellation request
        _cancellationTokenSource.Cancel();

        //releases all resources used by this cancellation token
        _cancellationTokenSource.Dispose();

        //recreate cancellation token
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Dispose cache manager
    /// </summary>
    public virtual void Dispose()
    {
        //nothing special
    }

    /// <summary>
    /// Get a cached item. If it's not in the cache yet, then load and cache it
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="acquire">Function to load item if it's not in the cache yet</param>
    /// <param name="cacheTime">Cache time in minutes; pass 0 to do not cache; pass null to use the default time</param>
    /// <returns>The cached value associated with the specified key</returns>
    public virtual T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
    {
        //item already is in cache, so return it
        if (_cache.TryGetValue(key, out T? value) && value != null)
            return value;

        //or create it using passed function
        var result = acquire();
        if (result == null && default(T) == null)
        {
            // Only log this as we need to conform to the interface which doesn't have nullability constraints
            Console.WriteLine($"Warning: Acquire function returned null for key: {key}");
        }

        //and set in cache (if cache time is defined)
        if ((cacheTime ?? 30) > 0 && result != null)
            Set(key, result, cacheTime ?? 30);

        return result!;
    }

    /// <summary>
    /// Get All Keys
    /// </summary>
    /// <returns></returns>
    public IList<string> GetKeys() => _allKeys.Keys.ToList();

    /// <summary>
    /// Gets a value indicating whether the value associated with the specified key is cached
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <returns>True if item already is in cache; otherwise false</returns>
    public virtual bool IsSet(string key)
    {
        return _cache.TryGetValue(key, out object _);
    }

    /// <summary>
    /// Explicitly implementing the interface method
    /// </summary>
    bool IMemoryCacheManager.PerformActionWithLock(string key, TimeSpan expirationTime, Action action)
    {
        // Forward to our internal implementation
        return InternalPerformActionWithLock(key, expirationTime, action);
    }

    /// <summary>
    /// Internal implementation for exclusive memory lock
    /// </summary>
    private bool InternalPerformActionWithLock(string key, TimeSpan expirationTime, Action action)
    {
        //ensure that lock is acquired
        if (string.IsNullOrEmpty(key) || !_allKeys.TryAdd(key, true))
            return false;

        try
        {
            // Using a boxed primitive value which cannot be null
            _cache.Set(key, 1, GetMemoryCacheEntryOptions(expirationTime));

            //perform action
            action();

            return true;
        }
        finally
        {
            //release lock even if action fails
            Remove(key);
        }
    }

    /// <summary>
    /// Removes the value with the specified key from the cache
    /// </summary>
    /// <param name="key">Key of cached item</param>
    public virtual void Remove(string key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            _cache.Remove(RemoveKey(key));
        }
    }

    /// <summary>
    /// Adds the specified key and object to the cache
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <param name="data">Value for caching</param>
    /// <param name="cacheTime">Cache time in minutes</param>
    public virtual void Set(string key, object? data, int cacheTime)
    {
        if (data != null)
        {
            _cache.Set(AddKey(key), data, GetMemoryCacheEntryOptions(TimeSpan.FromMinutes(cacheTime)));
        }
    }

}