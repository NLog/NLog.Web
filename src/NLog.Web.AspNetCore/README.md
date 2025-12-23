# NLog.Web.AspNetCore

[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=reliability_rating)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=sqale_rating)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=vulnerabilities)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 

Integrates NLog as [Logging provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers) for the ASP.NET Core platform, by just calling `UseNLog()` with the application host-builder.

Providing features like:

- Enrich logging output with additional details from active HttpContext using NLog [LayoutRenderers](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore), by just updating the NLog configuration.
- Supports middleware injection for [HTTP Request Logging](https://github.com/NLog/NLog.Web/wiki/HTTP-Request-Logging) and [HTTP Response Logging](https://github.com/NLog/NLog.Web/wiki/HTTP-Response-Body-Capture).
- Load NLog configuration from [appsettings.json](https://github.com/NLog/NLog.Extensions.Logging/wiki/NLog-configuration-with-appsettings.json)
- Capture [structured message properties](https://github.com/NLog/NLog.Extensions.Logging/wiki/NLog-properties-with-Microsoft-Extension-Logging) from the [Microsoft ILogger](https://github.com/NLog/NLog.Extensions.Logging/wiki/NLog-GetCurrentClassLogger-and-Microsoft-ILogger)
- Capture [scope context properties](https://github.com/NLog/NLog/wiki/ScopeProperty-Layout-Renderer) from the Microsoft ILogger `BeginScope`
- Routing logging output to multiple destinations via the available NLog [Targets](https://nlog-project.org/config/?tab=targets)
- Rendering logging output into standard formats like JSON, CVS, W3C ELF and XML using NLog [Layouts](https://nlog-project.org/config/?tab=layouts).
- Contributions are always welcome, by creating a [pull request](https://github.com/NLog/NLog.Web/pulls).

Supported platforms:

- ASP.NET Core 6, 7, 8, 9 and 10
- ASP.NET Core 2, .NET Standard 2.0 and .NET 4.6.2+

Register NLog as logging provider:
```csharp
builder.Logging.ClearProviders();
builder.Host.UseNLog();
```

If logging is needed before the host building, then one can use fluent setup:
```csharp
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
```

Alternative include NLog.Web.AspNetCore extension in the NLog.config file:

```xml
<extensions>
    <add assembly="NLog.Web.AspNetCore"/>
</extensions>
```

Useful Links:

- [Home Page](https://nlog-project.org/)
- [Change Log](https://github.com/NLog/NLog.Web/releases)
- [Getting started with ASP.NET Core 6](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6)
- [ASP.NET Core Layout Renderers](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore)
- [Logging Troubleshooting](https://github.com/NLog/NLog/wiki/Logging-troubleshooting)
- [Have a question?](https://stackoverflow.com/questions/tagged/nlog)
