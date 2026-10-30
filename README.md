![NLog](https://raw.githubusercontent.com/NLog/NLog.github.io/master/images/NLog-logo-only_small.png)

# NLog.Web (ASP.NET & ASP.NET Core) 

|  Nuget-package  | Description  | Supported Platforms |
|:----------------|:-------------|:--------------------|
| [![NuGet Release](https://img.shields.io/nuget/v/NLog.Web.AspNetCore.svg?label=NLog.Web.AspNetCore)](https://www.nuget.org/packages/NLog.Web.AspNetCore) | ASP.NET Core applications for new .NET platforms | .NET 6, 7, 8, 9 and 10 |
| [![NuGet Release](https://img.shields.io/nuget/v/NLog.Web.svg?label=NLog.Web)](https://www.nuget.org/packages/NLog.Web) | Classic ASP.NET MVC applications for .NET Framework | .NET Framework 3.5 - 4.8 | 

[![Build status](https://img.shields.io/appveyor/ci/nlog/nlog-web/master.svg)](https://ci.appveyor.com/project/nlog/nlog-web/branch/master)
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=ncloc)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=bugs)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=vulnerabilities)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=code_smells)](https://sonarcloud.io/project/issues?id=nlog.web&branch=master&resolved=false&types=CODE_SMELL) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=sqale_debt_ratio)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=coverage)](https://sonarcloud.io/component_measures?id=nlog.web&branch=master&metric=coverage) 

Extensions for [NLog](https://github.com/NLog/NLog/) that includes [layout-renderes](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore) for capturing contextual details from the HttpContext for both ASP.NET Core and IIS.

## Releases

For updates and releases, check [CHANGELOG.MD](CHANGELOG.MD) or [Releases](https://github.com/NLog/NLog.Web/releases)

## ASP.NET Core
The [NLog.Web.AspNetCore](https://www.nuget.org/packages/NLog.Web.AspNetCore)-package works with the new ASP.NET Core platforms .NET 6, 7, 8, 9 and 10

- [Tutorial for using NLog with ASP.NET Core](https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6)
- [Layout renderers for ASP.NET Core](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web.aspnetcore) 
- [How to use structured logging](https://github.com/NLog/NLog/wiki/How-to-use-structured-logging)

## ASP.NET Classic

The [NLog.Web](https://www.nuget.org/packages/NLog.Web)-package works with classic ASP.NET MVC with .NET Framework 3.5 - 4.8

- [Targets for ASP.NET](https://nlog-project.org/config/?tab=targets&search=package:nlog.web)
- [Layout renderers for ASP.NET](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web) 

NLog 5.0 requires that NLog.config must include NLog.Web in extensions:
```xml
  <!-- enable ASP.NET layout renderers -->
  <extensions>
    <add assembly="NLog.Web"/>
  </extensions>
```

## Contributions
Contributions are highly appreciated! Please make sure if works for ASP.NET and ASP.NET Core if possible and make sure it is covered by unit tests. 

## License

BSD
