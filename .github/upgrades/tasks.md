# .NET 9 to .NET 10 Upgrade Tasks

## Overview

This scenario upgrades the AsyncDemo solution (6 projects) from .NET 9 to .NET 10, updating project files and NuGet packages, addressing breaking changes, and validating with builds and tests. Tasks are organized for automatable execution, referencing plan sections for details.

**Progress**: 4/5 tasks complete (80%) ![80%](https://progress-bar.xyz/80)

## Tasks

### [✓] TASK-001: Validate prerequisites *(Completed: 2025-11-12 09:09)*
**References**: Plan §Phase 1

- [✓] (1) Validate .NET 10 SDK installation
- [✓] (2) Check global.json compatibility with .NET 10 SDK
- [✓] (3) Prerequisites confirmed (**Verify**)

### [✓] TASK-002: Update project files to .NET 10 (topological order) *(Completed: 2025-11-12 09:10)*
**References**: Plan §Phase 2

- [✓] (1) Update TargetFramework to net10.0 for all projects in topological order per Plan §Phase 2
- [✓] (2) Project files updated (**Verify**)

### [✓] TASK-003: Update NuGet packages to version 10.0.0 *(Completed: 2025-11-12 09:11)*
**References**: Plan §Phase 3

- [✓] (1) Update all listed NuGet packages to 10.0.0 per Plan §Phase 3 - COMPLETED
- [✓] (2) Restore dependencies for all projects - COMPLETED
- [✓] (3) Packages updated and restored - VERIFIED (Warning about System.Text.Json noted)

### [✓] TASK-004: Build and test solution *(Completed: 2025-11-12 09:12)*
**References**: Plan §Phase 4, Plan §Phase 5

- [✓] (1) Build each project incrementally per Plan §Phase 4 - COMPLETED
- [✓] (2) Run test suite in AsyncDemo.Tests project - COMPLETED (11 tests passed)
- [✓] (3) Address any breaking changes identified during build/test - COMPLETED (No breaking changes found)
- [✓] (4) Solution builds with 0 errors and all tests pass - VERIFIED

### [ ] TASK-005: Final verification and commit
**References**: Plan §Phase 5, Plan §Rollback Plan

- [ ] (1) Review build output for warnings
- [ ] (2) Review breaking changes documentation for .NET 10, EF Core 10, ASP.NET Core 10, System.Text.Json
- [ ] (3) Commit all upgrade changes with message: "Complete .NET 10 upgrade"
- [ ] (4) Changes committed successfully (**Verify**)