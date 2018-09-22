# NLog.Web (ASP.NET & ASP.NET Core) 
[![AppVeyor](https://img.shields.io/appveyor/ci/nlog/nlog-web/master.svg)](https://ci.appveyor.com/project/nlog/nlog-web/branch/master) [![codecov.io](https://codecov.io/github/NLog/NLog.Web/coverage.svg?branch=master)](https://codecov.io/github/NLog/NLog.Web?branch=master) [![BCH compliance](https://bettercodehub.com/edge/badge/NLog/NLog.Web)](https://bettercodehub.com/results/NLog/NLog.Web)

These packages are extensions to [NLog](https://github.com/NLog/NLog/). 

The packages contain 
targets and layout-renderes specific to ASP.NET (Core), MVC and IIS. 

ASP.NET:  [![Version](https://badge.fury.io/nu/NLog.Web.svg)](https://www.nuget.org/packages/NLog.Web) 

ASP.NET Core 1+2: [![Version](https://badge.fury.io/nu/NLog.Web.AspNetCore.svg)](https://www.nuget.org/packages/NLog.Web.AspNetCore) 


## Getting started

- [Getting started with ASP.NET Core 2](https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-2)
- [Getting started with .NET Core 2 Console application](https://github.com/NLog/NLog.Extensions.Logging/wiki/Getting-started-with-.NET-Core-2---Console-application)
- [Getting started with ASP.NET Core 1 (csproj - vs2017)](https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-(csproj---vs2017))
- [Getting Started with ASP.NET Core 1 (project.json - vs2015)](https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-(project.json))
- [Multiple blogs to get started with ASP.NET Core and NLog](https://github.com/damienbod/AspNetCoreNlog)



## Updates

For updates and releases, check [CHANGELOG.MD](CHANGELOG.MD) or [Releases](https://github.com/NLog/NLog.Web/releases)


## ASP.NET (non-core)

Simply install the package. NLog will detect the extension automatically. 

- [Supported targets for ASP.NET](https://nlog-project.org/config/?tab=target&search=package:nlog.web)
- [Supported layout renderers for ASP.NET](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web) 
 
## ASP.NET Core 1 / ASP.NET Core 2

------
ℹ️  Missing the trace and debug logs in .NET Core 2? [Check your appsettings.json](https://github.com/NLog/NLog.Web/wiki/Missing-trace%5Cdebug-logs-in-ASP.NET-Core-2%3F)

-----

There is a special package for ASP.NET Core / MVC Core. This is needed because `HttpContext.Current` isn't available in ASP.NET Core and we can't detect if ASP.NET or ASP.NET Core is used. The package depends on [NLog.Extensions.Logging](https://github.com/NLog/NLog.Extensions.Logging)

- [Supported layout renderers for ASP.NET Core](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore) 


## Content

This package contains one target, one target-wrapper, multiple layout renderers and one httpmodule. 


## HTTP module (ASP.NET non-core)

_note: Not listed on https://nlog-project.org/config_

There is a ASP.NET ASP.NET HttpModule that enables NLog to hook BeginRequest and EndRequest events easily.

The `NLogHttpModule` needs a registration in the web.config:
```xml
<system.webServer> 
	<modules runAllManagedModulesForAllRequests="true"> 
		<add name="NLog" type="NLog.Web.NLogHttpModule, NLog.Web" />
	</modules>
</system.webServer>
```

## License

BSD


