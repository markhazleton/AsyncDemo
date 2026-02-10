# AsyncSpark Copilot Instructions

## Constitution Compliance (MANDATORY)

**BEFORE making ANY code changes**, you MUST review and comply with the AsyncSpark Constitution located at `/.documentation/memory/constitution.md`.

All code generation, modifications, and refactoring MUST adhere to the following principles:

### I. Modern .NET Standards
- Target .NET 10+ with nullable reference types and implicit usings enabled
- Use modern C# features (primary constructors, file-scoped namespaces)

### II. Async/Await Best Practices
- Use `.ConfigureAwait(false)` in all library code
- Always accept and thread `CancellationToken` through async operations
- NEVER use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`

### III. Testing Standards
- Include unit tests using MSTest framework and Moq for all new features
- Maintain minimum 80% code coverage
- Follow Arrange-Act-Assert (AAA) pattern

### IV. Dependency Injection & Architecture
- All services MUST implement interfaces
- Use constructor injection with null validation
- Apply decorator pattern for cross-cutting concerns (logging, caching, telemetry)

### V. Resilience, Documentation & Logging
- Use Polly for all external HTTP calls with retry and timeout policies
- Include XML documentation (`/// <summary>`) for all public APIs
- Use structured logging with `ILogger<T>`

### Security & Observability
- Use User Secrets for local development, never commit secrets
- Validate and sanitize all user input
- Include health check endpoints
- Generate OpenAPI specs and Scalar documentation

## Code Review Checklist

When generating or modifying code, verify:
- [ ] Constitution principles I-V are followed
- [ ] Nullable reference types are properly handled
- [ ] Async methods use ConfigureAwait(false) and CancellationToken
- [ ] Services implement interfaces and use constructor injection
- [ ] XML documentation is included for public APIs
- [ ] Appropriate logging is added with structured parameters
- [ ] Unit tests are included or updated
- [ ] No secrets or sensitive data are hardcoded

## Documentation Organization

All generated documentation MUST follow this structure:
- **Copilot session notes**: `/.documentation/copilot/session-{YYYY-MM-DD}/`
- **Domain-specific docs**: `/.documentation/{domain}/` (e.g., guides, troubleshooting, migrations)
- **README.md**: Root level ONLY (exception to the rule)

## Azure Development

- @azure Rule - Use Azure Best Practices: When generating code for Azure, running terminal commands for Azure, or performing operations related to Azure, invoke your `azure_development-get_best_practices` tool if available.

## Reference Links

- Constitution: `/.documentation/memory/constitution.md`
- Spec Template: `/.documentation/templates/spec-template.md`
- Plan Template: `/.documentation/templates/plan-template.md`
- Tasks Template: `/.documentation/templates/tasks-template.md`