## Change Log

See also [releases](https://github.com/NLog/NLog.Web/releases) and [milestones](https://github.com/NLog/NLog.Web/milestones).

Date format: (year/month/day)

### v4.13 (2021/08/28)
- **NLog.Web.AspNetCore**
- [#677](https://github.com/NLog/NLog.Web/pull/677) Bump NLog.Extensions.Logging from 1.7.3 to 1.7.4 (#677) (@dependabot[bot])

- **NLog.Web**
- [#674](https://github.com/NLog/NLog.Web/pull/674) Bump NLog from 4.7.10 to 4.7.11  (#674) (@dependabot[bot])

### v4.13 (2021/07/14)
- **NLog.Web.AspNetCore**
  - [#670](https://github.com/NLog/NLog.Web/pull/670) NLogAspNetCoreOptions - Added LoggingConfigurationSectionName (#670) (@snakefoot)
  - [#670](https://github.com/NLog/NLog.Web/pull/670) Bump NLog.Extensions.Logging to 1.7.3 (#670) (@snakefoot)
  - [#669](https://github.com/NLog/NLog.Web/pull/669) Updated xml-docs for AddNLog-methods to instead use AddNLogWeb (#669) (@snakefoot)

- **NLog.Web**
  - [#655](https://github.com/NLog/NLog.Web/pull/655) Bump NLog to 4.7.10 (#664) (@dependabot[bot])

### v4.12-aspnetcore (2021/03/29)
- [#657](https://github.com/NLog/NLog.Web/pull/657) NLogAspNetCoreOptions - Added ReplaceLoggerFactory and RemoveLoggerFactoryFilter (#657) (@snakefoot)
- [#659](https://github.com/NLog/NLog.Web/pull/659) AspNetLayoutRendererBase.Register should cache IHttpContextAccessor (#659) (@304NotModified)
- [#656](https://github.com/NLog/NLog.Web/pull/656) Bump NLog.Extensions.Logging to 1.7.2 (#656) (@dependabot[bot])

### v4.12-aspnet (2021/03/29)
- [#659](https://github.com/NLog/NLog.Web/pull/659) AspNetLayoutRendererBase.Register should cache IHttpContextAccessor (#659) (@304NotModified)
- [#655](https://github.com/NLog/NLog.Web/pull/655) Bump NLog to 4.7.9  (#655) (@dependabot[bot])

### v4.11-aspnetcore (2021/02/27)
- [#644](https://github.com/NLog/NLog.Web/pull/644) Added readme.txt for the NuGet packages (#644) (@304NotModified)
- [#639](https://github.com/NLog/NLog.Web/pull/639) Enable deterministic build (#639) (@304NotModified)
- [#642](https://github.com/NLog/NLog.Web/pull/642) Added SourceLink (#642) (@304NotModified)
- [#631](https://github.com/NLog/NLog.Web/pull/631) AspNetLayoutRendererBase - Added more debug-info when failing to lookup HttpContext (#631) (@snakefoot)
- [#630](https://github.com/NLog/NLog.Web/pull/630) HttpContext - Extended ObjectDisposedException handling (#630) (@snakefoot)
- [#631](https://github.com/NLog/NLog.Web/pull/631) AspNetLayoutRendererBase - Added more debug-info when failing to lookup HttpContext (#631) (@snakefoot)
- [#626](https://github.com/NLog/NLog.Web/pull/626) Add AddNLogWeb with NLog LogFactory and NLogAspNetCoreOptions for better isolation (#626) (@snakefoot)
- [#641](https://github.com/NLog/NLog.Web/pull/641) Bump NLog.Extensions.Logging from 1.7.0 to 1.7.1 (#641) (@dependabot[bot])

### v4.11-aspnet4 (2021/02/27)
- [#644](https://github.com/NLog/NLog.Web/pull/644) Added readme.txt for the NuGet packages (#644) (@304NotModified)
- [#639](https://github.com/NLog/NLog.Web/pull/639) Enable deterministic build (#639) (@304NotModified)
- [#642](https://github.com/NLog/NLog.Web/pull/642) Added SourceLink (#642) (@304NotModified)
- [#635](https://github.com/NLog/NLog.Web/pull/635) Fix AssemblyVersionLayoutRenderer on ASP.NET to work with async=true (#634, #635) (@snakefoot)
- [#631](https://github.com/NLog/NLog.Web/pull/631) AspNetLayoutRendererBase - Added more debug-info when failing to lookup HttpContext (#631) (@snakefoot)
- [#636](https://github.com/NLog/NLog.Web/pull/636) Bump NLog from 4.7.6 to 4.7.8 (#629, #636) (@dependabot[bot])

### v4.10-aspnetcore (2020/12/30)
- [#614](https://github.com/NLog/NLog.Web/pull/614) ASPNETCORE_ENVIRONMENT takes now precedence over DOTNET_ENVIRONMENT (#614) (@snakefoot)
- [#616](https://github.com/NLog/NLog.Web/pull/616) Move to PackageIcon and PackageLicenseExpression for NuGet package (#616) (@snakefoot, @304NotModified)
- [#609](https://github.com/NLog/NLog.Web/pull/609) Added support for HttpContext.User claim lookup using ${aspnet-user-claim} (#609) (@snakefoot)
- [#602](https://github.com/NLog/NLog.Web/pull/602) Introduced AddNLogWeb to avoid conflict with AddNLog from NLog.Extension.Logging (#602) (@snakefoot)
- Updated NLog.Extensions.Logging dependency (Various pull requests)

### v4.10-aspnet4 (2020/12/30)
- [#616](https://github.com/NLog/NLog.Web/pull/616) Move to PackageIcon and PackageLicenseExpression for NuGet package (#616) (@snakefoot, @304NotModified)
- Updated NLog dependency (Various pull requests)

### v4.9.3-aspnetcore (2020/08/01)
- [#549](https://github.com/NLog/NLog.Web/pull/549) ${aspnet-request-posted-body}: Use Async reading to prevent "Synchronous operations are disallowed" (#549) (@304NotModified)
- [#570](https://github.com/NLog/NLog.Web/pull/570) ${aspnet-session} - added ValueType to handle integer session values (#570) (@304NotModified)
- [#540](https://github.com/NLog/NLog.Web/pull/540) LogManager.Setup() - SetupBuilderExtension with LoadConfigurationFromAppSettings (#540) (@snakefoot)
- [#567](https://github.com/NLog/NLog.Web/pull/567) Bump NLog.Extensions.Logging to 1.6.4 (#567) (@dependabot-preview[bot])

- [#574](https://github.com/NLog/NLog.Web/pull/574) Create Dependabot config file (#574) (@dependabot-preview[bot])
- [#553](https://github.com/NLog/NLog.Web/pull/553) Various code improvements (#553) (@304NotModified)

### v4.9.3-aspnet4 (2020/08/01)
- [#540](https://github.com/NLog/NLog.Web/pull/540) LogManager.Setup() - SetupBuilderExtension with LoadConfigurationFromAppSettings (#540) (@snakefoot)
- [#565](https://github.com/NLog/NLog.Web/pull/565) Bump NLog from 4.7.0 to 4.7.2 (#565) (@dependabot-preview[bot])

- [#574](https://github.com/NLog/NLog.Web/pull/574) Create Dependabot config file (#574) (@dependabot-preview[bot])
- [#553](https://github.com/NLog/NLog.Web/pull/553) Various code improvements (#553) (@304NotModified)

### v4.9.2-aspnetcore (2020/04/20)
- [#546](https://github.com/NLog/NLog.Web/pull/546) ${aspnet-request-ip}: make ForwardedForHeader configurable (@304NotModified)
- [#530](https://github.com/NLog/NLog.Web/pull/530) ${aspnet-request-url} Added UseRawTarget option (@sm-g, @304NotModified)

### v4.9.2-aspnet4 (2020/04/20)
- [#546](https://github.com/NLog/NLog.Web/pull/546) ${aspnet-request-ip}: make ForwardedForHeader configurable (@304NotModified)

### v4.9.1-aspnetcore (2020/03/25)
- [#524](https://github.com/NLog/NLog.Web/pull/524) ${aspnet-traceidentifier} should automatically check Activity.Current.Id for .NET Core 3 (@snakefoot)
- [#527](https://github.com/NLog/NLog.Web/pull/527) Improved AddNLog with LoggingConfiguration to handle custom LogFactory (@snakefoot)

### v4.9.1-aspnet4 (2020/03/25)
- Updated dependencies

### v4.9.0-aspnetcore (2019/10/11)
This version adds support for ASP.NET Core 3 and some nice renderers has been added and improved!

- [#496](https://github.com/NLog/NLog.Web/pull/496) Support ASP.NET Core 3 (@snakefoot, @304NotModified)
- [#491](https://github.com/NLog/NLog.Web/pull/491) Added example with ASP.NET Core 3 (@304NotModified)
- [#477](https://github.com/NLog/NLog.Web/pull/477) Added ${aspnet-response-statuscode} - rendering response status code (@304NotModified, @ThomasArdal)
- [#473](https://github.com/NLog/NLog.Web/pull/473) Added ${aspnet-request-headers} - rendering headers for ASP.NET and ASP.NET Core (@ThomasArdal)
- [#472](https://github.com/NLog/NLog.Web/pull/472) ${aspnet-request-cookie} - Added Exclude option (@snakefoot)
- [#465](https://github.com/NLog/NLog.Web/pull/465) ${aspnet-request-cookie} (without cookienames) will render all cookies (@304NotModified)
- [#494](https://github.com/NLog/NLog.Web/pull/494) Added AddNLog with functor to create/initialize LogFactory for NLogLoggingProvider (@snakefoot)
- [#483](https://github.com/NLog/NLog.Web/pull/483) AddNLog-methods for ILoggingBuilder (@snakefoot)

### v4.9.0-aspnet4 (2019/10/11)
- [#477](https://github.com/NLog/NLog.Web/pull/477) Added ${aspnet-response-statuscode} - rendering response status code (@304NotModified, @ThomasArdal)
- [#473](https://github.com/NLog/NLog.Web/pull/473) Added ${aspnet-request-headers} - rendering headers for ASP.NET and ASP.NET Core (@ThomasArdal)
- [#472](https://github.com/NLog/NLog.Web/pull/472) ${aspnet-request-cookie} - Added Exclude option (@snakefoot)
- [#465](https://github.com/NLog/NLog.Web/pull/465) ${aspnet-request-cookie} (without cookienames) will render all cookies (@304NotModified)

### v4.8.6-aspnetcore (2019/09/30)
- [#487](https://github.com/NLog/NLog.Web/pull/487) Fixed: Mixing AddDebug() with a file based NLog configuration no longer works (@snakefoot)

### v4.8.5 aspnetcore (2019/09/07)
- [#459](https://github.com/NLog/NLog.Web/pull/459) Fixed double logging when mixing UseNLog with AddNLog (@snakefoot)

### v4.8.5 aspnet4 (2019/09/07)
- Updated dependencies

### v4.8.4 aspnetcore (2019/07/02)
- [#436](https://github.com/NLog/NLog.Web/pull/436) Better checking for availability of session (@Defee, @snakefoot)

### v4.8.4 aspnet4 (2019/07/02)
- [#442](https://github.com/NLog/NLog.Web/pull/442) Fixed Nuget-Package generation to include dependencies and release notes (@snakefoot)
- [#436](https://github.com/NLog/NLog.Web/pull/436) Better checking for availability of session (@Defee, @snakefoot)

### v4.8.3 aspnetcore & aspnet4 (2019/06/05)
- [#428](https://github.com/NLog/NLog.Web/pull/428) Added AspNetLayoutRendererBase.Register for registering lambdas with httpcontext (@304NotModified)
- [#431](https://github.com/NLog/NLog.Web/pull/431) Fix NotSupportedException with ${aspnet-request-posted-body} (@304NotModified)

### v4.8.2 aspnetcore (2019/05/06)
- [#401](https://github.com/NLog/NLog.Web/pull/401) Fixed that ${aspnet-request-posted-body} could lead to "A non-empty request body is required." in ASP.NET Core 2 (@snakefoot, @304NotModified)
- [#401](https://github.com/NLog/NLog.Web/pull/401) Added MaxContentLength option to ${aspnet-request-posted-body} (@snakefoot, @304NotModified)

### v4.8.2 aspnet4 (2019/05/04)
- [#401](https://github.com/NLog/NLog.Web/pull/401) Added MaxContentLength option to ${aspnet-request-posted-body} (@snakefoot, @304NotModified)

### v4.8.1 aspnetcore & aspnet4 (2019/03/18)
- [#339](https://github.com/NLog/NLog.Web/pull/339) Added ${aspnet-request-posted-body} - log posted request body/payload (@304NotModified)

### v4.8.0-aspnet4 (2019/01/30)
- [#334](https://github.com/NLog/NLog.Web/pull/334) Added ${aspnet-request-contenttype} for NLog.Web (@snakefoot)

### v4.8.0 aspnetcore (2019/01/25)
- [#335](https://github.com/NLog/NLog.Web/pull/335)   (@snakefoot)
    - Updated Microsoft.AspNetCore to ver. 2.1 (Long-Term-Support)
    - ${configsetting} layout renderer now depends on UseNLog-call to provide IConfiguration using IServiceProvider
    - NLogLoggerProvider options now binds to IConfiguration (Using section Logging:NLog)
    - UseNLog-call now also available for IHostBuilder

### v4.7.1-aspnetcore (2019/01/06)
- [#326](https://github.com/NLog/NLog.Web/pull/326) Added ThreadSafe-attribute for LayoutRenderer to optimize async Precalculate (@snakefoot)
- [#325](https://github.com/NLog/NLog.Web/pull/325) ${AspNetRequestIp} added CheckForwardedForHeader to check for X-Forwarded-For header (@Giorgi)
- [#322](https://github.com/NLog/NLog.Web/pull/322) Check that Content-Type is correct before accessing the Form (@mcliment)
- [#333](https://github.com/NLog/NLog.Web/pull/333) Enabled SymbolPackageFormat=snupkg (@snakefoot)

### v4.7.1 aspnet4 (2019/01/06)
- [#330](https://github.com/NLog/NLog.Web/pull/330) aspnet-traceidentifier: Support for ASP.NET (non-core) - requires IIS ETW (@snakefoot)
- [#326](https://github.com/NLog/NLog.Web/pull/326) Added ThreadSafe-attribute for LayoutRenderer to optimize async Precalculate (@snakefoot)
- [#325](https://github.com/NLog/NLog.Web/pull/325) ${AspNetRequestIp} added CheckForwardedForHeader to check for X-Forwarded-For header (@Giorgi)

### v4.7-aspnetcore & aspnet4 (2018/09/22)
- [#315](https://github.com/NLog/NLog.Web/pull/315) Added ${configsetting} for reading appsettings.json etc - via NLog.Extensions.Logging (@304NotModified) (ASP.NET Core only)
- [#313](https://github.com/NLog/NLog.Web/pull/313) Added ${aspnet-request-form} (@DrewBrasher)
- [#317](https://github.com/NLog/NLog.Web/pull/317) Make separators layoutable for (${aspnet-request-querystring} and ${aspnet-request-cookie}) (@304NotModified)
- [#314](https://github.com/NLog/NLog.Web/pull/314) Better missing HttpContext reporting & Skip null-check of HttpContextAccessor.HttpContext in DoAppend (@snakefoot)
- [#312](https://github.com/NLog/NLog.Web/pull/312) Removed dependency on Microsoft.AspNetCore.Http.Extensions (@snakefoot) ASP.NET Core only)

### v4.6-aspnetcore & aspnet4 (2018/08/05)
- [#306](https://github.com/nlog/nlog.web/pull/306) Inherits AssemblyVersionLayoutRenderer from NLog implementation, so it has all NLog's features  (@alexangas)
- [#307](https://github.com/nlog/nlog.web/pull/307) Added "ValuesOnly" property to Cookie and QueryString layout renderers + support for multivalue cookie keys in ASP.NET core (@alexangas)
- [#303](https://github.com/nlog/nlog.web/pull/303) Improve error-handling during startup / shutdown for all layout renders using httpcontext

### v4.5.4-aspnetcore (2018/05/10)
- [#285](https://github.com/nlog/nlog.web/pull/285) Update NLog dependency (@304NotModified)

### v4.5.3-aspnetcore (2018/04/29)
- [#283](https://github.com/nlog/nlog.web/pull/283) Avoid assembly lookup on static class initialization (@snakefoot)

### v4.5.3-aspnet4 (2018/04/12)
- [#275](https://github.com/nlog/nlog.web/pull/275) ${aspnet-request-ip} handle emtpy Request on HttpContext (@304NotModified)

### v4.5.2-aspnetcore (2018/04/12)
- [#273](https://github.com/nlog/nlog.web/pull/273) Added public method to configure the NLog ServiceLocator (@snakefoot)
- [#272](https://github.com/nlog/nlog.web/pull/272) Improved registration of hidden assemblies for callsite handling (@snakefoot)

### v4.5.1-aspnetcore (2018/04/02)
- [#268](https://github.com/nlog/nlog.web/pull/268) Improved ConfigureNLog for better out-of-the-box experience (@snakefoot)

### v4.5.0-aspnetcore (2018/03/27)
- [#257](https://github.com/nlog/nlog.web/pull/257) ConfigureNLog should not be used together with UseNLog (@snakefoot)
- [#256](https://github.com/nlog/nlog.web/pull/256) ASP.NET Core2 Example - Updated with NLog.LogManager.Shutdown() (@snakefoot)
- [#253](https://github.com/nlog/nlog.web/pull/253) ASP.NET Core2 Example - Updated blackhole configuration + ${aspnet-appbasepath} + ${aspnet-webrootpath} (@snakefoot)
- [#252](https://github.com/nlog/nlog.web/pull/252) Configured xunit DisableTestParallelization to make tests more stable (@snakefoot)
- [#250](https://github.com/nlog/nlog.web/pull/250) Removed System.ValueTuple as dependency for .NET Core app 1, add file version to assembly (@snakefoot)
- [#249](https://github.com/nlog/nlog.web/pull/249) Automatically register NLog.Web LayoutRenders for NetCoreApp1 (@snakefoot)
- [#245](https://github.com/nlog/nlog.web/pull/245) Use LogManager.LoadConfiguration + RC3 (@snakefoot)
- [#236](https://github.com/nlog/nlog.web/pull/236) AspNetCore 4.5 RC2 - Update changelog and version (@304NotModified)
- [#232](https://github.com/nlog/nlog.web/pull/232) Updated dependencies (@304NotModified)
- [#233](https://github.com/nlog/nlog.web/pull/233) Better hide assemblies for ${callsite} (@304NotModified)
- [#231](https://github.com/nlog/nlog.web/pull/231) Made ServiceLocator public (@304NotModified)
- [#226](https://github.com/nlog/nlog.web/pull/226) ASP.NET core: 4.5 rc1 + changelog (@304NotModified)
- [#223](https://github.com/nlog/nlog.web/pull/223) .NET 4.6.1 + ASP.NET Core 2 support (@304NotModified)
- [#224](https://github.com/nlog/nlog.web/pull/224) Fix assembly-version (@304NotModified)
- [#212](https://github.com/nlog/nlog.web/pull/212) Beta4 (@304NotModified)
- [#207](https://github.com/nlog/nlog.web/pull/207) Fix ${callsite} from configure (@304NotModified)

### v4.5.0-aspnetcore-beta02 (2017/10/13)
- [#205](https://github.com/nlog/nlog.web/pull/205) Register NLogProvider instead of NLogFactory (@304NotModified)
- [#204](https://github.com/nlog/nlog.web/pull/204) Improve ASP.NET core 2 example - add callsite (@304NotModified)
- [#202](https://github.com/nlog/nlog.web/pull/202) Fix ${callsite} in Asp.net core 2 (@304NotModified)

### v4.5.1-aspnet4 (2017/10/12)
- [#198](https://github.com/nlog/nlog.web/pull/198) Refactor code (@304NotModified)

### v4.5.0-aspnetcore-beta01 (2017/10/12)
- [#197](https://github.com/nlog/nlog.web/pull/197) versions and changelog, updated dependencies: NLog.Web 4.5.1 and NLog.Web.AspNetCore 4.5.0-beta01 (@304NotModified)
- [#195](https://github.com/nlog/nlog.web/pull/195) add ConfigureNLog on ILoggingBuilder (@304NotModified)
- [#191](https://github.com/nlog/nlog.web/pull/191) Added  NLogBuilder and  ASP.NET Core 2 example (@304NotModified)
- [#190](https://github.com/nlog/nlog.web/pull/190) Fix error reporting, fix introduced bug (@304NotModified)
- [#188](https://github.com/nlog/nlog.web/pull/188) Auto register NLog.Web.AspNetCore (ASP.NET 2 only) (@304NotModified)
- [#186](https://github.com/nlog/nlog.web/pull/186) Register HttpContextAccessor by default (@304NotModified)
- [#181](https://github.com/nlog/nlog.web/pull/181) Add UseNLog for ASP.NET Core 2 (@304NotModified)
- [#182](https://github.com/nlog/nlog.web/pull/182) Codecov tweaks (@304NotModified)
- [#180](https://github.com/nlog/nlog.web/pull/180) Fix IHttpContextAccessor warning (@304NotModified)
- [#178](https://github.com/nlog/nlog.web/pull/178) ASP.NET Core 2 support (@304NotModified)
- [#179](https://github.com/nlog/nlog.web/pull/179) rename NETSTANDARD_1plus => ASP_NET_CORE (@304NotModified)
- [#156](https://github.com/nlog/nlog.web/pull/156) Remove unneeded path in build script (@304NotModified)
- [#164](https://github.com/nlog/nlog.web/pull/164) Add IncludeScheme option to ${aspnet-request-url} (@BlaikiC)
- [#162](https://github.com/nlog/nlog.web/pull/162) Update AssemblyVersionLayoutRenderer docs for .NET core (@Valeriy1991)
- [#155](https://github.com/nlog/nlog.web/pull/155) fix xUnit warnings (@304NotModified)
- [#154](https://github.com/nlog/nlog.web/pull/154) deploy to Nuget from AppVeyor (@304NotModified)

### v4.4.1-aspnetcore (2017/06/16)
- [#152](https://github.com/nlog/nlog.web/pull/152) Run test for asp.net core on full and core framework, fix unit test build (@johnkors)
- [#150](https://github.com/nlog/nlog.web/pull/150) Update vulnerable dependency (@johnkors)

### v4.5.0-aspnet4 (2017/05/14)

#### Features

- [#66](https://github.com/nlog/nlog.web/pull/66) ${Assembly-version} working for asp.net (core) (@304NotModified)
- [#140](https://github.com/nlog/nlog.web/pull/140) Added ${aspnet-request-ip} (@304NotModified)
- [#143](https://github.com/nlog/nlog.web/pull/143) added ${aspnet-user-isAuthenticated} (@304NotModified)
- [#125](https://github.com/nlog/nlog.web/pull/125) Added ${aspnet-environment} (@grokky1)
- [#123](https://github.com/nlog/nlog.web/pull/123) Added Culture option to ${aspnet-item} and {aspnet-session} (@sbebrys)

#### Improvements

- [#141](https://github.com/nlog/nlog.web/pull/141) Improve performance with many threads 2 (@304NotModified)
- [#133](https://github.com/nlog/nlog.web/pull/133) Improve performance with many threads (@304NotModified)

#### Fixes

- [#132](https://github.com/nlog/nlog.web/pull/132) Fix invalid JSON format for ${aspnet-request-cookie} and ${aspnet-request-querystring}, added extra formatting options, consistent rendering of multiple values (@304NotModified)


### v4.4.0-aspnetcore (2017/05/14)

#### Features

- [#66](https://github.com/nlog/nlog.web/pull/66) ${Assembly-version} working for asp.net (core) (@304NotModified)
- [#140](https://github.com/nlog/nlog.web/pull/140) Added ${aspnet-request-ip} (@304NotModified)
- [#143](https://github.com/nlog/nlog.web/pull/143) added ${aspnet-user-isAuthenticated} (@304NotModified)
- [#126](https://github.com/nlog/nlog.web/pull/126) Add ${aspnet-request-contenttype} (ASP.NET Core only) (@304NotModified)
- [#125](https://github.com/nlog/nlog.web/pull/125) Added ${aspnet-environment} (@grokky1)
- [#123](https://github.com/nlog/nlog.web/pull/123) Added Culture option to ${aspnet-item} and {aspnet-session} (@sbebrys)

#### Improvements

- [#144](https://github.com/nlog/nlog.web/pull/144) Limit dependecies (@304NotModified)
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
- [#61](https://github.com/nlog/nlog.web/pull/61) Upgrade RC2 to RTM. .Net Core. (@Page-Not-Found)
- [#59](https://github.com/nlog/nlog.web/pull/59) Upgrade to ASP.NET Core RC2 (@304NotModified)

### v4.2.1-aspnet4 (2016/05/12)
- [#57](https://github.com/nlog/nlog.web/pull/57) Package both nupkg + build scripts (@304NotModified)
- [#56](https://github.com/nlog/nlog.web/pull/56) Suppressing exception thrown when accessing HttpContext.Request (@vegar)

### 4.2.2 (2016/04/15)
- [#48](https://github.com/nlog/nlog.web/pull/48) Update LICENSE (@304NotModified)
- [#41](https://github.com/nlog/nlog.web/pull/41) DNX: Prevent stackoverflowException in ${aspnet-session} (@304NotModified)
- [#52](https://github.com/nlog/nlog.web/pull/52) Extra null check (@304NotModified)
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
