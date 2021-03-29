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
#if !ASP_NET_CORE
        ~RegisterCustomLayoutRenderer()
        {
            AspNetLayoutRendererBase.DefaultHttpContextAccessor = new DefaultHttpContextAccessor();
        }
#endif

        [Fact]
        public void RegisterLayoutRendererTest()
        {
            var httpContextMock = SetupHttpAccessorWithHttpContext();
#if ASP_NET_CORE
            httpContextMock.Connection.LocalPort.Returns(123);
#else
            httpContextMock.Request.RawUrl.Returns("123");
         
#endif

            // Act
            AspNetLayoutRendererBase.Register("test-web",
                (logEventInfo, httpContext, loggingConfiguration) =>
#if ASP_NET_CORE
                    httpContext.Connection.LocalPort);
#else
                    httpContext.Request.RawUrl);
#endif
            Layout l = "${test-web}";
            var result = l.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("123", result);
        }

        private static
#if ASP_NET_CORE
            HttpContext
#else
            HttpContextBase
#endif

            SetupHttpAccessorWithHttpContext()
        {
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();


#if ASP_NET_CORE
            var serviceProviderMock = Substitute.For<IServiceProvider>();
            serviceProviderMock.GetService(typeof(IHttpContextAccessor)).Returns(httpContextAccessorMock);
            var httpContext = Substitute.For<HttpContext>();
            ServiceLocator.ServiceProvider = serviceProviderMock;
#else
            var httpContext = Substitute.For<HttpContextBase>();
            httpContextAccessorMock.HttpContext.Returns(httpContext);
            AspNetLayoutRendererBase.DefaultHttpContextAccessor = httpContextAccessorMock;
#endif


            httpContextAccessorMock.HttpContext.Returns(httpContext);
            return httpContext;
        }
    }
}
