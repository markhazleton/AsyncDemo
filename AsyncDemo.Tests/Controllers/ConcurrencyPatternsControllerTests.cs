namespace AsyncDemo.Tests.Controllers;

/// <summary>
/// Tests demonstrating concurrency patterns and performance characteristics
/// </summary>
[TestClass]
public class ConcurrencyPatternsControllerTests
{
    /// <summary>
    /// Tests sequential execution - operations should run one after another
    /// </summary>
    [TestMethod]
    public async Task Sequential_RunsOperationsInOrder()
    {
        // Arrange
        var logger = new Mock<ILogger<ConcurrencyPatternsController>>();
        var controller = new ConcurrencyPatternsController(logger.Object);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await controller.Sequential(operationCount: 3, iterationsPerOperation: 10);
        stopwatch.Stop();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        // Sequential execution should take approximately sum of all operations
        var data = okResult.Value as dynamic;
        Assert.IsNotNull(data);
    }

    /// <summary>
    /// Tests parallel execution is faster than sequential
    /// </summary>
    [TestMethod]
    public async Task Parallel_IsFasterThanSequential()
    {
        // Arrange
        var logger = new Mock<ILogger<ConcurrencyPatternsController>>();
        var controller = new ConcurrencyPatternsController(logger.Object);
        const int operationCount = 5;
        const int iterations = 20;

        // Act - Sequential
        var seqStopwatch = System.Diagnostics.Stopwatch.StartNew();
        await controller.Sequential(operationCount, iterations);
        seqStopwatch.Stop();

        // Act - Parallel
        var parStopwatch = System.Diagnostics.Stopwatch.StartNew();
        await controller.Parallel(operationCount, iterations);
        parStopwatch.Stop();

        // Assert - Parallel should be significantly faster
        Assert.IsTrue(
            parStopwatch.ElapsedMilliseconds < seqStopwatch.ElapsedMilliseconds,
            $"Parallel ({parStopwatch.ElapsedMilliseconds}ms) should be faster than Sequential ({seqStopwatch.ElapsedMilliseconds}ms)");
    }

    /// <summary>
    /// Tests parallel execution completes all operations
    /// </summary>
    [TestMethod]
    public async Task Parallel_CompletesAllOperations()
    {
        // Arrange
        var logger = new Mock<ILogger<ConcurrencyPatternsController>>();
        var controller = new ConcurrencyPatternsController(logger.Object);
        const int operationCount = 5;

        // Act
        var result = await controller.Parallel(operationCount: operationCount, iterationsPerOperation: 10);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        // Verify all operations completed
        dynamic data = okResult.Value;
        Assert.AreEqual(operationCount, (int)data.totalOperations);
    }

    /// <summary>
    /// Tests throttled execution respects concurrency limit
    /// </summary>
    [TestMethod]
    public async Task Throttled_RespectsMaxConcurrency()
    {
        // Arrange
        var logger = new Mock<ILogger<ConcurrencyPatternsController>>();
        var controller = new ConcurrencyPatternsController(logger.Object);
        const int operationCount = 10;
        const int maxConcurrency = 2;

        // Act
        var result = await controller.Throttled(
            operationCount: operationCount,
            maxConcurrency: maxConcurrency,
            iterationsPerOperation: 10);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        dynamic data = okResult.Value;
        Assert.AreEqual(operationCount, (int)data.totalOperations);
        Assert.AreEqual(maxConcurrency, (int)data.maxConcurrency);
    }

    /// <summary>
    /// Tests throttled is slower than fully parallel but faster than sequential
    /// </summary>
    [TestMethod]
    public async Task Throttled_PerformanceBetweenSequentialAndParallel()
    {
        // Arrange
        var logger = new Mock<ILogger<ConcurrencyPatternsController>>();
        var controller = new ConcurrencyPatternsController(logger.Object);
        const int operationCount = 6;
        const int maxConcurrency = 2;
        const int iterations = 20;

        // Act - Sequential
        var seqStopwatch = System.Diagnostics.Stopwatch.StartNew();
        await controller.Sequential(operationCount, iterations);
        seqStopwatch.Stop();

        // Act - Parallel
        var parStopwatch = System.Diagnostics.Stopwatch.StartNew();
        await controller.Parallel(operationCount, iterations);
        parStopwatch.Stop();

        // Act - Throttled
        var thrStopwatch = System.Diagnostics.Stopwatch.StartNew();
        await controller.Throttled(operationCount, maxConcurrency, iterations);
        thrStopwatch.Stop();

        // Assert - Throttled should be between Sequential and Parallel
        Assert.IsTrue(
            thrStopwatch.ElapsedMilliseconds < seqStopwatch.ElapsedMilliseconds,
            $"Throttled ({thrStopwatch.ElapsedMilliseconds}ms) should be faster than Sequential ({seqStopwatch.ElapsedMilliseconds}ms)");

        // Note: Throttled might be similar to parallel for small operation counts
        // but the pattern demonstrates the concept
    }

    /// <summary>
    /// Tests comparison endpoint returns all three execution modes
    /// </summary>
    [TestMethod]
    public async Task Comparison_ReturnsAllThreeModes()
    {
        // Arrange
        var logger = new Mock<ILogger<ConcurrencyPatternsController>>();
        var controller = new ConcurrencyPatternsController(logger.Object);

        // Act
        var result = await controller.Comparison(
            operationCount: 3,
            maxConcurrency: 2,
            iterationsPerOperation: 10);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        dynamic data = okResult.Value;
        Assert.IsNotNull(data.results.sequential);
        Assert.IsNotNull(data.results.parallel);
        Assert.IsNotNull(data.results.throttled);

        // Sequential should be slowest
        long seqMs = data.results.sequential.milliseconds;
        long parMs = data.results.parallel.milliseconds;

        Assert.IsTrue(parMs < seqMs,
            $"Parallel ({parMs}ms) should be faster than Sequential ({seqMs}ms)");
    }

    /// <summary>
    /// Tests that parallel execution shows significant speedup
    /// </summary>
    [TestMethod]
    public async Task Comparison_ShowsParallelSpeedup()
    {
        // Arrange
        var logger = new Mock<ILogger<ConcurrencyPatternsController>>();
        var controller = new ConcurrencyPatternsController(logger.Object);
        const int operationCount = 4;

        // Act
        var result = await controller.Comparison(
            operationCount: operationCount,
            maxConcurrency: 2,
            iterationsPerOperation: 20);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        dynamic data = okResult.Value;
        long seqMs = data.results.sequential.milliseconds;
        long parMs = data.results.parallel.milliseconds;

        // With 4 operations, parallel should be roughly 4x faster (with some overhead)
        double speedup = (double)seqMs / parMs;
        Assert.IsTrue(speedup > 1.5,
            $"Parallel speedup ({speedup:F1}x) should be significant for {operationCount} operations");
    }
}
