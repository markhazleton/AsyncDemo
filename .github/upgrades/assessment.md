# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NET 9.0.

## Table of Contents

- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [AsyncDemo.Console\AsyncDemo.Console.csproj](#asyncdemoconsoleasyncdemoconsolecsproj)
  - [AsyncDemo.Tests\AsyncDemo.Tests.csproj](#asyncdemotestsasyncdemotestscsproj)
  - [AsyncDemo.Web\AsyncDemo.Web.csproj](#asyncdemowebasyncdemowebcsproj)
  - [AsyncDemo\AsyncDemo.csproj](#asyncdemoasyncdemocsproj)
  - [CancelAsyncWithToken\CancelAsyncWithToken.csproj](#cancelasyncwithtokencancelasyncwithtokencsproj)
  - [OpenWeatherMapClient\OpenWeatherMapClient.csproj](#openweathermapclientopenweathermapclientcsproj)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)


## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;AsyncDemo.csproj</b><br/><small>net9.0</small>"]
    P2["<b>📦&nbsp;AsyncDemo.Tests.csproj</b><br/><small>net9.0</small>"]
    P3["<b>📦&nbsp;AsyncDemo.Console.csproj</b><br/><small>net9.0</small>"]
    P4["<b>📦&nbsp;OpenWeatherMapClient.csproj</b><br/><small>net9.0</small>"]
    P5["<b>📦&nbsp;CancelAsyncWithToken.csproj</b><br/><small>net9.0</small>"]
    P6["<b>📦&nbsp;AsyncDemo.Web.csproj</b><br/><small>net9.0</small>"]
    P2 --> P1
    P3 --> P1
    P6 --> P1
    P6 --> P4
    click P1 "#asyncdemoasyncdemocsproj"
    click P2 "#asyncdemotestsasyncdemotestscsproj"
    click P3 "#asyncdemoconsoleasyncdemoconsolecsproj"
    click P4 "#openweathermapclientopenweathermapclientcsproj"
    click P5 "#cancelasyncwithtokencancelasyncwithtokencsproj"
    click P6 "#asyncdemowebasyncdemowebcsproj"

```

## Project Details

<a id="asyncdemoconsoleasyncdemoconsolecsproj"></a>
### AsyncDemo.Console\AsyncDemo.Console.csproj

#### Project Info

- **Current Target Framework:** net9.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 1
- **Dependants**: 0
- **Number of Files**: 3
- **Lines of Code**: 171

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["AsyncDemo.Console.csproj"]
        MAIN["<b>📦&nbsp;AsyncDemo.Console.csproj</b><br/><small>net9.0</small>"]
        click MAIN "#asyncdemoconsoleasyncdemoconsolecsproj"
    end
    subgraph downstream["Dependencies (1"]
        P1["<b>📦&nbsp;AsyncDemo.csproj</b><br/><small>net9.0</small>"]
        click P1 "#asyncdemoasyncdemocsproj"
    end
    MAIN --> P1

```

#### Project Package References

| Package | Type | Current Version | Suggested Version | Description |
| :--- | :---: | :---: | :---: | :--- |

<a id="asyncdemotestsasyncdemotestscsproj"></a>
### AsyncDemo.Tests\AsyncDemo.Tests.csproj

#### Project Info

- **Current Target Framework:** net9.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 1
- **Dependants**: 0
- **Number of Files**: 9
- **Lines of Code**: 241

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["AsyncDemo.Tests.csproj"]
        MAIN["<b>📦&nbsp;AsyncDemo.Tests.csproj</b><br/><small>net9.0</small>"]
        click MAIN "#asyncdemotestsasyncdemotestscsproj"
    end
    subgraph downstream["Dependencies (1"]
        P1["<b>📦&nbsp;AsyncDemo.csproj</b><br/><small>net9.0</small>"]
        click P1 "#asyncdemoasyncdemocsproj"
    end
    MAIN --> P1

```

#### Project Package References

| Package | Type | Current Version | Suggested Version | Description |
| :--- | :---: | :---: | :---: | :--- |
| coverlet.collector | Explicit | 6.0.4 |  | ✅Compatible |
| Microsoft.Extensions.Caching.Abstractions | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |
| Microsoft.Extensions.Caching.Memory | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |
| Microsoft.Extensions.Http | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |
| Microsoft.NET.Test.Sdk | Explicit | 18.0.0 |  | ✅Compatible |
| Moq | Explicit | 4.20.72 |  | ✅Compatible |
| MSTest.TestAdapter | Explicit | 4.0.1 |  | ✅Compatible |
| MSTest.TestFramework | Explicit | 4.0.1 |  | ✅Compatible |

<a id="asyncdemowebasyncdemowebcsproj"></a>
### AsyncDemo.Web\AsyncDemo.Web.csproj

#### Project Info

- **Current Target Framework:** net9.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** AspNetCore
- **Dependencies**: 2
- **Dependants**: 0
- **Number of Files**: 55
- **Lines of Code**: 3870

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["AsyncDemo.Web.csproj"]
        MAIN["<b>📦&nbsp;AsyncDemo.Web.csproj</b><br/><small>net9.0</small>"]
        click MAIN "#asyncdemowebasyncdemowebcsproj"
    end
    subgraph downstream["Dependencies (2"]
        P1["<b>📦&nbsp;AsyncDemo.csproj</b><br/><small>net9.0</small>"]
        P4["<b>📦&nbsp;OpenWeatherMapClient.csproj</b><br/><small>net9.0</small>"]
        click P1 "#asyncdemoasyncdemocsproj"
        click P4 "#openweathermapclientopenweathermapclientcsproj"
    end
    MAIN --> P1
    MAIN --> P4

```

#### Project Package References

| Package | Type | Current Version | Suggested Version | Description |
| :--- | :---: | :---: | :---: | :--- |
| Azure.Identity | Explicit | 1.17.0 |  | ✅Compatible |
| Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |
| Microsoft.Extensions.Http.Resilience | Explicit | 9.10.0 |  | ✅Compatible |
| Polly | Explicit | 8.6.4 |  | ✅Compatible |
| Swashbuckle.AspNetCore | Explicit | 9.0.6 |  | ✅Compatible |
| Westwind.AspNetCore.Markdown | Explicit | 3.24.0 |  | ✅Compatible |

<a id="asyncdemoasyncdemocsproj"></a>
### AsyncDemo\AsyncDemo.csproj

#### Project Info

- **Current Target Framework:** net9.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 3
- **Number of Files**: 12
- **Lines of Code**: 727

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (3)"]
        P2["<b>📦&nbsp;AsyncDemo.Tests.csproj</b><br/><small>net9.0</small>"]
        P3["<b>📦&nbsp;AsyncDemo.Console.csproj</b><br/><small>net9.0</small>"]
        P6["<b>📦&nbsp;AsyncDemo.Web.csproj</b><br/><small>net9.0</small>"]
        click P2 "#asyncdemotestsasyncdemotestscsproj"
        click P3 "#asyncdemoconsoleasyncdemoconsolecsproj"
        click P6 "#asyncdemowebasyncdemowebcsproj"
    end
    subgraph current["AsyncDemo.csproj"]
        MAIN["<b>📦&nbsp;AsyncDemo.csproj</b><br/><small>net9.0</small>"]
        click MAIN "#asyncdemoasyncdemocsproj"
    end
    P2 --> MAIN
    P3 --> MAIN
    P6 --> MAIN

```

#### Project Package References

| Package | Type | Current Version | Suggested Version | Description |
| :--- | :---: | :---: | :---: | :--- |
| Microsoft.Extensions.Caching.Abstractions | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |
| Microsoft.Extensions.Http | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |

<a id="cancelasyncwithtokencancelasyncwithtokencsproj"></a>
### CancelAsyncWithToken\CancelAsyncWithToken.csproj

#### Project Info

- **Current Target Framework:** net9.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 0
- **Dependants**: 0
- **Number of Files**: 3
- **Lines of Code**: 118

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["CancelAsyncWithToken.csproj"]
        MAIN["<b>📦&nbsp;CancelAsyncWithToken.csproj</b><br/><small>net9.0</small>"]
        click MAIN "#cancelasyncwithtokencancelasyncwithtokencsproj"
    end

```

#### Project Package References

| Package | Type | Current Version | Suggested Version | Description |
| :--- | :---: | :---: | :---: | :--- |

<a id="openweathermapclientopenweathermapclientcsproj"></a>
### OpenWeatherMapClient\OpenWeatherMapClient.csproj

#### Project Info

- **Current Target Framework:** net9.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 1
- **Number of Files**: 14
- **Lines of Code**: 864

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P6["<b>📦&nbsp;AsyncDemo.Web.csproj</b><br/><small>net9.0</small>"]
        click P6 "#asyncdemowebasyncdemowebcsproj"
    end
    subgraph current["OpenWeatherMapClient.csproj"]
        MAIN["<b>📦&nbsp;OpenWeatherMapClient.csproj</b><br/><small>net9.0</small>"]
        click MAIN "#openweathermapclientopenweathermapclientcsproj"
    end
    P6 --> MAIN

```

#### Project Package References

| Package | Type | Current Version | Suggested Version | Description |
| :--- | :---: | :---: | :---: | :--- |
| Azure.Identity | Explicit | 1.17.0 |  | ✅Compatible |
| Microsoft.AspNet.WebApi.Client | Explicit | 6.0.0 |  | ✅Compatible |
| Microsoft.EntityFrameworkCore.SqlServer | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.Tools | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |
| Microsoft.Extensions.Http | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |
| Microsoft.Extensions.Logging.Debug | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |
| System.Drawing.Common | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |
| System.Text.Json | Explicit | 9.0.10 | 10.0.0 | NuGet package upgrade is recommended |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| Azure.Identity | 1.17.0 |  | [AsyncDemo.Web.csproj](#asyncdemowebcsproj)<br/>[OpenWeatherMapClient.csproj](#openweathermapclientcsproj) | ✅Compatible |
| coverlet.collector | 6.0.4 |  | [AsyncDemo.Tests.csproj](#asyncdemotestscsproj) | ✅Compatible |
| Microsoft.AspNet.WebApi.Client | 6.0.0 |  | [OpenWeatherMapClient.csproj](#openweathermapclientcsproj) | ✅Compatible |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.10 | 10.0.0 | [OpenWeatherMapClient.csproj](#openweathermapclientcsproj) | NuGet package upgrade is recommended |
| Microsoft.EntityFrameworkCore.Tools | 9.0.10 | 10.0.0 | [OpenWeatherMapClient.csproj](#openweathermapclientcsproj) | NuGet package upgrade is recommended |
| Microsoft.Extensions.Caching.Abstractions | 9.0.10 | 10.0.0 | [AsyncDemo.Tests.csproj](#asyncdemotestscsproj)<br/>[AsyncDemo.csproj](#asyncdemocsproj) | NuGet package upgrade is recommended |
| Microsoft.Extensions.Caching.Memory | 9.0.10 | 10.0.0 | [AsyncDemo.Tests.csproj](#asyncdemotestscsproj) | NuGet package upgrade is recommended |
| Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore | 9.0.10 | 10.0.0 | [AsyncDemo.Web.csproj](#asyncdemowebcsproj) | NuGet package upgrade is recommended |
| Microsoft.Extensions.Http | 9.0.10 | 10.0.0 | [AsyncDemo.Tests.csproj](#asyncdemotestscsproj)<br/>[AsyncDemo.csproj](#asyncdemocsproj)<br/>[OpenWeatherMapClient.csproj](#openweathermapclientcsproj) | NuGet package upgrade is recommended |
| Microsoft.Extensions.Http.Resilience | 9.10.0 |  | [AsyncDemo.Web.csproj](#asyncdemowebcsproj) | ✅Compatible |
| Microsoft.Extensions.Logging.Debug | 9.0.10 | 10.0.0 | [OpenWeatherMapClient.csproj](#openweathermapclientcsproj) | NuGet package upgrade is recommended |
| Microsoft.NET.Test.Sdk | 18.0.0 |  | [AsyncDemo.Tests.csproj](#asyncdemotestscsproj) | ✅Compatible |
| Moq | 4.20.72 |  | [AsyncDemo.Tests.csproj](#asyncdemotestscsproj) | ✅Compatible |
| MSTest.TestAdapter | 4.0.1 |  | [AsyncDemo.Tests.csproj](#asyncdemotestscsproj) | ✅Compatible |
| MSTest.TestFramework | 4.0.1 |  | [AsyncDemo.Tests.csproj](#asyncdemotestscsproj) | ✅Compatible |
| Polly | 8.6.4 |  | [AsyncDemo.Web.csproj](#asyncdemowebcsproj) | ✅Compatible |
| Swashbuckle.AspNetCore | 9.0.6 |  | [AsyncDemo.Web.csproj](#asyncdemowebcsproj) | ✅Compatible |
| System.Drawing.Common | 9.0.10 | 10.0.0 | [OpenWeatherMapClient.csproj](#openweathermapclientcsproj) | NuGet package upgrade is recommended |
| System.Text.Json | 9.0.10 | 10.0.0 | [OpenWeatherMapClient.csproj](#openweathermapclientcsproj) | NuGet package upgrade is recommended |
| Westwind.AspNetCore.Markdown | 3.24.0 |  | [AsyncDemo.Web.csproj](#asyncdemowebcsproj) | ✅Compatible |

