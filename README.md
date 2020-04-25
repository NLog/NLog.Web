![NLog](https://raw.githubusercontent.com/NLog/NLog.github.io/master/images/NLog-logo-only_small.png)

# NLog.Web (ASP.NET & ASP.NET Core) 
[![AppVeyor](https://img.shields.io/appveyor/ci/nlog/nlog-web/master.svg)](https://ci.appveyor.com/project/nlog/nlog-web/branch/master)
[![Version](https://img.shields.io/nuget/v/NLog.Web.AspNetCore?label=nuget%20%28ASP.NET%20Core%29)](https://www.nuget.org/packages/NLog.Web.AspNetCore)
[![Version](https://img.shields.io/nuget/v/NLog.Web?label=nuget%20%28ASP.NET%29)](https://www.nuget.org/packages/NLog.Web)

[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=ncloc)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=bugs)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=vulnerabilities)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=code_smells)](https://sonarcloud.io/project/issues?id=nlog.web&branch=master&resolved=false&types=CODE_SMELL) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=duplicated_lines_density)](https://sonarcloud.io/component_measures/domain/Duplications?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=sqale_debt_ratio)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=coverage)](https://sonarcloud.io/component_measures?id=nlog.web&branch=master&metric=coverage) 

These packages are extensions to [NLog](https://github.com/NLog/NLog/). 

The packages contain 
targets and layout-renderes specific to ASP.NET (Core), MVC and IIS. 

## Getting started with NLog

- [Getting started for ASP.NET Core 3](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-3)
- [Getting started for ASP.NET Core 2](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-2)
- [Getting started for .NET Core 2 Console application](https://github.com/NLog/NLog/wiki/Getting-started-with-.NET-Core-2---Console-application)
- [Getting started for ASP.NET Core 1 (csproj - vs2017)](https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-(csproj---vs2017))
- [How to use structured logging](https://github.com/NLog/NLog/wiki/How-to-use-structured-logging)

### Config
- All config options: [nlog-project.org/config](http://nlog-project.org/config)

### Troubleshooting
- [Troubleshooting](https://github.com/NLog/NLog/wiki/Logging-troubleshooting)
- [Internal log](https://github.com/NLog/NLog/wiki/Internal-logging)


## Releases

For updates and releases, check [CHANGELOG.MD](CHANGELOG.MD) or [Releases](https://github.com/NLog/NLog.Web/releases)

## ASP.NET Core
ASP.NET Core 1, 2 and 3 are supported!

Supported platforms:

- For ASP.NET Core 3, .NET Core 3.0
- For ASP.NET Core 2, .NET Standard 2.0+ and .NET 4.6+
- For ASP.NET Core 1, .NET Standard 1.5+ and .NET 4.5.x

ℹ️  Missing the trace and debug logs in .NET Core 2? [Check your appsettings.json](https://github.com/NLog/NLog.Web/wiki/Missing-trace%5Cdebug-logs-in-ASP.NET-Core-2%3F)

Use the NLog.Web.AspNetCore package

- [Supported layout renderers for ASP.NET Core](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore) 

_There is a special package for ASP.NET Core / MVC Core. This is needed because we can't detect if ASP.NET or ASP.NET Core is used. The package depends on [NLog.Extensions.Logging](https://github.com/NLog/NLog.Extensions.Logging) - which integrates with the ASP.NET Core logging system._


## ASP.NET (non-core)

Use the NLog.Web package.

- [Supported targets for ASP.NET](https://nlog-project.org/config/?tab=targets&search=package:nlog.web)
- [Supported layout renderers for ASP.NET](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web) 
- Simply install the package. NLog will detect the extension automatically. 


## HTTP module (ASP.NET non-core)

_note: not listed on https://nlog-project.org/config_

There is a ASP.NET ASP.NET HttpModule that enables NLog to hook BeginRequest and EndRequest events easily.

The `NLogHttpModule` needs a registration in the web.config:
```xml
<system.webServer> 
	<modules runAllManagedModulesForAllRequests="true"> 
		<add name="NLog" type="NLog.Web.NLogHttpModule, NLog.Web" />
	</modules>
</system.webServer>
```

## Contributions
Contributions are highly appreciated! Please make sure if works for ASP.NET and ASP.NET Core if possible and make sure it is covered by unit tests. 


## License

BSD


