# NLog.Web (ASP.NET & ASP.NET Core)  [![AppVeyor](https://img.shields.io/appveyor/ci/nlog/nlog-web/master.svg)](https://ci.appveyor.com/project/nlog/nlog-web/branch/master) [![codecov.io](https://codecov.io/github/NLog/NLog.Web/coverage.svg?branch=master)](https://codecov.io/github/NLog/NLog.Web?branch=master)

These packages are extensions to [NLog](https://github.com/NLog/NLog/). 

The packages contain 
targets and layout-renderes specific to ASP.NET (Core), MVC and IIS. 

ASP.NET:  [![Version](https://badge.fury.io/nu/NLog.Web.svg)](https://www.nuget.org/packages/NLog.Web)

ASP.NET Core: [![Version](https://badge.fury.io/nu/NLog.Web.AspNetCore.svg)](https://www.nuget.org/packages/NLog.Web.AspNetCore) 


## ASP.NET

Simply install the package. NLog will detect the extension automatically. 


## ASP.NET Core

There is a special package for ASP.NET Core / MVC Core. This is needed because `HttpContext.Current` isn't available in ASP.NET Core and we can't detect if ASP.NET or ASP.NET Core is used. The package depends on [NLog.Extensions.Logging](https://github.com/NLog/NLog.Extensions.Logging)

The following parts are supported in ASP.NET Core:


* [${aspnet-Item}](https://github.com/NLog/NLog/wiki/AspNetItem-layout-renderer) - ASP.NET `HttpContext` item variable.
* [${aspnet-Request}](https://github.com/NLog/NLog/wiki/AspNetRequest-layout-renderer) - ASP.NET Request variable.  (except
ServerVariable)
* [${aspnet-Session}](https://github.com/NLog/NLog/wiki/AspNetSession-layout-renderer) - ASP.NET Session variable. 
* [${aspnet-SessionId}](https://github.com/NLog/NLog/wiki/AspNetSessionId-layout-renderer) - ASP.NET Session ID variable.
* [${aspnet-User-AuthType}](https://github.com/NLog/NLog/wiki/AspNetUserAuthType-layout-renderer) - ASP.NET User auth.
* [${aspnet-User-Identity}](https://github.com/NLog/NLog/wiki/AspNetUserIdentity-layout-renderer) - ASP.NET User variable.
* [${iis-site-name}](https://github.com/NLog/NLog/wiki/IIS-site-name-Layout-Renderer) - IIS site name.

Introduced in NLog.Web 4.3 & NLog.Web.AspNetCore 4.3

* [${aspnet-MVC-Action}](https://github.com/NLog/NLog/wiki/AspNet-MVC-Action-Layout-Renderer) - ASP.NET MVC action name
* [${aspnet-MVC-Controller}](https://github.com/NLog/NLog/wiki/AspNet-MVC-Controller-Layout-Renderer) - ASP.NET MVC controller name
* [${aspnet-Request-Cookie}](https://github.com/NLog/NLog/wiki/AspNetRequest-Cookie-Layout-Renderer) - ASP.NET Request cookie content. 
* [${aspnet-Request-Host}](https://github.com/NLog/NLog/wiki/AspNetRequest-Host-Layout-Renderer) - ASP.NET Request host.
* [${aspnet-Request-Method}](https://github.com/NLog/NLog/wiki/AspNetRequest-Method-Layout-Renderer) - ASP.NET Request method (GET, POST etc).
* [${aspnet-Request-QueryString}](https://github.com/NLog/NLog/wiki/AspNetRequest-QueryString-Layout-Renderer) - ASP.NET Request querystring.
* [${aspnet-Request-Referrer}](https://github.com/NLog/NLog/wiki/AspNetRequest-Referrer-Renderer) - ASP.NET Request referrer.
* [${aspnet-Request-UserAgent}](https://github.com/NLog/NLog/wiki/AspNetRequest-UserAgent-Layout-Renderer) - ASP.NET Request useragent.
* [${aspnet-Request-Url}](https://github.com/NLog/NLog/wiki/AspNetRequest-Url-Layout-Renderer) - ASP.NET Request URL.

Introduced in NLog.Web.AspNetCore 4.3.1

* [${aspnet-TraceIdentifier}](https://github.com/NLog/NLog/wiki/AspNetTraceIdentifier-Layout-Renderer) - ASP.NET trace identifier

### Usage

- [Getting Started with ASP.NET Core (project.json - vs2015)](https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-(project.json))
- [Getting started with ASP.NET Core (csproj - vs2017)](https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-(csproj---vs2017))
- [Multiple blogs to get started with ASP.NET Core and NLog](https://github.com/damienbod/AspNetCoreNlog)

## Content

This package contains one target, one target-wrapper, multiple layout renderers and one httpmodule. 

### Targets

* AspNetTrace
* AspNetBufferingWrapper

See [Target documentation at the NLog wiki](https://github.com/NLog/NLog/wiki/Targets)

### Layout renderers

* [${aspnet-MVC-Action}](https://github.com/NLog/NLog/wiki/AspNet-MVC-Action-Layout-Renderer) - ASP.NET MVC action name
* [${aspnet-MVC-Controller}](https://github.com/NLog/NLog/wiki/AspNet-MVC-Controller-Layout-Renderer) - ASP.NET MVC controller name
* [${aspnet-Application}](https://github.com/NLog/NLog/wiki/AspNetApplication-layout-renderer) - ASP.NET Application variable.
* [${aspnet-Item}](https://github.com/NLog/NLog/wiki/AspNetItem-layout-renderer) - ASP.NET `HttpContext` item variable.
* [${aspnet-Request}](https://github.com/NLog/NLog/wiki/AspNetRequest-layout-renderer) - ASP.NET Request variable.
* [${aspnet-Request-Cookie}](https://github.com/NLog/NLog/wiki/AspNetRequest-Cookie-Layout-Renderer) - ASP.NET Request cookie content. 
* [${aspnet-Request-Host}](https://github.com/NLog/NLog/wiki/AspNetRequest-Host-Layout-Renderer) - ASP.NET Request host.
* [${aspnet-Request-Method}](https://github.com/NLog/NLog/wiki/AspNetRequest-Method-Layout-Renderer) - ASP.NET Request method (GET, POST etc).
* [${aspnet-Request-QueryString}](https://github.com/NLog/NLog/wiki/AspNetRequest-QueryString-Layout-Renderer) - ASP.NET Request querystring.
* [${aspnet-Request-Referrer}](https://github.com/NLog/NLog/wiki/AspNetRequest-Referrer-Renderer) - ASP.NET Request referrer.
* [${aspnet-Request-UserAgent}](https://github.com/NLog/NLog/wiki/AspNetRequest-UserAgent-Layout-Renderer) - ASP.NET Request useragent.
* [${aspnet-Request-Url}](https://github.com/NLog/NLog/wiki/AspNetRequest-Url-Layout-Renderer) - ASP.NET Request URL.
* [${aspnet-Session}](https://github.com/NLog/NLog/wiki/AspNetSession-layout-renderer) - ASP.NET Session variable. 
* [${aspnet-SessionId}](https://github.com/NLog/NLog/wiki/AspNetSessionId-layout-renderer) - ASP.NET Session ID variable.
* [${aspnet-TraceIdentifier}](https://github.com/NLog/NLog/wiki/AspNetTraceIdentifier-Layout-Renderer) - ASP.NET trace identifier
* [${aspnet-UserAuthType}](https://github.com/NLog/NLog/wiki/AspNetUserAuthType-layout-renderer) - ASP.NET User auth.
* [${aspnet-UserIdentity}](https://github.com/NLog/NLog/wiki/AspNetUserIdentity-layout-renderer) - ASP.NET User variable.
* [${iis-site-name}](https://github.com/NLog/NLog/wiki/IIS-site-name-Layout-Renderer) - IIS site name.


See [Layout renderers documentation at the NLog wiki](https://github.com/NLog/NLog/wiki/Layout-Renderers)

## Configuration
For the targets and layout renderers, no additional configuration is needed.

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


