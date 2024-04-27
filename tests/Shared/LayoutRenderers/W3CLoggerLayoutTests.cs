using System;
using System.Linq;
using NLog.Layouts;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using NLog.Web.DependencyInjection;
#else
using System.Web;
using NLog.Web.LayoutRenderers;
#endif
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class W3CLoggerLayoutTests : TestBase, IDisposable
    {
        // teardown
        public void Dispose()
        {
#if !ASP_NET_CORE
            HttpContext.Current = null;
            AspNetLayoutRendererBase.DefaultHttpContextAccessor = new DefaultHttpContextAccessor();
#endif
        }

        [Fact]
        public void W3CLoggerLayoutNoContextTest()
        {
            var httpContextEmpty = SetupHttpAccessorWithHttpContext(null);

            NLog.Time.TimeSource.Current = new Time.AccurateUtcTimeSource();

            var logFactory = new NLog.LogFactory().Setup().SetupExtensions(ext => ext.RegisterAssembly(typeof(NLog.Web.Layouts.W3CExtendedLogLayout).Assembly)).LoadConfiguration(builder =>
            {
                var layout = new NLog.Web.Layouts.W3CExtendedLogLayout();
                var target = new NLog.Targets.MemoryTarget() { Name = "Debug", Layout = layout };
                builder.Configuration.AddRuleForAllLevels(target);
            }).LogFactory;

            var logger = logFactory.GetCurrentClassLogger();
            var logEvent = new LogEventInfo(LogLevel.Info, null, "RequestLogging");
            logger.Log(logEvent);
            string expectedFieldHeaders = "c-ip cs-username s-computername cs-method cs-uri-stem cs-uri-query sc-status sc-bytes cs-bytes time-taken cs-host cs(User-Agent)";
            string expectedFieldValues = $"- - {Environment.MachineName} - - - - - - - - -";
            string expectedHeader = $@"#Software: Microsoft Internet Information Server{System.Environment.NewLine}#Version: 1.0{System.Environment.NewLine}#Start-Date: {logEvent.TimeStamp.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)}{System.Environment.NewLine}#Fields: date time {expectedFieldHeaders}";
            string expectedBody= $@"{logEvent.TimeStamp.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)} {expectedFieldValues}";

            var header = logFactory.Configuration.FindTargetByName<NLog.Targets.MemoryTarget>("Debug")?.Logs?.FirstOrDefault();
            var body = logFactory.Configuration.FindTargetByName<NLog.Targets.MemoryTarget>("Debug")?.Logs?.LastOrDefault();

            Assert.Equal(expectedHeader, header);
            Assert.Equal(expectedBody, body);
        }
/*

#if NET6_0_OR_GREATER
        [Fact(Skip = "Mock not working")]
#else
        [Fact]
#endif
        public void W3CLoggerLayoutWithContextTest()
        {
            var httpContextMock = SetupHttpAccessorWithHttpContext("nlog-project.org:80", "http", "/Test.asp", "?t=1");

            NLog.Time.TimeSource.Current = new Time.AccurateUtcTimeSource();

            var logFactory = new NLog.LogFactory().Setup().SetupExtensions(ext => ext.RegisterAssembly(typeof(NLog.Web.Layouts.W3CExtendedLogLayout).Assembly)).LoadConfiguration(builder =>
            {
                var layout = new NLog.Web.Layouts.W3CExtendedLogLayout();
                var target = new NLog.Targets.MemoryTarget() { Name = "Debug", Layout = layout };
                builder.Configuration.AddRuleForAllLevels(target);
            }).LogFactory;

            var logger = logFactory.GetCurrentClassLogger();
            var logEvent = new LogEventInfo(LogLevel.Info, null, "RequestLogging");
            logger.Log(logEvent);
            string expectedFieldHeaders = "c-ip cs-username s-computername cs-method cs-uri-stem cs-uri-query sc-status sc-bytes cs-bytes time-taken cs-host cs(User-Agent)";
            string expectedFieldValues = $"- - {Environment.MachineName} - /Test.asp ?t=1 200 7 42 - nlog-project.org -";
            string expectedHeader = $@"#Software: Microsoft Internet Information Server{System.Environment.NewLine}#Version: 1.0{System.Environment.NewLine}#Start-Date: {logEvent.TimeStamp.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)}{System.Environment.NewLine}#Fields: date time {expectedFieldHeaders}";
            string expectedBody = $@"{logEvent.TimeStamp.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)} {expectedFieldValues}";

            var header = logFactory.Configuration.FindTargetByName<NLog.Targets.MemoryTarget>("Debug")?.Logs?.FirstOrDefault();
            var body = logFactory.Configuration.FindTargetByName<NLog.Targets.MemoryTarget>("Debug")?.Logs?.LastOrDefault();

            Assert.Equal(expectedHeader, header);
            Assert.Equal(expectedBody, body);
        }
*/
        private static
#if ASP_NET_CORE
            HttpContext
#else
            HttpContextBase
#endif
            SetupHttpAccessorWithHttpContext(string hostBase, string scheme = "http", string page = "/", string queryString = "", string userAgent = "Mozilla")
        {
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();

#if ASP_NET_CORE
            var serviceProviderMock = Substitute.For<IServiceProvider>();
            serviceProviderMock.GetService(typeof(IHttpContextAccessor)).Returns(httpContextAccessorMock);
            var httpContext = Substitute.For<HttpContext>();
            ServiceLocator.ServiceProvider = serviceProviderMock;
            if (!string.IsNullOrEmpty(hostBase))
            {
                httpContext.Request.Path.Returns(new PathString(page));
                httpContext.Request.PathBase.Returns(new PathString(string.Empty));
                httpContext.Request.QueryString.Returns(new QueryString(queryString));
                httpContext.Request.Host.Returns(new HostString(hostBase));
                httpContext.Request.Scheme.Returns(scheme);
                httpContext.Request.ContentLength.Returns(42);
                httpContext.Response.StatusCode.Returns(200);
                httpContext.Response.ContentLength.Returns(7);
            }
#else
            var httpContext = Substitute.For<HttpContextBase>();
            AspNetLayoutRendererBase.DefaultHttpContextAccessor = httpContextAccessorMock;
            if (!string.IsNullOrEmpty(hostBase))
            {
                var url = $"{scheme}://{hostBase}{page}{queryString}";
                httpContext.Request.Url.Returns(new Uri(url));
                httpContext.Request.ContentLength.Returns(42);
                httpContext.Response.StatusCode.Returns(200);
                httpContext.Response.OutputStream.Returns(new System.IO.MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7 }));
            }
#endif

            httpContextAccessorMock.HttpContext.Returns(httpContext);
            return httpContext;
        }
    }
}
