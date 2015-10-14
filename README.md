# NLog.Web

[![Version](https://img.shields.io/nuget/v/NLog.Web.svg)](https://www.nuget.org/packages/NLog.Web)
[![AppVeyor](https://img.shields.io/appveyor/ci/nlog/nlog-web/master.svg)](https://ci.appveyor.com/project/nlog/nlog-web/branch/master)

This package is an extension to [NLog](https://github.com/NLog/NLog/). 

This package contains 
targets and layout-renderes specific to ASP.Net and IIS. 

###Targets
* AspNetTrace
* AspNetBufferingWrapper

See [Target documentation at the NLog wiki](https://github.com/NLog/NLog/wiki/Targets)

###Layout renderers
* ${aspnet-application}
* ${aspnet-request}
* ${aspnet-session}
* ${aspnet-item}
* ${aspnet-sessionid}
* ${aspnet-user-authtype}
* ${aspnet-user-identity}
* ${iis-site-name}

See [Layout renderers documentation at the NLog wiki](https://github.com/NLog/NLog/wiki/Layout-Renderers)

##How to use
When installing with Nuget, no additional configuration is needed.

##License
BSD


