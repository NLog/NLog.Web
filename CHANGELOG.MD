## Change Log

### v4.5.0-aspnet4 (2017/05/14)

#### Features

- [#66](https://github.com/nlog/nlog.web/pull/66) ${Assembly-version} working for asp.net (core) (#66) (@304NotModified)
- [#140](https://github.com/nlog/nlog.web/pull/140) Added ${aspnet-request-ip} (@304NotModified)
- [#143](https://github.com/nlog/nlog.web/pull/143) added ${aspnet-user-isAuthenticated} (@304NotModified)
- [#125](https://github.com/nlog/nlog.web/pull/125) Added ${aspnet-environment} (@grokky1)
- [#123](https://github.com/nlog/nlog.web/pull/123) Added Culture option to ${aspnet-item} and {aspnet-session} (#123) (@sbebrys)

#### Improvements

- [#141](https://github.com/nlog/nlog.web/pull/141) Improve performance with many threads 2 (@304NotModified)
- [#133](https://github.com/nlog/nlog.web/pull/133) Improve performance with many threads (@304NotModified)

#### Fixes

- [#132](https://github.com/nlog/nlog.web/pull/132) Fix invalid JSON format for ${aspnet-request-cookie} and ${aspnet-request-querystring}, added extra formatting options, consistent rendering of multiple values (@304NotModified)


### v4.4.0-aspnetcore (2017/05/14)

#### Features

- [#66](https://github.com/nlog/nlog.web/pull/66) ${Assembly-version} working for asp.net (core) (#66) (@304NotModified)
- [#140](https://github.com/nlog/nlog.web/pull/140) Added ${aspnet-request-ip} (@304NotModified)
- [#143](https://github.com/nlog/nlog.web/pull/143) added ${aspnet-user-isAuthenticated} (@304NotModified)
- [#126](https://github.com/nlog/nlog.web/pull/126) Add ${aspnet-request-contenttype} (ASP.NET Core only) (@304NotModified)
- [#125](https://github.com/nlog/nlog.web/pull/125) Added ${aspnet-environment} (@grokky1)
- [#123](https://github.com/nlog/nlog.web/pull/123) Added Culture option to ${aspnet-item} and {aspnet-session} (#123) (@sbebrys)

#### Improvements

- [#144](https://github.com/nlog/nlog.web/pull/144) Limit dependecies (#144) (@304NotModified)
- [#142](https://github.com/nlog/nlog.web/pull/142) ignore ObjectDisposedException in aspnet-user renderers (@304NotModified)
- [#141](https://github.com/nlog/nlog.web/pull/141) Improve performance with many threads 2 (@304NotModified)
- [#133](https://github.com/nlog/nlog.web/pull/133) Improve performance with many threads (@304NotModified)

#### Fixes

- [#132](https://github.com/nlog/nlog.web/pull/132) Fix invalid JSON format for ${aspnet-request-cookie} and ${aspnet-request-querystring}, added extra formatting options, consistent rendering of multiple values (@304NotModified)
- [#125](https://github.com/nlog/nlog.web/pull/125) fix ${iis-site-name} for ASP.NET Core (@grokky1)

### v4.3.2-aspnetcore (2017/02/17)
- [#112](https://github.com/nlog/nlog.web/pull/112) NLog.Web.AspNetCore 4.3.2 (@304NotModified)

### v4.4.0-aspnet4 (2017/02/17)
- [#110](https://github.com/nlog/nlog.web/pull/110) Enabled strong naming with correct version (@304NotModified)
- [#111](https://github.com/nlog/nlog.web/pull/111) Update nlog version (@304NotModified)
- [#106](https://github.com/nlog/nlog.web/pull/106) Update assemblyinfo & strong name version to 4.0.0.0 (@304NotModified)
- [#108](https://github.com/nlog/nlog.web/pull/108) Added mvc tag (@304NotModified)

### v4.3.1 (2017/02/10)
- [#98](https://github.com/nlog/nlog.web/pull/98) support for empty 'QueryStringKeys' for ${all-querystrings{aspnet-request-querystring} (@304NotModified)
- [#100](https://github.com/nlog/nlog.web/pull/100) Fixing missing name for ${aspnet-request-cookie} (@304NotModified)
- [#99](https://github.com/nlog/nlog.web/pull/99) Add ${aspnet-traceidentifier} (ASP.NET Core only) (@304NotModified)
- [#97](https://github.com/nlog/nlog.web/pull/97) Update dependencies and version number (@304NotModified)
- [#95](https://github.com/nlog/nlog.web/pull/95) Adding a link to some ASP.NET Core examples (@damienbod)
- [#91](https://github.com/nlog/nlog.web/pull/91) Update AssemblyInfo (@304NotModified)

### v4.3.0-aspnetcore (2017/01/18)
- [#89](https://github.com/nlog/nlog.web/pull/89) Update NLog.Extensions.Logging dependecy (@304NotModified)
- [#80](https://github.com/nlog/nlog.web/pull/80) Update README.md for 4.3 (@304NotModified)
- [#88](https://github.com/nlog/nlog.web/pull/88) Move ASP.NET helpers from nlog.extensions.logging, (@304NotModified)
- [#87](https://github.com/nlog/nlog.web/pull/87) Renames #2, allow aspnet-request-host in non-core ASP.NET (@304NotModified)
- [#86](https://github.com/nlog/nlog.web/pull/86) renames for consistency (@304NotModified)
- [#78](https://github.com/nlog/nlog.web/pull/78) added null checks (@304NotModified)
- [#77](https://github.com/nlog/nlog.web/pull/77) Upgrade to NLog 4.4 (@304NotModified)
- [#68](https://github.com/nlog/nlog.web/pull/68) Readme - make core vs non-core clearer (@timabell)
- [#54](https://github.com/nlog/nlog.web/pull/54) Additional aspnet renderers & unit tests for .net core (@304NotModified, @Page-Not-Found)
- [#61](https://github.com/nlog/nlog.web/pull/61) Upgrade RC2 to RTM. .Net Core. (#61) (@Page-Not-Found)
- [#59](https://github.com/nlog/nlog.web/pull/59) Upgrade to ASP.NET Core RC2 (@304NotModified)

### v4.2.1-aspnet4 (2016/05/12)
- [#57](https://github.com/nlog/nlog.web/pull/57) Package both nupkg + build scripts (#57) (@304NotModified)
- [#56](https://github.com/nlog/nlog.web/pull/56) Suppressing exception thrown when accessing HttpContext.Request (#56) (@vegar)

### 4.2.2 (2016/04/15)
- [#48](https://github.com/nlog/nlog.web/pull/48) Update LICENSE (#48) (@304NotModified)
- [#41](https://github.com/nlog/nlog.web/pull/41) DNX: Prevent stackoverflowException in ${aspnet-session} (#41) (@304NotModified)
- [#52](https://github.com/nlog/nlog.web/pull/52) Extra null check (#52) (@304NotModified)
- [#50](https://github.com/nlog/nlog.web/pull/50) Add dotnet5.4 framework (@304NotModified)
- [#37](https://github.com/nlog/nlog.web/pull/37) fix syntax of readme.md (@petemounce)

### v4.2.1 (2016/02/16)
- [#36](https://github.com/nlog/nlog.web/pull/36) Update README for aspnet5 (@304NotModified)
- [#35](https://github.com/nlog/nlog.web/pull/35) Fix aspnet5 (@304NotModified)
- [#33](https://github.com/nlog/nlog.web/pull/33) Less dependencies (@304NotModified)
- [#30](https://github.com/nlog/nlog.web/pull/30) Added ASP.NET 5 compatible package (@304NotModified)

### v4.2.0 (2016/02/07)
- [#27](https://github.com/nlog/nlog.web/pull/27) Add ability to render Headers (@petemounce)
- [#22](https://github.com/nlog/nlog.web/pull/22) Introducing ASP.NET Layout Renderer base class (@epignosisx)
- [#25](https://github.com/nlog/nlog.web/pull/25) Add info about configuration of NLog.Web in web.config to README.md (@muromek)

### v4.1.0 (2015/11/24)
- [#18](https://github.com/nlog/nlog.web/pull/18) Added Codecov (@304NotModified)
- [#17](https://github.com/nlog/nlog.web/pull/17) MsTest  => xUnit2 (@304NotModified)
- [#16](https://github.com/nlog/nlog.web/pull/16) Added Culture tests and use current culture (can be set) (@304NotModified)
- [#15](https://github.com/nlog/nlog.web/pull/15) Aspnet-item renderer (@304NotModified, @dafanasiev)

### v4.0.0 (2015/10/22)
- [#13](https://github.com/nlog/nlog.web/pull/13) Improved config for release (@304NotModified)
- [#7](https://github.com/nlog/nlog.web/pull/7) Enable Strong name signing (@304NotModified)
- [#8](https://github.com/nlog/nlog.web/pull/8) added unit tests project + tests for session (@304NotModified)

### 2.0.0 (2015/06/11)
- [#3](https://github.com/nlog/nlog.web/pull/3) Added support for logging nested properties of session items. (@Giorgi)
- [#2](https://github.com/nlog/nlog.web/pull/2) Move stuff from extended (@kichristensen)
- [#1](https://github.com/nlog/nlog.web/pull/1) Add IIS web site name layout renderer (@kichristensen)