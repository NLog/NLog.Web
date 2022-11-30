![NLog](https://raw.githubusercontent.com/NLog/NLog.github.io/master/images/NLog-logo-only_small.png)

# NLog.Web ASP.NET 

[![NuGet Release](https://img.shields.io/nuget/v/NLog.Web.svg?label=NLog.Web)](https://www.nuget.org/packages/NLog.Web)
<!--[![NuGet Pre Release](https://img.shields.io/nuget/vpre/NLog.Web.svg?label=NLog.Web)](https://www.nuget.org/packages/NLog.Web) -->

[![Build status](https://img.shields.io/appveyor/ci/nlog/nlog-web/master.svg)](https://ci.appveyor.com/project/nlog/nlog-web/branch/master)
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=ncloc)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=bugs)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=vulnerabilities)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=code_smells)](https://sonarcloud.io/project/issues?id=nlog.web&branch=master&resolved=false&types=CODE_SMELL) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=duplicated_lines_density)](https://sonarcloud.io/component_measures/domain/Duplications?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=sqale_debt_ratio)](https://sonarcloud.io/dashboard/?id=nlog.web&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=nlog.web&branch=master&metric=coverage)](https://sonarcloud.io/component_measures?id=nlog.web&branch=master&metric=coverage) 

These packages are extensions to [NLog](https://github.com/NLog/NLog/), and provides targets and layout-renderes specific to ASP.NET, MVC and IIS.

## Getting started with NLog

- [How to use structured logging](https://github.com/NLog/NLog/wiki/How-to-use-structured-logging)

### Config
- All config options: [nlog-project.org/config](https://nlog-project.org/config)

### Troubleshooting
- [Troubleshooting](https://github.com/NLog/NLog/wiki/Logging-troubleshooting)
- [Internal log](https://github.com/NLog/NLog/wiki/Internal-logging)


## Releases

For updates and releases, check [CHANGELOG.MD](CHANGELOG.MD) or [Releases](https://github.com/NLog/NLog.Web/releases)


Use the NLog.Webpackage

- [Supported layout renderers for ASP.NET Core](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web) 

## ASP.NET

The [NLog.Web](https://www.nuget.org/packages/NLog.Web)-package works with classic ASP.NET MVC

- [Supported targets for ASP.NET](https://nlog-project.org/config/?tab=targets&search=package:nlog.web)
- [Supported layout renderers for ASP.NET](https://nlog-project.org/config/?tab=layout-renderers&search=package:nlog.web) 
- NLog 5.0 requires that NLog.config must include NLog.Web in extensions:

```xml
  <!-- enable ASP.NET layout renderers -->
  <extensions>
    <add assembly="NLog.Web"/>
  </extensions>
```

## HTTP module

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
