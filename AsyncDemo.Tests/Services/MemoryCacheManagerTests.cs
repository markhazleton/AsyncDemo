using Microsoft.Extensions.Caching.Memory;
using System;
using Moq;
using AsyncDemo.Services;

namespace AsyncDemo.Tests.Services;


[TestClass]
public class MemoryCacheManagerTests
{
    private MemoryCacheManager _memoryCacheManager;
    private IMemoryCache _memoryCache;

    [TestInitialize]
    public void SetUp()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _memoryCacheManager = new MemoryCacheManager(_memoryCache);
    }

    [TestMethod]
    public void Set_ShouldAddItemToCache()
    {
        string key = "testKey";
        string value = "testValue";

        _memoryCacheManager.Set(key, value, 30);

        Assert.IsTrue(_memoryCacheManager.IsSet(key));
        Assert.AreEqual(value, _memoryCache.Get<string>(key));
    }

    [TestMethod]
    public void Get_ShouldReturnCachedItem()
    {
        string key = "testKey";
        string expectedValue = "testValue";
        _memoryCacheManager.Set(key, expectedValue, 30);

        var result = _memoryCacheManager.Get<string>(key, () => "defaultValue");

        Assert.AreEqual(expectedValue, result);
    }

    [TestMethod]
    public void Remove_ShouldRemoveItemFromCache()
    {
        string key = "testKey";
        _memoryCacheManager.Set(key, "testValue",30);
        _memoryCacheManager.Remove(key);

        Assert.IsFalse(_memoryCacheManager.IsSet(key));
    }

    [TestMethod]
    public void Clear_ShouldClearAllCacheItems()
    {
        _memoryCacheManager.Set("key1", "value1", 30);
        _memoryCacheManager.Set("key2", "value2", 30);

        _memoryCacheManager.Clear();

        Assert.IsFalse(_memoryCacheManager.IsSet("key1"));
        Assert.IsFalse(_memoryCacheManager.IsSet("key2"));
    }

    [TestMethod]
    public void PerformActionWithLock_ShouldPerformAction()
    {
        string key = "lockKey";
        bool actionPerformed = false;
        Action action = () => { actionPerformed = true; };

        bool result = _memoryCacheManager.PerformActionWithLock(key, TimeSpan.FromMinutes(1), action);

        Assert.IsTrue(result);
        Assert.IsTrue(actionPerformed);
    }

    // Additional tests can be added here to cover more methods and scenarios

    [TestCleanup]
    public void CleanUp()
    {
        _memoryCache.Dispose();
    }
}
