# Upgrade Plan: .NET 9 to .NET 10

## Overview
This plan outlines the steps to upgrade the AsyncDemo solution from .NET 9 to .NET 10. The solution contains 6 projects with various dependencies and NuGet packages that need to be updated.

## Projects to Upgrade
1. **AsyncDemo.csproj** - Core class library (3 dependants)
2. **AsyncDemo.Tests.csproj** - Test project
3. **AsyncDemo.Console.csproj** - Console application
4. **AsyncDemo.Web.csproj** - ASP.NET Core Razor Pages application
5. **OpenWeatherMapClient.csproj** - Weather API client library (1 dependant)
6. **CancelAsyncWithToken.csproj** - Console application

## Upgrade Strategy

### Phase 1: Pre-Upgrade Validation
- Validate .NET 10 SDK installation
- Check global.json compatibility
- Create backup/branch for rollback

### Phase 2: Update Project Files (Topological Order)
Update projects in dependency order to minimize build errors:
1. AsyncDemo.csproj (no dependencies)
2. OpenWeatherMapClient.csproj (no dependencies)
3. CancelAsyncWithToken.csproj (no dependencies)
4. AsyncDemo.Tests.csproj (depends on AsyncDemo)
5. AsyncDemo.Console.csproj (depends on AsyncDemo)
6. AsyncDemo.Web.csproj (depends on AsyncDemo and OpenWeatherMapClient)

### Phase 3: NuGet Package Updates
Update the following packages to version 10.0.0:
- Microsoft.Extensions.Caching.Abstractions (2 projects)
- Microsoft.Extensions.Caching.Memory (1 project)
- Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore (1 project)
- Microsoft.Extensions.Http (3 projects)
- Microsoft.Extensions.Logging.Debug (1 project)
- Microsoft.EntityFrameworkCore.SqlServer (1 project)
- Microsoft.EntityFrameworkCore.Tools (1 project)
- System.Drawing.Common (1 project)
- System.Text.Json (1 project)

### Phase 4: Build and Test
1. Build each project incrementally
2. Run test suite (AsyncDemo.Tests)
3. Verify web application functionality
4. Address any breaking changes

### Phase 5: Verification
- Review build output for warnings
- Run full test suite
- Manual testing of critical functionality
- Review breaking changes documentation

## Potential Breaking Changes
- Review .NET 10 breaking changes documentation
- Entity Framework Core 10 changes
- ASP.NET Core 10 changes
- System.Text.Json serialization changes

## Rollback Plan
- Solution is on branch `upgrade-to-NET10`
- Can revert commits if issues arise
- Original code on main branch