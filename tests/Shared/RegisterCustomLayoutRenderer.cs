using System;
using System.Linq;
using NLog.Layouts;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using NLog.Web.DependencyInjection;
#else
using System.Web;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests
{
    public class RegisterCustomLayoutRenderer : TestBase
    {
        [Fact]
        public void RegisterLayoutRendererTest()
        {
            var httpContextMock = Substitute.For<IHttpContextAccessor>();
#if ASP_NET_CORE
            httpContextMock.HttpContext.Connection.LocalPort.Returns(123);
#else
            httpContextMock.HttpContext.Request.RawUrl.Returns("123");
#endif

            // Act
            var logFactory = new LogFactory().Setup().RegisterNLogWeb().SetupExtensions(ext =>
                ext.RegisterAspNetLayoutRenderer("test-web",
                (logEventInfo, httpContext, loggingConfiguration) =>
#if ASP_NET_CORE
                    httpContext.Connection.LocalPort)
#else
                    httpContext.Request.RawUrl)
#endif
            ).LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteTo(new NLog.Targets.MemoryTarget("hello") { Layout = "${test-web}" });
            }).LogFactory;

            var target = logFactory.Configuration.FindTargetByName<NLog.Targets.MemoryTarget>("hello");
            var layoutRenderer = (target.Layout as NLog.Layouts.SimpleLayout).Renderers.FirstOrDefault() as NLogWebFuncLayoutRenderer;
            layoutRenderer.HttpContextAccessor = httpContextMock;

            logFactory.GetCurrentClassLogger().Info("Hello World");

            // Assert
            Assert.Single(target.Logs);
            Assert.Equal("123", target.Logs[0]);
        }
    }
}
