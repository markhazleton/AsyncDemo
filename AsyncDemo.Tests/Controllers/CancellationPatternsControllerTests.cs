namespace AsyncDemo.Tests.Controllers;

/// <summary>
/// Tests demonstrating cancellation patterns and timeout handling
/// </summary>
[TestClass]
public class CancellationPatternsControllerTests
{
    /// <summary>
    /// Tests that operations without cancellation support continue running even after timeout
    /// </summary>
    [TestMethod]
    public async Task NoCancellation_CompletesSuccessfully()
    {
        // Arrange
        var logger = new Mock<ILogger<CancellationPatternsController>>();
        var controller = new CancellationPatternsController(logger.Object);

        // Act
        var result = await controller.NoCancellation(iterations: 10);

        // Assert
        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    /// <summary>
    /// Tests that operations with cancellation support can be cancelled
    /// </summary>
    [TestMethod]
    public async Task WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        var logger = new Mock<ILogger<CancellationPatternsController>>();
        var controller = new CancellationPatternsController(logger.Object);
        var cts = new CancellationTokenSource();

        // Act - Cancel immediately
        cts.Cancel();
        var result = await controller.WithCancellationToken(iterations: 100, cts.Token);

        // Assert - Should return 499 (Client Closed Request)
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(499, statusCodeResult.StatusCode);
    }

    /// <summary>
    /// Tests that operations complete successfully when not cancelled
    /// </summary>
    [TestMethod]
    public async Task WithCancellationToken_CompletesWhenNotCancelled()
    {
        // Arrange
        var logger = new Mock<ILogger<CancellationPatternsController>>();
        var controller = new CancellationPatternsController(logger.Object);
        var cts = new CancellationTokenSource();

        // Act
        var result = await controller.WithCancellationToken(iterations: 10, cts.Token);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    /// <summary>
    /// Tests timeout behavior - operation should timeout if it takes too long
    /// </summary>
    [TestMethod]
    public async Task WithTimeout_TimesOutWhenTooSlow()
    {
        // Arrange
        var logger = new Mock<ILogger<CancellationPatternsController>>();
        var controller = new CancellationPatternsController(logger.Object);

        // Act - Set very short timeout for operation that takes longer
        var result = await controller.WithTimeout(
            iterations: 200,
            timeoutSeconds: 1,
            CancellationToken.None);

        // Assert - Should return 408 (Request Timeout)
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(408, statusCodeResult.StatusCode);
    }

    /// <summary>
    /// Tests that operations complete within timeout window
    /// </summary>
    [TestMethod]
    public async Task WithTimeout_CompletesWithinTimeout()
    {
        // Arrange
        var logger = new Mock<ILogger<CancellationPatternsController>>();
        var controller = new CancellationPatternsController(logger.Object);

        // Act - Give plenty of time for small operation
        var result = await controller.WithTimeout(
            iterations: 10,
            timeoutSeconds: 30,
            CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    /// <summary>
    /// Tests HTTP cancellation takes precedence over timeout
    /// </summary>
    [TestMethod]
    public async Task WithTimeout_HttpCancellationTakesPrecedence()
    {
        // Arrange
        var logger = new Mock<ILogger<CancellationPatternsController>>();
        var controller = new CancellationPatternsController(logger.Object);
        var cts = new CancellationTokenSource();

        // Act - Cancel via HTTP token before timeout
        cts.Cancel();
        var result = await controller.WithTimeout(
            iterations: 100,
            timeoutSeconds: 30,
            cts.Token);

        // Assert - Should return 499 (Client Closed Request), not 408
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(499, statusCodeResult.StatusCode);
    }

    /// <summary>
    /// Tests that cleanup executes even when cancelled
    /// </summary>
    [TestMethod]
    public async Task WithCleanup_ExecutesCleanupOnCancellation()
    {
        // Arrange
        var logger = new Mock<ILogger<CancellationPatternsController>>();
        var controller = new CancellationPatternsController(logger.Object);
        var cts = new CancellationTokenSource();

        // Act - Cancel the operation
        cts.Cancel();
        var result = await controller.WithCleanup(iterations: 100, cts.Token);

        // Assert - Cleanup should have executed (check logs)
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(499, statusCodeResult.StatusCode);

        // Verify cleanup was logged
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Cleanup executed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Tests that cleanup executes on successful completion
    /// </summary>
    [TestMethod]
    public async Task WithCleanup_ExecutesCleanupOnSuccess()
    {
        // Arrange
        var logger = new Mock<ILogger<CancellationPatternsController>>();
        var controller = new CancellationPatternsController(logger.Object);

        // Act
        var result = await controller.WithCleanup(iterations: 10, CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        // Verify cleanup was logged
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Cleanup executed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
