# NLog.Web.AspNetCore

- [Logging provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers) for ASP.NET Core platform via extension methods for IHostBuilder, and IWebHostBuilder.
- Extends logging output with details from the active HttpContext via NLog [LayoutRenderers](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore), just by changing NLog.config file.
- Capable to log to many destinations via NLog [Targets](https://nlog-project.org/config/?tab=targets).
- Supports logging output in standard formats like JSON, CVS, W3C ELF and XML using NLog [Layouts](https://nlog-project.org/config/?tab=layouts).
- Supports advanced HTTP pipeline interception via provided IMiddleware classes, including logging of HTTP request body and HTTP response body.
- To add new features [pull requests](https://github.com/NLog/NLog.Web/pulls) are always welcome.

Supported platforms:

- For ASP.NET Core 6, .NET 6
- For ASP.NET Core 5, .NET 5
- For ASP.NET Core 3, .NET Core 3.1
- For ASP.NET Core 2, .NET Standard 2.0 and .NET 4.6.1+

Registration of NLog.Web.AspNetCore in the NLog.config File

```xml
<!-- enable ASP.NET Core layout renderers -->
<extensions>
    <add assembly="NLog.Web.AspNetCore"/>
</extensions>
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
