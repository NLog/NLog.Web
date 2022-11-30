![NLog](https://raw.githubusercontent.com/NLog/NLog.github.io/master/images/NLog-logo-only_small.png)

# NLog.Web ASP.NET Core

[![NuGet Release](https://img.shields.io/nuget/v/NLog.Web.AspNetCore.svg?label=NLog.Web.AspNetCore)](https://www.nuget.org/packages/NLog.Web.AspNetCore)
<!--[![NuGet Pre Release](https://img.shields.io/nuget/vpre/NLog.Web.AspNetCore.svg?label=NLog.Web.AspNetCore)](https://www.nuget.org/packages/NLog.Web.AspNetCore)-->

[![Build status](https://img.shields.io/appveyor/ci/nlog/nlog-web/master.svg)](https://ci.appveyor.com/project/nlog/nlog-web/branch/master)
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=ncloc)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=bugs)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=vulnerabilities)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=code_smells)](https://sonarcloud.io/project/issues?id=nlog.web&branch=master&resolved=false&types=CODE_SMELL) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=duplicated_lines_density)](https://sonarcloud.io/component_measures/domain/Duplications?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=sqale_debt_ratio)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=coverage)](https://sonarcloud.io/component_measures?id=nlog.web&branch=master&metric=coverage) 

These packages are extensions to [NLog](https://github.com/NLog/NLog/), and provides targets and layout-renderes specific to ASP.NET Core, MVC and IIS.

## Getting started with NLog

- [Getting started for ASP.NET Core 6](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6)
- [Getting started for ASP.NET Core 5](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-5)
- [Getting started for ASP.NET Core 3.1](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-3)
- [Getting started for .NET Core Console application](https://github.com/NLog/NLog/wiki/Getting-started-with-.NET-Core-2---Console-application)
- [How to use structured logging](https://github.com/NLog/NLog/wiki/How-to-use-structured-logging)
- [Blog posts for how to get started with ASP.NET Core and NLog](https://github.com/damienbod/AspNetCoreNlog)

### Config
- All config options: [nlog-project.org/config](https://nlog-project.org/config)

### Troubleshooting
- [Troubleshooting](https://github.com/NLog/NLog/wiki/Logging-troubleshooting)
- [Internal log](https://github.com/NLog/NLog/wiki/Internal-logging)


## Releases

For updates and releases, check [CHANGELOG.MD](CHANGELOG.MD) or [Releases](https://github.com/NLog/NLog.Web/releases)

## ASP.NET Core
The [NLog.Web.AspNetCore](https://www.nuget.org/packages/NLog.Web.AspNetCore)-package supports the platforms:

- For ASP.NET Core 6, .NET 6
- For ASP.NET Core 5, .NET 5
- For ASP.NET Core 3, .NET Core 3
- For ASP.NET Core 2.1 .NET Standard 2.0 for .NET 4.6.1

ℹ️  Missing the trace and debug logs? [Check your appsettings.json](https://github.com/NLog/NLog.Web/wiki/Missing-trace%5Cdebug-logs-in-ASP.NET-Core-6%3F)

Use the NLog.Web.AspNetCore package

- [Supported layout renderers for ASP.NET Core](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore) 

## Contributions
Contributions are highly appreciated! Please make sure if works for ASP.NET and ASP.NET Core if possible and make sure it is covered by unit tests.

## License

BSD
