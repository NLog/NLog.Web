# NLog.Web

[![Version](https://img.shields.io/nuget/v/NLog.Web.svg)](https://www.nuget.org/packages/NLog.Web)
[![AppVeyor](https://img.shields.io/appveyor/ci/nlog/nlog-web/master.svg)](https://ci.appveyor.com/project/nlog/nlog-web/branch/master)
[![codecov.io](https://codecov.io/github/NLog/NLog.Web/coverage.svg?branch=master)](https://codecov.io/github/NLog/NLog.Web?branch=master)

This package is an extension to [NLog](https://github.com/NLog/NLog/). 

This package contains 
targets and layout-renderes specific to ASP.Net and IIS. 

###Targets
* AspNetTrace
* AspNetBufferingWrapper

See [Target documentation at the NLog wiki](https://github.com/NLog/NLog/wiki/Targets)

###Layout renderers

* [${aspnet-application}](https://github.com/NLog/NLog/wiki/AspNetApplication-Layout-Renderer) - ASP.NET Application variable.
* [${aspnet-item}](https://github.com/NLog/NLog/wiki/AspNetItem-layout-renderer) - ASP.NET `HttpContext` item variable.
* [${aspnet-request}](https://github.com/NLog/NLog/wiki/AspNetRequest-Layout-Renderer) - ASP.NET Request variable.
* [${aspnet-session}](https://github.com/NLog/NLog/wiki/AspNetSession-Layout-Renderer) - ASP.NET Session variable.
* [${aspnet-sessionid}](https://github.com/NLog/NLog/wiki/AspNetSessionId-Layout-Renderer) - ASP.NET Session ID.
* [${aspnet-user-authtype}](https://github.com/NLog/NLog/wiki/AspNetUserAuthType-Layout-Renderer) - ASP.NET User variable.
* [${aspnet-user-identity}](https://github.com/NLog/NLog/wiki/AspNetUserIdentity-Layout-Renderer) - ASP.NET User variable.
* [${iis-site-name}](https://github.com/NLog/NLog/wiki/IIS-site-name-Layout-Renderer) - IIS site name.


See [Layout renderers documentation at the NLog wiki](https://github.com/NLog/NLog/wiki/Layout-Renderers)

##Configuration
For the targets and layout renderers, no additional configuration is needed.

The `NLogHttpModule` needs a registration in the web.config:
```xml
<system.webServer> 
	<modules runAllManagedModulesForAllRequests="true"> 
		<add name="NLog" type="NLog.Web.NLogHttpModule, NLog.Web" />
	</modules>
</system.webServer>
```

##License
BSD


