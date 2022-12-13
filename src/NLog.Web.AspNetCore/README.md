# NLog.Web.AspNetCore

- LoggerProvider for ASP.NET Core platform. 
- Adds helpers and layout renderers for websites and web applications.

Supported platforms:

- For ASP.NET Core 6, .NET 6
- For ASP.NET Core 5, .NET 5
- For ASP.NET Core 3, .NET Core 3.1
- For ASP.NET Core 2, .NET Standard 2.0 and .NET 4.6.1+

Registration of NLog.Web in the NLog.config File

```xml
	<!-- enable ASP.NET layout renderers -->
	<extensions>
		<add assembly="NLog.Web"/>
	</extensions>
```

Useful Links

- [Change Log](https://github.com/NLog/NLog.Web/releases)
- [Getting started with ASP.NET Core 6](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6)
- [Getting started with ASP.NET Core 5](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-5)
- [Getting started with ASP.NET Core 3](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-3)
- [ASP.NET Core Layout Renderers](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore)
- [Logging Troubleshooting](https://github.com/NLog/NLog/wiki/Logging-troubleshooting)
