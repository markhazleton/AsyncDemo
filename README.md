# AsyncSpark

Production-ready reference implementation for async/await patterns in .NET 10 with constitution-driven development practices.

[![Build and deploy ASP.Net Core app to Azure Web App - AsyncSpark](https://github.com/markhazleton/AsyncSpark/actions/workflows/main_AsyncSpark.yml/badge.svg)](https://github.com/markhazleton/AsyncSpark/actions/workflows/main_AsyncSpark.yml)

## Overview

AsyncSpark demonstrates enterprise-grade async programming patterns with enforced quality standards, automated compliance auditing, and interactive API documentation. Built on .NET 10 with 80% code coverage enforcement in CI/CD.

**Live Demo**: [asyncspark.azurewebsites.net](https://asyncspark.azurewebsites.net/) | **API Docs**: [/scalar/v1](https://asyncspark.azurewebsites.net/scalar/v1)

## Key Features

- **Constitution-Driven Development**: Formalized coding standards enforced through automated audits
- **80% Code Coverage Enforcement**: CI/CD fails if coverage drops below threshold
- **Modern .NET 10**: Nullable reference types, implicit usings, file-scoped namespaces, primary constructors
- **Async Best Practices**: ConfigureAwait(false) in all library code, proper CancellationToken usage, no blocking calls
- **Resilience Patterns**: Polly integration via WebSpark.HttpClientUtility with retry, timeout, and circuit breaker policies
- **Clean Architecture**: Dependency injection, decorator pattern, interface-based design
- **Interactive API Documentation**: Scalar-powered API explorer with live testing
- **SpecKit Development Workflow**: Constitution validation, automated audits, PR reviews, feature planning

## Project Structure

```
AsyncSpark/                    # Core library - async utilities and services
AsyncSpark.Web/               # ASP.NET Core web application with API endpoints
AsyncSpark.Weather/           # Weather service integration demonstrating external API patterns
AsyncSpark.Tests/             # MSTest + Moq unit tests
AsyncSpark.Console/           # Console application demos
.documentation/               # Constitution, guides, templates, audit reports
.github/                      # CI/CD workflows, SpecKit agents, Copilot instructions
.editorconfig                # C# code style enforcement
```

## Learning Objectives

Each pattern links to specific code implementing the technique:

### 1. ConfigureAwait(false) in Library Code
**Implementation**: [HttpGetCallService.cs:24](AsyncSpark/HttpGetCall/HttpGetCallService.cs#L24), [OpenWeatherMapWeatherService.cs:31](AsyncSpark.Weather/Services/OpenWeatherMapWeatherService.cs#L31)  
**Pattern**: All library async methods use `.ConfigureAwait(false)` to prevent deadlocks when consumed by UI threads.  
**Test**: [HttpGetCallServiceTests.cs](AsyncSpark.Tests/HttpGetCall/HttpGetCallServiceTests.cs)

### 2. CancellationToken Threading
**Implementation**: [RemoteController.cs:26](AsyncSpark.Web/Controllers/Api/RemoteController.cs#L26), [AsyncMockService.cs:112](AsyncSpark/Services/AsyncMockService.cs#L112)  
**Pattern**: CancellationToken passed from HTTP request through entire call chain for graceful cancellation.  
**Test**: [CancellationPatternsControllerTests.cs](AsyncSpark.Tests/Controllers/CancellationPatternsControllerTests.cs)

### 3. Task.WhenAll for Parallel Execution
**Implementation**: [BulkCallsController.cs:62](AsyncSpark.Web/Controllers/BulkCallsController.cs#L62), [ConcurrencyPatternsController.cs:149](AsyncSpark.Web/Controllers/Api/ConcurrencyPatternsController.cs#L149)  
**Pattern**: Execute multiple async operations concurrently and wait for all completions.  
**Test**: [ConcurrencyPatternsControllerTests.cs](AsyncSpark.Tests/Controllers/ConcurrencyPatternsControllerTests.cs)

### 4. SemaphoreSlim for Throttling
**Implementation**: [BulkCallsController.cs:28](AsyncSpark.Web/Controllers/BulkCallsController.cs#L28)  
**Pattern**: Limit concurrent operations with SemaphoreSlim to prevent resource exhaustion.  
**Test**: Demonstrated in concurrency controller tests

### 5. Polly Resilience Policies
**Implementation**: [Program.cs:26](AsyncSpark.Web/Program.cs#L26) via WebSpark.HttpClientUtility  
**Pattern**: Retry with exponential backoff, timeouts, circuit breakers for external API calls.  
**Test**: [PollyResilienceTests.cs](AsyncSpark.Tests/Resilience/PollyResilienceTests.cs)

### 6. Decorator Pattern for Cross-Cutting Concerns
**Implementation**: [HttpGetCallServiceTelemetry.cs](AsyncSpark/HttpGetCall/HttpGetCallServiceTelemetry.cs), [Program.cs:74](AsyncSpark.Web/Program.cs#L74)  
**Pattern**: Wrap services with decorators for logging, telemetry, caching without modifying business logic.  
**Constitution**: Principle IV mandates decorator pattern for cross-cutting concerns

### 7. Fire-and-Forget with Safety
**Implementation**: [FireAndForgetUtility.cs](AsyncSpark/FireAndForget/FireAndForgetUtility.cs)  
**Pattern**: Safely execute fire-and-forget tasks with exception handling and logging.  
**Test**: [AsyncFireAndForgetUtilityTests.cs](AsyncSpark.Tests/FireAndForget/AsyncFireAndForgetUtilityTests.cs)

### 8. No Blocking Calls
**All Controllers**: Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` - always `await`.  
**Enforcement**: .editorconfig rule CA2007 treats missing async patterns as errors.

## Constitution-Driven Development

AsyncSpark uses a formalized constitution to enforce coding standards and architectural patterns.

**Constitution**: [.documentation/memory/constitution.md](.documentation/memory/constitution.md)  
**Version**: 1.0.0 | **Ratified**: 2026-02-09

### Five Core Principles

1. **Modern .NET Standards**: .NET 10+, nullable reference types, implicit usings, file-scoped namespaces
2. **Async/Await Best Practices**: ConfigureAwait(false) in libraries, CancellationToken threading, no blocking calls
3. **Testing Standards**: MSTest + Moq, 80% code coverage enforced in CI/CD, AAA pattern
4. **Dependency Injection & Architecture**: Interface-based design, constructor injection with null validation, decorator pattern
5. **Resilience, Documentation & Logging**: Polly policies, XML documentation, structured logging with ILogger<T>

### Automated Compliance

- **Site Audit**: `/speckit.site-audit` - Comprehensive codebase audit against constitution
- **PR Review**: `/speckit.pr-review` - Constitution-aware pull request reviews
- **CI/CD Enforcement**: GitHub Actions fails builds on coverage < 80% or async violations
- **Audit Reports**: [.documentation/copilot/audit/](.documentation/copilot/audit/)

**Latest Audit**: [2026-02-09_results.md](.documentation/copilot/audit/2026-02-09_results.md) - 90% Constitution compliance

## API Documentation with Scalar

Interactive API documentation using Scalar v2.12.36 with OpenAPI 3.1 specification.

**Endpoints**:
- Interactive Docs: `/scalar/v1`
- OpenAPI Spec: `/openapi/v1.json`
- Live Demo: [asyncspark.azurewebsites.net/scalar/v1](https://asyncspark.azurewebsites.net/scalar/v1)

**Features**: Dark/light mode, auto-generated code examples (C#, JavaScript, Python, cURL, HTTP), tag-based organization, live request testing, mobile-friendly

### API Endpoint Categories

**Cancellation Patterns** (`/api/cancellation/*`) - Timeout, cancellation, linked tokens, cleanup patterns  
**Concurrency Patterns** (`/api/concurrency/*`) - Sequential vs parallel vs throttled execution comparisons  
**Remote Operations** (`/api/remote/*`) - Timeout and retry pattern demonstrations  
**Weather Patterns** (`/api/weather/*`) - Real OpenWeatherMap API integration with caching, retry, parallel calls  
**Health & Status** (`/health`, `/status`, `/api/status`) - Health checks, application status, build version

**Detailed Documentation**: [API_LEARNING_GUIDE.md](.documentation/guides/API_LEARNING_GUIDE.md)

## Technology Stack

**Framework**: .NET 10.0  
**Web**: ASP.NET Core MVC + Web API  
**Testing**: MSTest 4.1.0, Moq 4.20.72, coverlet.collector 6.0.4 (80% coverage enforced)  
**Resilience**: Polly 8.6.5, Microsoft.Extensions.Http.Resilience 10.2.0  
**HTTP Utilities**: WebSpark.HttpClientUtility 2.2.0 (caching, telemetry, resilience)  
**API Documentation**: Scalar.AspNetCore 2.12.36, Microsoft.AspNetCore.OpenApi 10.0.2  
**Azure**: Azure.Identity 1.17.1 (Key Vault integration)  
**UI**: WebSpark.Bootswatch 1.34.0 (theme switcher)  
**Markdown**: Westwind.AspNetCore.Markdown 3.31.0  
**External API**: OpenWeatherMap

**Development Tools**: User Secrets, Docker support (Linux), npm integration for frontend assets, .editorconfig
## Getting Started

### Prerequisites
- .NET 10 SDK
- Node.js 20+ (for npm build)
- (Optional) OpenWeatherMap API key for weather endpoints

### Quick Start

```bash
# Clone repository
git clone https://github.com/markhazleton/AsyncSpark.git
cd AsyncSpark

# Install npm dependencies
cd AsyncSpark.Web
npm ci

# Configure OpenWeatherMap API key (optional)
dotnet user-secrets set "OpenWeatherMapApiKey" "your-api-key-here"
cd ..

# Build solution
dotnet build --configuration Release

# Run tests with coverage
dotnet test --configuration Release --collect:"XPlat Code Coverage"

# Run web application
dotnet run --project AsyncSpark.Web
```

### Explore the Application

1. **API Documentation**: Navigate to `https://localhost:5001/scalar/v1`
2. **Test Endpoints**: Use Scalar UI to execute live API requests
3. **Health Check**: Visit `/health` for application health status
4. **Status Page**: Visit `/status` for build version and configuration

### Run Constitution Audit

```bash
# In GitHub Copilot Chat
/speckit.site-audit
```

Generates comprehensive audit report in `.documentation/copilot/audit/` validating:
- Async/await best practices compliance
- ConfigureAwait(false) usage in library code
- Code coverage percentages
- XML documentation coverage
- Dependency vulnerabilities
- Constitution principle adherence

## CI/CD Pipeline

**GitHub Actions**: [.github/workflows/main_asyncspark.yml](.github/workflows/main_asyncspark.yml)

**Build Steps**:
1. Checkout code
2. Setup .NET 10
3. Setup Node.js 20
4. Install npm dependencies (`npm ci`)
5. Build solution (`dotnet build`)
6. **Run tests with coverage collection**
7. **Validate 80% coverage threshold** (build fails if < 80%)
8. Publish artifacts
9. Deploy to Azure App Service

**Enforcement**: Pull requests cannot merge without passing coverage threshold.

## SpecKit Development Workflow

SpecKit agents provide constitution-aware development assistance:

- `/speckit.constitution` - Create or update coding standards
- `/speckit.site-audit` - Comprehensive codebase audit
- `/speckit.pr-review` - Constitution-aware PR reviews
- `/speckit.specify` - Create feature specifications
- `/speckit.plan` - Generate implementation plans
- `/speckit.tasks` - Break down work into actionable tasks
- `/speckit.implement` - Execute tasks with validation
- `/speckit.quickfix` - Rapid fixes with constitution validation

**Documentation**: [.github/agents/](.github/agents/), [.github/prompts/](.github/prompts/)

## Additional Resources

**Async Programming**:
- [Cancellation Tokens in C#](https://markhazleton.com/cancellation-token.html) - Mark Hazleton
- [Async and Decorator Pattern](https://markhazleton.com/decorator-pattern-http-client.html) - Mark Hazleton
- [Await and UI Deadlocks](https://devblogs.microsoft.com/pfxteam/await-and-ui-and-deadlocks-oh-my/) - Microsoft
- [Stop Calling .Result](https://montemagno.com/c-sharp-developers-stop-calling-dot-result/) - James Montemagno

**Polly Resilience**:
- [Polly Project](http://www.thepollyproject.org/) - Official documentation
- [Retry and Circuit Breaker Patterns](https://medium.com/@therealjordanlee/retry-circuit-breaker-patterns-in-c-with-polly-9aa24c5fe23a)

**API Documentation**:
- [Scalar GitHub](https://github.com/scalar/scalar) - Modern API reference tool
- [Microsoft OpenAPI Docs](https://learn.microsoft.com/aspnet/core/fundamentals/openapi)

## License

MIT License - See [LICENSE.txt](LICENSE.txt) for details

## Author

**Mark Hazleton**  
Website: [markhazleton.com](https://markhazleton.com)  
Email: mark.hazleton@controlorigins.com  
GitHub: [@markhazleton](https://github.com/markhazleton)

---

**AsyncSpark** - Production-ready async patterns for .NET 10 | Built with constitution-driven development | API documentation powered by Scalar

