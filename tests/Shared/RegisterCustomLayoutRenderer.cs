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
            AspNetLayoutRendererBase.Register("test-web",
                (logEventInfo, httpContext, loggingConfiguration) =>
#if ASP_NET_CORE
                    httpContext.Connection.LocalPort);
#else
                    httpContext.Request.RawUrl);
#endif
            SimpleLayout l = "${test-web}";
            (l.Renderers.FirstOrDefault() as NLogWebFuncLayoutRenderer).HttpContextAccessor = httpContextMock;
            var result = l.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("123", result);
        }
    }
}
