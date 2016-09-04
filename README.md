# NLog.Web (ASP.NET & ASP.NET Core)


ASP.NET: [![Version](https://badge.fury.io/nu/NLog.Web.svg)](https://www.nuget.org/packages/NLog.Web)

ASP.NET Core: [![Version](https://badge.fury.io/nu/NLog.Web.AspNetCore.svg)](https://www.nuget.org/packages/NLog.Web.AspNetCore) 

[![AppVeyor](https://img.shields.io/appveyor/ci/nlog/nlog-web/master.svg)](https://ci.appveyor.com/project/nlog/nlog-web/branch/master)
[![codecov.io](https://codecov.io/github/NLog/NLog.Web/coverage.svg?branch=master)](https://codecov.io/github/NLog/NLog.Web?branch=master)

This package is an extension to [NLog](https://github.com/NLog/NLog/). 

This package contains 
targets and layout-renderes specific to ASP.Net and IIS. 

## ASP.NET Core

There is a special package for ASP.NET Core / MVC Core. This is needed because `HttpContext.Current` isn't available in ASP.NET Core and we can't detect if ASP.NET or ASP.NET Core is used.

The following parts are supported in ASP.NET Core:

* aspnet-item
* aspnet-request (except ServerVariable)
* aspnet-session
* aspnet-sessionid
* aspnet-user-authtype
* aspnet-user-identity
* iis-site-name

Please note:

* [ServerVariables are non-existing in ASP.NET Core. ](http://stackoverflow.com/questions/25849217/vnext-server-variables-missing)

### Usage

In your nlog.config:

```xml
  <extensions>
    <!--enable NLog.Web for ASP.NET Core-->
    <add assembly="NLog.Web.AspNetCore"/>
  </extensions>
```

In your startup.cs

```c#
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();

            //add NLog.Web
            app.AddNLogWeb();

```

in project.json:

```json
	"dependencies": {
	    "NLog.Extensions.Logging": "1.0.0-rtm-alpha4",
	    "NLog.Web.AspNetCore": "4.2.3"
	},
```

## Content

This package contains one target, one target-wrapper, multiple layout renderers and one httpmodule. 

### Targets

* AspNetTrace
* AspNetBufferingWrapper

See [Target documentation at the NLog wiki](https://github.com/NLog/NLog/wiki/Targets)

### Layout renderers

* [${aspnet-application}](https://github.com/NLog/NLog/wiki/AspNetApplication-Layout-Renderer) - ASP.NET Application variable.
* [${aspnet-item}](https://github.com/NLog/NLog/wiki/AspNetItem-layout-renderer) - ASP.NET `HttpContext` item variable.
* [${aspnet-request}](https://github.com/NLog/NLog/wiki/AspNetRequest-Layout-Renderer) - ASP.NET Request variable.
* [${aspnet-session}](https://github.com/NLog/NLog/wiki/AspNetSession-Layout-Renderer) - ASP.NET Session variable.
* [${aspnet-sessionid}](https://github.com/NLog/NLog/wiki/AspNetSessionId-Layout-Renderer) - ASP.NET Session ID.
* [${aspnet-user-authtype}](https://github.com/NLog/NLog/wiki/AspNetUserAuthType-Layout-Renderer) - ASP.NET User variable.
* [${aspnet-user-identity}](https://github.com/NLog/NLog/wiki/AspNetUserIdentity-Layout-Renderer) - ASP.NET User variable.
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


