# AsyncDemo API Learning Guide

This guide helps you navigate the AsyncDemo API documentation to learn async/await patterns systematically.

## 📚 API Organization by Learning Concept

The API endpoints are organized into **tags** in Scalar that represent progressive async/await concepts. Work through them in order for the best learning experience.

### 1️⃣ Async Basics (`/api/weather/*`)

**Start here** to understand fundamental async patterns with real-world weather API calls.

| Endpoint | What It Teaches | Try This |
|----------|----------------|----------|
| `GET /api/weather/slow` | Basic async WITHOUT timeout protection | Call it and see how it could hang |
| `GET /api/weather/with-timeout` | Async WITH timeout and cancellation | Set `timeoutSeconds=2`, then cancel request |
| `GET /api/weather/with-retry` | Polly retry policies with exponential backoff | Check `retryCount` in response |
| `GET /api/weather/multiple` | `Task.WhenAll` for concurrent operations | Request 5+ cities, note speed |

**Key Takeaways:**
- Always protect external calls with timeouts
- Wire cancellation tokens through the call chain
- Use `Task.WhenAll` for parallel independent operations
- Polly retries handle transient failures automatically

---

### 2️⃣ Cancellation Patterns (`/api/cancellation/*`)

Learn how to properly implement and handle cancellation in async operations.

| Endpoint | What It Teaches | Try This |
|----------|----------------|----------|
| `GET /api/cancellation/no-cancellation` | Anti-pattern: operation CAN'T be cancelled | Cancel request, check logs - keeps running |
| `GET /api/cancellation/with-token` | Proper cancellation token usage | Cancel request, see immediate stop |
| `GET /api/cancellation/with-timeout` | Combining timeout + HTTP cancellation | Trigger timeout vs user cancellation |
| `GET /api/cancellation/with-cleanup` | Resource cleanup despite cancellation | Verify cleanup executes in logs |

**Key Takeaways:**
- Accept `CancellationToken` parameters (ASP.NET auto-wires from HTTP)
- Use `CreateLinkedTokenSource()` to combine cancellation sources
- Distinguish timeout (408) from user cancellation (499)
- Always cleanup resources in `finally` blocks

---

### 3️⃣ Concurrency & Parallelism (`/api/concurrency/*`)

Understand the performance implications of sequential vs parallel execution.

| Endpoint | What It Teaches | Try This |
|----------|----------------|----------|
| `GET /api/concurrency/sequential` | Sequential async (anti-pattern for independent ops) | 5 operations = ~5x longer |
| `GET /api/concurrency/parallel` | `Task.WhenAll` parallel execution | 5 operations = ~1x time (5x faster!) |
| `GET /api/concurrency/throttled` | `SemaphoreSlim` for controlled concurrency | Set `maxConcurrency=2` for 10 ops |
| `GET /api/concurrency/comparison` | Side-by-side performance comparison | See all three approaches compared |

**Key Takeaways:**
- Sequential: `await` in loop = operations run one at a time
- Parallel: `Task.WhenAll` = operations run concurrently
- Throttled: `SemaphoreSlim` = controlled concurrency for resource limits
- Choose based on: Are operations independent? Are there resource limits?

---

### 4️⃣ Resilience & Timeouts (`/api/remote/*`)

Real-world patterns for handling failures and timeouts in distributed systems.

| Endpoint | What It Teaches | Try This |
|----------|----------------|----------|
| `POST /api/remote/Results` | Timeout-based cancellation | Set `MaxTimeMS` < `LoopCount` processing time |

**Key Takeaways:**
- Use `CancellationTokenSource(TimeSpan)` for operation timeouts
- Return 408 (Request Timeout) for timeout scenarios
- Pass cancellation tokens through entire call chain

---

### 5️⃣ Monitoring & Health (`/status`)

Check application health and status.

| Endpoint | What It Teaches |
|----------|----------------|
| `GET /status` | Application status endpoint |
| `GET /health` | Health check endpoint |

---

## 🎯 Learning Paths

### Path 1: Complete Beginner
1. Start with `/api/weather/slow` to see basic async
2. Compare with `/api/weather/with-timeout` to learn protection
3. Try `/api/cancellation/with-token` to understand cancellation
4. Experiment with `/api/concurrency/comparison` for performance

### Path 2: Intermediate Developer
1. Study `/api/weather/with-retry` for Polly resilience
2. Understand `/api/cancellation/with-timeout` for linked tokens
3. Master `/api/concurrency/throttled` for resource management
4. Review `/api/remote/Results` for production patterns

### Path 3: Advanced Patterns
1. Examine source code of decorator pattern in `Program.cs:68-82`
2. Study `AsyncMockService.cs:80-109` for `TaskCompletionSource` pattern
3. Analyze `BulkCallsController.cs:28-58` for `SemaphoreSlim` usage
4. Review Polly retry policies in `PollyController.cs:32-53`

---

## 🧪 Testing Guide

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test category
dotnet test --filter Category=Cancellation
dotnet test --filter Category=Concurrency
dotnet test --filter Category=Resilience
```

### Key Test Files

| Test File | What It Demonstrates |
|-----------|---------------------|
| `CancellationPatternsControllerTests.cs` | Cancellation, timeouts, cleanup |
| `ConcurrencyPatternsControllerTests.cs` | Sequential vs parallel performance |
| `PollyResilienceTests.cs` | Retry policies, transient failures, jitter |

### Simulating Failures

The tests in `PollyResilienceTests.cs` show how to:
- ✅ Simulate transient failures (fail 2x, then succeed)
- ✅ Test permanent failures (always fail)
- ✅ Verify retry exhaustion
- ✅ Test cancellation during retries
- ✅ Combine timeout + retry policies
- ✅ Use jitter to prevent thundering herd

**Example: Testing Transient Failures**

```csharp
[TestMethod]
public async Task RetryPolicy_SucceedsAfterTransientFailures()
{
    var retryPolicy = Policy
        .Handle<HttpRequestException>()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt =>
                TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100));

    var result = await retryPolicy.ExecuteAsync(
        async () => await TransientFailureService());

    Assert.AreEqual("Success", result);
}
```

---

## 📖 Additional Resources

### Code References
- **Avoiding Deadlocks**: [AsyncMockService.cs:96](AsyncDemo/Services/AsyncMockService.cs#L96)
- **Cancellation Tokens**: [RemoteController.cs:58-78](AsyncDemo.Web/Controllers/Api/RemoteController.cs#L58-L78)
- **Task.WhenAll**: [BulkCallsController.cs:62](AsyncDemo.Web/Controllers/BulkCallsController.cs#L62)
- **Polly Retries**: [PollyController.cs:32-53](AsyncDemo.Web/Controllers/PollyController.cs#L32-L53)
- **Semaphore Throttling**: [BulkCallsController.cs:28-58](AsyncDemo.Web/Controllers/BulkCallsController.cs#L28-L58)
- **Decorator Pattern**: [Program.cs:68-82](AsyncDemo.Web/Program.cs#L68-L82)

### Articles
See the [main README](README.md#articles--resources) for curated articles on:
- Async/await best practices
- Cancellation tokens
- Polly resilience patterns
- Avoiding deadlocks
- Performance optimization

---

## 🚀 Quick Start

1. **Run the application:**
   ```bash
   dotnet run --project AsyncDemo.Web
   ```

2. **Open Scalar documentation:**
   - Navigate to `https://localhost:5001/scalar/v1`
   - Browse endpoints organized by learning concept tags

3. **Try an endpoint:**
   - Click on `/api/weather/with-timeout`
   - Set `location=London` and `timeoutSeconds=5`
   - Click "Send" and observe the response

4. **Experiment:**
   - Try different timeout values
   - Cancel requests mid-flight
   - Compare sequential vs parallel performance
   - Trigger retries by simulating failures

5. **Read the code:**
   - Click the code reference links in endpoint descriptions
   - See the pattern implementation
   - Understand the tradeoffs

---

## ❓ Common Questions

### Q: When should I use Task.WhenAll vs sequential await?
**A:** Use `Task.WhenAll` when operations are **independent** and can run in parallel. Use sequential await when operations **depend on each other**.

### Q: How do I choose between parallel and throttled execution?
**A:** Use **throttled** when you have resource limits (connection pools, API rate limits). Use **parallel** when resources are unlimited.

### Q: Should I always use cancellation tokens?
**A:** Yes! At minimum, wire the HTTP cancellation token through. Add timeout tokens for operations that could hang.

### Q: When should I retry vs fail fast?
**A:** Retry **transient** failures (network blips, 503 errors). Fail fast on **permanent** failures (validation errors, 404s, 401s).

### Q: What's the difference between 408 and 499 status codes?
**A:**
- **408 Request Timeout**: Server-side timeout expired
- **499 Client Closed Request**: Client cancelled the request

---

## 📝 Next Steps

1. Work through the endpoints in order (1 → 5)
2. Read the code references for patterns you use
3. Run the tests to see patterns in action
4. Try the "Try This" suggestions for each endpoint
5. Apply the patterns in your own projects

**Remember:** The best way to learn async/await is to experiment, observe the behavior, and understand the tradeoffs.

Happy learning! 🎓
