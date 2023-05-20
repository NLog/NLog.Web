# NLog.Web.AspNetCore

[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=reliability_rating)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=sqale_rating)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=vulnerabilities)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 

Integrates NLog as [Logging provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers) for the ASP.NET Core platform, by just calling `UseNLog()` with the application host-builder.

Providing features like:

- Enrich logging output with additional details from active HttpContext using NLog [LayoutRenderers](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore), by just updating the NLog configuration.
- Supports middleware injection for [HTTP Request Logging](https://github.com/NLog/NLog.Web/wiki/HTTP-Request-Logging) and [HTTP Response Logging](https://github.com/NLog/NLog.Web/wiki/HTTP-Response-Body-Capture).
- Routing logging output to multiple destinations via the available NLog [Targets](https://nlog-project.org/config/?tab=targets)
- Rendering logging output into standard formats like JSON, CVS, W3C ELF and XML using NLog [Layouts](https://nlog-project.org/config/?tab=layouts).
- Contributions are always welcome, by creating a [pull request](https://github.com/NLog/NLog.Web/pulls).

Supported platforms:

- For ASP.NET Core 6, .NET 6
- For ASP.NET Core 5, .NET 5
- For ASP.NET Core 3, .NET Core 3.1
- For ASP.NET Core 2, .NET Standard 2.0 and .NET 4.6.1+

Registration of NLog.Web.AspNetCore in the NLog.config file:

```xml
<extensions>
    <add assembly="NLog.Web.AspNetCore"/>
</extensions>
```

Registration of NLog.Web.AspNetCore can also be performed with fluent setup:

```csharp
NLog.LogManager.Setup().LoadConfigurationFromAppSettings();
```

Useful Links

- [Home Page](https://nlog-project.org/)
- [Change Log](https://github.com/NLog/NLog.Web/releases)
- [Getting started with ASP.NET Core 6](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6)
- [Getting started with ASP.NET Core 5](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-5)
- [Getting started with ASP.NET Core 3](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-3)
- [ASP.NET Core Layout Renderers](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore)
- [Logging Troubleshooting](https://github.com/NLog/NLog/wiki/Logging-troubleshooting)
- [Have a question?](https://stackoverflow.com/questions/tagged/nlog)
