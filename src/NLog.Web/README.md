# NLog.Web

[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=reliability_rating)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=sqale_rating)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=vulnerabilities)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 

Integrates NLog with the System.Web.HttpContext. If using ASP.NET Core then check [NLog.Web.AspNetCore](https://www.nuget.org/packages/NLog.Web.AspNetCore).

Providing features like:

- Enrich logging output with additional details from active HttpContext using NLog [LayoutRenderers](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore), by just updating the NLog configuration.
- Supports HttpModule injection for [HTTP Request Logging](https://github.com/NLog/NLog.Web/wiki/HTTP-Request-Logging)
- Routing logging output to multiple destinations via the available NLog [Targets](https://nlog-project.org/config/?tab=targets)
- Rendering logging output into standard formats like JSON, CVS, W3C ELF and XML using NLog [Layouts](https://nlog-project.org/config/?tab=layouts).
- Contributions are always welcome, by creating a [pull request](https://github.com/NLog/NLog.Web/pulls).

Supported platforms:

 - .NET 3.5 - 4.8

Registration of NLog.Web in the NLog.config file:

```xml
<extensions>
    <add assembly="NLog.Web"/>
</extensions>
```

Registration of NLog.Web can also be performed with fluent setup:

```csharp
NLog.LogManager.Setup().RegisterNLogWeb();
```

Useful Links:

- [Home Page](https://nlog-project.org/)
- [Change Log](https://github.com/NLog/NLog.Web/releases)
- [Tutorial](https://github.com/NLog/NLog/wiki/Tutorial)
- [ASP.NET Layout Renderers](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web)
- [Logging Troubleshooting](https://github.com/NLog/NLog/wiki/Logging-troubleshooting)
- [Have a question?](https://stackoverflow.com/questions/tagged/nlog)
