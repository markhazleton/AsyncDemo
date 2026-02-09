# AsyncSpark
Various demos, tips and tricks for using async in C#

[![Build and deploy ASP.Net Core app to Azure Web App - AsyncSpark](https://github.com/markhazleton/AsyncSpark/actions/workflows/main_AsyncSpark.yml/badge.svg)](https://github.com/markhazleton/AsyncSpark/actions/workflows/main_AsyncSpark.yml)

Sample code for making and canceling Async Calls with beautiful API documentation powered by **[Scalar](https://github.com/scalar/scalar)**.

## Learning Objectives

This demo teaches critical async/await patterns through focused, real-world examples. Each concept links directly to specific code you can explore:

### 1. Avoiding Deadlocks
**Where to look**: [AsyncMockService.cs:96](AsyncSpark/Services/AsyncMockService.cs#L96) - Notice the `.ConfigureAwait(false)` usage
**What to learn**: How to prevent deadlocks in library code by not capturing the synchronization context
**What can go wrong**: Omitting `ConfigureAwait(false)` in library code can cause deadlocks when called from UI threads. Never use `.Result` or `.Wait()` - always use `await`.

### 2. Cancellation Tokens End-to-End
**Where to look**: [RemoteController.cs:58-78](AsyncSpark.Web/Controllers/Api/RemoteController.cs#L58-L78) and [AsyncMockService.cs:80-109](AsyncSpark/Services/AsyncMockService.cs#L80-L109)
**What to learn**: How to pass cancellation tokens from HTTP requests through the entire call chain
**What can go wrong**: Ignoring cancellation means wasting resources on work that nobody needs anymore. Always wire `CancellationToken` through your async methods.

### 3. Task.WhenAll for Concurrency
**Where to look**: [BulkCallsController.cs:62](AsyncSpark.Web/Controllers/BulkCallsController.cs#L62)
**What to learn**: How to execute multiple async operations concurrently and wait for all to complete
**What can go wrong**: Using a `foreach` loop with `await` inside runs operations sequentially. Use `Task.WhenAll` to run them in parallel.

### 4. Timeouts with Polly
**Where to look**: [PollyController.cs:32-53](AsyncSpark.Web/Controllers/PollyController.cs#L32-L53)
**What to learn**: How to implement retry policies with exponential backoff and jitter
**What can go wrong**: Without timeouts and retries, transient failures become permanent failures. Always protect external calls.

### 5. Semaphore for Throttling
**Where to look**: [BulkCallsController.cs:28-58](AsyncSpark.Web/Controllers/BulkCallsController.cs#L28-L58)
**What to learn**: How to limit concurrent operations using `SemaphoreSlim`
**What can go wrong**: Unbounded concurrency can overwhelm downstream services or exhaust connection pools.

### 6. Decorator Pattern for Cross-Cutting Concerns
**Where to look**: [Program.cs:68-82](AsyncSpark.Web/Program.cs#L68-L82)
**What to learn**: How to compose behavior (logging, caching) around async operations cleanly
**What can go wrong**: Mixing cross-cutting concerns into business logic creates unmaintainable code.

## Features

- **[?] Modern API Documentation** - Beautiful, interactive API docs using Scalar
- **[>] Async Programming Patterns** - Real-world examples of async/await best practices
- **[!] Resilience with Polly** - Retry policies, circuit breakers, and fallback strategies
- **[+] Decorator Pattern** - Clean, maintainable code architecture
- **[*] OpenWeather Integration** - Live API integration examples
- **[#] .NET 10** - Built on the latest .NET framework

## API Documentation with Scalar

This project features stunning API documentation powered by **Scalar**, organized by async learning concepts. After running the application, explore:

- **Interactive API Docs**: Visit `/scalar/v1` for the beautiful Scalar UI
- **OpenAPI Specification**: Access `/openapi/v1.json` for the spec
- **Live Demo**: [https://AsyncSpark.azurewebsites.net/scalar/v1](https://AsyncSpark.azurewebsites.net/scalar/v1)
- **Learning Guide**: See [API_LEARNING_GUIDE.md](API_LEARNING_GUIDE.md) for a structured learning path

### API Endpoints by Learning Concept

The API is organized into **5 learning modules**, each teaching specific async patterns:

1. **Async Basics** (`/api/weather/*`) - Timeouts, cancellation, retry, and parallel calls with real weather data
2. **Cancellation Patterns** (`/api/cancellation/*`) - Proper cancellation token usage, linked tokens, and cleanup
3. **Concurrency & Parallelism** (`/api/concurrency/*`) - Sequential vs parallel vs throttled execution
4. **Resilience & Timeouts** (`/api/remote/*`) - Production-ready timeout and retry patterns
5. **Monitoring & Health** (`/status`, `/health`) - Application status and health checks

**See the [API Learning Guide](API_LEARNING_GUIDE.md) for detailed endpoint documentation, learning paths, and testing guide.**

Scalar provides:
- [?] Beautiful, modern UI with dark/light mode
- [>] Auto-generated code examples in multiple languages (C#, JavaScript, Python, cURL, and more)
- [?] Fast, responsive interface
- [+] Advanced search and filtering
- [+] Mobile-friendly design
- [>] **Tags for concept-based navigation** - endpoints grouped by learning objective

## Articles & Resources

### Async Programming Articles
- [Cancellation Tokens in C#](https://markhazleton.com/cancellation-token.html)
- [Async and Decorator Pattern](https://markhazleton.com/decorator-pattern-http-client.html)
- [Cancel Asynchronous Operations](https://johnthiriet.com/cancel-asynchronous-operation-in-csharp/)
- [Going Async with Async Command](https://johnthiriet.com/mvvm-going-async-with-async-command/)

## Hosting
Live demo hosted at [https://AsyncSpark.azurewebsites.net/](https://AsyncSpark.azurewebsites.net/)
## Additional Resources

### Async & Await Best Practices
- [Await and UI and Deadlocks](https://devblogs.microsoft.com/pfxteam/await-and-ui-and-deadlocks-oh-my/?WT.mc_id=friends-0000-jamont)
- [Stop Calling .Result](https://montemagno.com/c-sharp-developers-stop-calling-dot-result/)
- [Async Task with Timeout](https://codereview.stackexchange.com/questions/113108/async-task-with-timeout)
- [Cancel Async Tasks After a Period of Time](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/cancel-async-tasks-after-a-period-of-time)
- [Crafting a Task.TimeoutAfter Method](https://devblogs.microsoft.com/pfxteam/crafting-a-task-timeoutafter-method/)
- [Asynchronously Wait for Task with Timeout](https://stackoverflow.com/questions/4238345/asynchronously-wait-for-taskt-to-complete-with-timeout)

### Polly Resilience Resources
- [Polly Project](http://www.thepollyproject.org/) - .NET resilience and transient-fault-handling library
- [Using Context to Obtain Retry Count](https://www.stevejgordon.co.uk/polly-using-context-to-obtain-retry-count-diagnostics)
- [Retry and Circuit Breaker Patterns](https://medium.com/@therealjordanlee/retry-circuit-breaker-patterns-in-c-with-polly-9aa24c5fe23a)

### API Documentation
- [Scalar Documentation](https://github.com/scalar/scalar) - Modern API documentation tool
- [Scalar NuGet Package](https://www.nuget.org/packages/Scalar.AspNetCore)
- [Microsoft OpenAPI Documentation](https://learn.microsoft.com/aspnet/core/fundamentals/openapi)

## Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/markhazleton/AsyncSpark.git
   cd AsyncSpark
   ```

2. **Build the solution**
   ```bash
   dotnet build
   ```

3. **Run the web application**
   ```bash
   dotnet run --project AsyncSpark.Web
   ```

4. **Explore the API documentation**
   - Open your browser to `https://localhost:5001/scalar/v1`
   - View the beautiful Scalar-powered API documentation organized by learning concepts
   - Try out the interactive examples in each category

5. **Follow the learning path**
   - Read the [API Learning Guide](API_LEARNING_GUIDE.md) for structured learning
   - Start with "Async Basics" endpoints
   - Progress through cancellation, concurrency, and resilience patterns
   - Review the code references linked in endpoint descriptions

6. **Run the tests**
   ```bash
   dotnet test
   ```
   - Explore tests that demonstrate resilience patterns
   - See simulated transient failures with Polly
   - Learn from cancellation and concurrency test examples

## Technologies

- **.NET 10** - Latest .NET framework
- **ASP.NET Core** - Web framework
- **Scalar** - Beautiful API documentation
- **Polly** - Resilience and transient-fault-handling
- **OpenWeatherMap API** - Weather data integration

## License

MIT License - see LICENSE file for details

## Author

**Mark Hazleton**
- Website: [markhazleton.com](https://markhazleton.com)
- Email: mark.hazleton@controlorigins.com

---

**Built with [love] on .NET 10 | Powered by Scalar API Documentation**

