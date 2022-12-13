# NLog.Web.AspNetCore

- LoggerProvider for ASP.NET Core platform. 
- Access many properties via [layout renderers](https://nlog-project.org/config/?tab=layout-renderers) of the event and the application.
- Capable to log to many destinations via [targets](https://nlog-project.org/config/?tab=targets).
- Supports common [layouts](https://nlog-project.org/config/?tab=layouts), such as CVS, JSON, W3C ELF, and XML.
- To add new features [pull Requests](https://github.com/NLog/NLog.Web/pulls) are alwsys welcome.

Supported platforms:

- For ASP.NET Core 6, .NET 6
- For ASP.NET Core 5, .NET 5
- For ASP.NET Core 3, .NET Core 3.1
- For ASP.NET Core 2, .NET Standard 2.0 and .NET 4.6.1+

Registration of NLog.Web.AspNetCore in the NLog.config File

```xml
	<!-- enable ASP.NET Core layout renderers -->
	<extensions>
		<add assembly="NLog.Web"/>
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
- [Support](https://stackoverflow.com/questions/tagged/nlog)
