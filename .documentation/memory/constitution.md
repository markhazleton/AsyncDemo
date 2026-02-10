<!--
SYNC IMPACT REPORT - AsyncSpark Constitution v1.0.0
Generated: 2026-02-09
Change Type: Initial Ratification

VERSION CHANGE: none → 1.0.0
- Initial constitution formalized from codebase analysis

PRINCIPLES ADDED:
1. Modern .NET Standards (MANDATORY)
2. Async/Await Best Practices (MANDATORY)
3. Testing Standards (NON-NEGOTIABLE)
4. Dependency Injection & Architecture (MANDATORY)
5. Resilience, Documentation & Logging (MANDATORY)

SECTIONS ADDED:
- Security & Observability Standards
- Development Workflow & CI/CD
- Governance (Amendment Process & Compliance)

TEMPLATES REQUIRING UPDATES:
✅ plan-template.md - Review for async/testing/DI alignment
✅ spec-template.md - Ensure requirements sections match principles
✅ tasks-template.md - Add task types for testing/documentation/resilience
⚠️ commands/*.md - Remove agent-specific references if present

IMMEDIATE ACTIONS REQUIRED:
1. Create .editorconfig file (Principle VIII gap)
2. Enable 80% code coverage enforcement in CI/CD (Principle III gap)
3. Audit ConfigureAwait(false) usage across all library code (Principle II gap)
4. Add XML documentation to all public APIs (Principle VII gap)

FOLLOW-UP:
- Run /speckit.site-audit to validate codebase compliance
- Update GitHub Actions workflow to enforce coverage threshold
- Create .editorconfig with C# standards
-->

# AsyncSpark Constitution

## Core Principles

### I. Modern .NET Standards (MANDATORY)

All projects MUST target .NET 10 or later with nullable reference types enabled and implicit usings. Libraries MUST be self-contained with clear purpose and interfaces.

**Non-Negotiable Requirements:**
- All `.csproj` files MUST specify `<TargetFramework>net10.0</TargetFramework>` or later
- All projects MUST enable `<Nullable>enable</Nullable>` for compile-time null safety
- All projects MUST enable `<ImplicitUsings>enable</ImplicitUsings>` for reduced boilerplate
- Modern C# features (primary constructors, file-scoped namespaces) SHOULD be used for clean, concise code

**Evidence**: All 5 project files (AsyncSpark.csproj, AsyncSpark.Web.csproj, AsyncSpark.Tests.csproj, AsyncSpark.Console.csproj, AsyncSpark.Weather.csproj) consistently use .NET 10 and nullable types.

**Rationale**: Nullable reference types prevent null reference exceptions at compile time, reducing production bugs. .NET 10 provides the latest performance improvements, security updates, and language features. Consistent framework usage across all projects ensures maintainability.

### II. Async/Await Best Practices (MANDATORY)

All asynchronous code MUST follow best practices to prevent deadlocks, enable proper cancellation, and ensure responsive applications.

**Non-Negotiable Requirements:**
- Library code MUST use `.ConfigureAwait(false)` on all awaited tasks to prevent deadlocks
- All async methods MUST accept a `CancellationToken` parameter and thread it through the call chain
- Never use blocking calls: `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` are FORBIDDEN
- Controllers MUST pass `CancellationToken` from HTTP context through entire operation chain
- Always check cancellation using `cancellationToken.IsCancellationRequested` or `ThrowIfCancellationRequested()`

**Evidence**: ConfigureAwait(false) found in 13 locations including AsyncMockService.cs, FireAndForgetUtility.cs, and RemoteController.cs. CancellationToken parameters consistently used across async methods.

**Rationale**: ConfigureAwait(false) in library code prevents deadlocks when called from synchronization contexts. CancellationToken enables graceful cancellation of long-running operations, improving responsiveness and resource management. These patterns are essential for production-quality async code.

### III. Testing Standards (NON-NEGOTIABLE)

Comprehensive testing with MSTest framework and Moq is mandatory. Minimum 80% code coverage MUST be achieved and enforced in CI/CD.

**Non-Negotiable Requirements:**
- All tests MUST use MSTest framework with `[TestClass]` and `[TestMethod]` attributes
- Moq MUST be used for mocking dependencies in unit tests
- Minimum 80% code coverage MUST be achieved and enforced in GitHub Actions CI/CD pipeline
- Tests MUST follow Arrange-Act-Assert (AAA) pattern for clarity and consistency
- Each test MUST have a clear, descriptive name indicating scenario and expected behavior
- New features MUST include corresponding tests before PR approval

**Evidence**: All 9 test files use MSTest + Moq pattern consistently. AsyncSpark.Tests.csproj includes MSTest.TestFramework, MSTest.TestAdapter, Moq, and coverlet.collector packages.

**Rationale**: Standardized testing framework ensures consistency across team. 80% coverage provides strong confidence in code quality while remaining achievable. MSTest integrates seamlessly with Visual Studio and GitHub Actions. Moq enables effective isolation of units under test.

### IV. Dependency Injection & Architecture (MANDATORY)

All services MUST implement interfaces and use constructor injection. Decorator pattern MUST be used for cross-cutting concerns.

**Non-Negotiable Requirements:**
- All services MUST implement an interface (e.g., `IHttpGetCallService`, `IWeatherService`, `IMemoryCacheManager`)
- Constructor injection MUST be used for all dependencies
- All constructor parameters MUST validate for null using `ArgumentNullException.ThrowIfNull()` or null-coalescing throw
- Services MUST be registered in DI container with appropriate lifetime (Scoped/Singleton/Transient)
- Primary constructors SHOULD be used for concise service definitions
- Logging, caching, and telemetry MUST be implemented as decorators wrapping base services
- Core business logic MUST NOT contain cross-cutting concerns directly
- Decorators MUST implement the same interface as the service they wrap

**Evidence**: 100% of services implement interfaces. HttpGetCallServiceTelemetry demonstrates decorator pattern. Program.cs shows consistent DI registration. BulkCallsController uses primary constructor pattern.

**Rationale**: Interfaces enable testability through mocking and promote loose coupling. Constructor injection makes dependencies explicit and testable. Null validation prevents runtime failures. Decorator pattern keeps business logic clean and allows cross-cutting concerns to be added/removed without modifying core code.

### V. Resilience, Documentation & Logging (MANDATORY)

All external calls MUST use Polly resilience policies. All public APIs MUST have XML documentation. Structured logging with ILogger<T> is mandatory.

**Resilience Requirements:**
- All external HTTP calls MUST use Polly retry and timeout policies
- Retry policies MUST use exponential backoff with jitter to prevent thundering herd
- All external calls MUST have reasonable timeouts configured
- SemaphoreSlim SHOULD be used to limit concurrent operations when needed

**Documentation Requirements:**
- All public classes, methods, properties, and parameters MUST have XML documentation (`/// <summary>`)
- Controllers MUST document all action methods with parameter descriptions
- XML documentation files MUST be generated and included in builds
- Complex business logic SHOULD include inline comments explaining "why" not "what"

**Logging Requirements:**
- All services and controllers MUST use `ILogger<T>` for logging
- Log messages MUST use structured logging with named parameters (e.g., `{ErrorMessage}`)
- Appropriate log levels MUST be used (Trace, Debug, Information, Warning, Error, Critical)
- Sensitive data (passwords, tokens, PII) MUST NOT be logged

**Evidence**: Polly packages in AsyncSpark.Web.csproj. PollyController demonstrates retry patterns. BulkCallsController shows SemaphoreSlim throttling. GenerateDocumentationFile enabled in AsyncSpark.Web.csproj. ILogger<T> used consistently across all services and controllers.

**Rationale**: Transient failures are common in distributed systems; Polly provides battle-tested resilience patterns. XML documentation powers IntelliSense and API documentation tools like Scalar, improving developer experience. Structured logging enables better log aggregation, searching, and analysis in production environments.

## Security & Observability Standards

**Security (MANDATORY):**
- User Secrets MUST be used for local development secrets (never commit secrets to source control)
- Azure Key Vault SHOULD be used for production secrets
- All HTTP calls MUST use HTTPS in production environments
- Security headers MUST be configured: X-Content-Type-Options, X-Frame-Options, X-XSS-Protection
- User input MUST be validated and sanitized to prevent injection attacks

**Observability (MANDATORY):**
- Health check endpoints MUST be implemented at `/health` for monitoring
- Application status endpoints SHOULD be available at `/status` for operational visibility
- All long-running operations SHOULD be instrumented with timing metrics (Stopwatch)
- API documentation MUST be available via Scalar at `/scalar/v1`
- OpenAPI specification MUST be generated and available at `/openapi/v1.json`

**Code Formatting (MANDATORY):**
- A `.editorconfig` file MUST exist at the repository root defining C# formatting standards
- All code MUST follow the rules defined in `.editorconfig`
- IDE/editor MUST be configured to respect `.editorconfig` settings
- Code formatting SHOULD be validated in CI/CD pipeline before merge

**Evidence**: Security headers configured in Program.cs. Health checks registered. Scalar integration in CustomScalarExtensions. UserSecretsId in AsyncSpark.Web.csproj. ⚠️ .editorconfig file NOT YET CREATED (immediate action required).

**Rationale**: Security-in-depth prevents common vulnerabilities. Observability enables rapid incident response and performance optimization. Consistent code formatting prevents merge conflicts and improves code review efficiency. Health checks enable automated monitoring and alerting.

## Development Workflow & CI/CD

**GitHub Actions CI/CD (MANDATORY):**
- GitHub Actions MUST be used for continuous integration and deployment
- All PRs MUST pass build and test validation before merge approval
- Code coverage MUST be measured and 80% threshold enforced before deployment
- Automated deployment MUST occur on merge to main branch
- Azure Web Apps MUST be used for production hosting
- Build process MUST include npm dependencies for AsyncSpark.Web frontend assets

**Code Review Requirements (MANDATORY):**
- All code changes MUST go through Pull Request review process
- At least one approval REQUIRED before merge
- All PR reviews MUST verify compliance with constitution principles
- Constitution violations SHOULD be flagged during code review with specific principle references
- Repeated violations MAY result in PR rejection
- `/speckit.pr-review` SHOULD be used to automate constitution compliance checks

**Testing Gates (MANDATORY):**
- All tests MUST pass before PR merge
- New features MUST include corresponding unit tests
- Breaking changes MUST include migration guide and version bump justification
- Regressions MUST include tests that would have caught the bug

**Evidence**: .github/workflows/main_asyncspark.yml implements automated build, test, npm install, and Azure deployment. Workflow includes Node.js 20 setup and npm ci step. Deploy occurs automatically on main branch merge.

**Rationale**: Automated CI/CD prevents broken code from reaching production and enables rapid, confident deployments. Code review catches issues early and spreads knowledge across team. Testing gates ensure quality remains high as codebase grows. Azure provides reliable, scalable hosting with integrated deployment tooling.

## Governance

**Amendment Process:**
This constitution supersedes all other development practices and guidelines. Changes require careful consideration and team alignment.

- Any team member MAY propose constitution amendments via Pull Request to `.documentation/memory/constitution.md`
- Amendments MUST include rationale explaining why the change improves the project
- Amendments MUST include evidence from codebase or external sources supporting the change
- Amendments REQUIRE code review approval before merge (minimum one approval from team)
- All amendments MUST update the version number using semantic versioning (see below)
- All amendments MUST update the "Last Amended" date to current date (YYYY-MM-DD format)

**Versioning Policy:**
- MAJOR version: Backward incompatible changes (e.g., removing principle, changing MUST to MUST NOT)
- MINOR version: Additive changes (e.g., new principle added, expanding guidance, SHOULD → MUST)
- PATCH version: Clarifications, wording improvements, typo fixes, non-semantic refinements

**Compliance Enforcement:**
- All PR reviews MUST verify compliance with constitution principles
- Use principle numbers (I, II, III, etc.) when citing violations in review comments
- Constitution violations SHOULD be flagged during code review with educational context
- Repeated violations MAY result in PR rejection until principles are followed
- `/speckit.pr-review` SHOULD be used to automate constitution compliance checks
- Complexity or deviations MUST be justified with documented rationale in PR description

**Review Schedule:**
- Constitution SHOULD be reviewed quarterly for relevance and accuracy
- Major technology upgrades (e.g., .NET version bumps) MAY trigger ad-hoc review
- Team retrospectives MAY propose amendments based on learnings from production issues
- New team members MUST review constitution during onboarding

**Guidance:**
- Use `.github/copilot-instructions.md` for AI-assisted development guidance aligned with constitution
- Use Speckit templates (spec, plan, tasks) for feature development workflows
- Run `/speckit.site-audit` periodically to validate codebase compliance with principles

**Version**: 1.0.0 | **Ratified**: 2026-02-09 | **Last Amended**: 2026-02-09
