# AsyncDemo
Various demos, tips and tricks for using async in C#

[![Build and deploy ASP.Net Core app to Azure Web App - asyncdemo](https://github.com/markhazleton/AsyncDemo/actions/workflows/main_asyncdemo.yml/badge.svg)](https://github.com/markhazleton/AsyncDemo/actions/workflows/main_asyncdemo.yml)

Sample code for making and canceling Async Calls with beautiful API documentation powered by **[Scalar](https://github.com/scalar/scalar)**.

## ?? Features

- **?? Modern API Documentation** - Beautiful, interactive API docs using Scalar
- **? Async Programming Patterns** - Real-world examples of async/await best practices
- **??? Resilience with Polly** - Retry policies, circuit breakers, and fallback strategies
- **?? Decorator Pattern** - Clean, maintainable code architecture
- **??? OpenWeather Integration** - Live API integration examples
- **?? .NET 10** - Built on the latest .NET framework

## ?? API Documentation with Scalar

This project features stunning API documentation powered by **Scalar**. After running the application, explore:

- **Interactive API Docs**: Visit `/scalar/v1` for the beautiful Scalar UI
- **OpenAPI Specification**: Access `/openapi/v1.json` for the spec
- **Live Demo**: [https://asyncdemo.azurewebsites.net/scalar/v1](https://asyncdemo.azurewebsites.net/scalar/v1)

Scalar provides:
- ? Beautiful, modern UI with dark/light mode
- ?? Auto-generated code examples in multiple languages (C#, JavaScript, Python, cURL, and more)
- ? Fast, responsive interface
- ?? Advanced search and filtering
- ?? Mobile-friendly design

## ?? Articles & Resources

### Async Programming Articles
- [Cancellation Tokens in C#](https://markhazleton.com/cancellation-token.html)
- [Async and Decorator Pattern](https://markhazleton.com/decorator-pattern-http-client.html)
- [Cancel Asynchronous Operations](https://johnthiriet.com/cancel-asynchronous-operation-in-csharp/)
- [Going Async with Async Command](https://johnthiriet.com/mvvm-going-async-with-async-command/)

## ?? Hosting
Live demo hosted at [https://asyncdemo.azurewebsites.net/](https://asyncdemo.azurewebsites.net/)
## ?? Additional Resources

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

## ?? Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/markhazleton/AsyncDemo.git
   cd AsyncDemo
   ```

2. **Build the solution**
   ```bash
   dotnet build
   ```

3. **Run the web application**
   ```bash
   dotnet run --project AsyncDemo.Web
   ```

4. **Explore the API documentation**
   - Open your browser to `https://localhost:5001/scalar/v1`
   - View the beautiful Scalar-powered API documentation
   - Try out the interactive examples

## ?? Technologies

- **.NET 10** - Latest .NET framework
- **ASP.NET Core** - Web framework
- **Scalar** - Beautiful API documentation
- **Polly** - Resilience and transient-fault-handling
- **OpenWeatherMap API** - Weather data integration

## ?? License

MIT License - see LICENSE file for details

## ?? Author

**Mark Hazleton**
- Website: [markhazleton.com](https://markhazleton.com)
- Email: mark.hazleton@controlorigins.com

---

**Built with ?? on .NET 10 | Powered by Scalar API Documentation**

