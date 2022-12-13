# NLog.Web

- LoggerProvider for ASP.NET platform. 
- Access many properties via [layout renderers](https://nlog-project.org/config/?tab=layout-renderers) of the event and the application.
- Capable to log to many destinations via [targets](https://nlog-project.org/config/?tab=targets).
- Supports common [layouts](https://nlog-project.org/config/?tab=layouts), such as CVS, JSON, W3C ELF, and XML.
- To add new features [pull requests](https://github.com/NLog/NLog.Web/pulls) are alwsys welcome.

Supported platforms:

.NET 3.5 - 4.8

Registration of NLog.Web in the NLog.config File

```xml
	<!-- enable ASP.NET layout renderers -->
	<extensions>
		<add assembly="NLog.Web"/>
	</extensions>
```

Useful Links

- [Home Page](https://nlog-project.org/)
- [Change Log](https://github.com/NLog/NLog.Web/releases)
- [Tutorial](https://github.com/NLog/NLog/wiki/Tutorial)
- [ASP.NET Layout Renderers](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web)
- [Logging Troubleshooting](https://github.com/NLog/NLog/wiki/Logging-troubleshooting)
- [Support](https://stackoverflow.com/questions/tagged/nlog)
