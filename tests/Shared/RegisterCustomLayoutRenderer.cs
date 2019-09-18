using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            var httpcontextMock = SetupHttpAccessorWithHttpContext();
#if ASP_NET_CORE
            httpcontextMock.Connection.LocalPort.Returns(123);
#else
            httpcontextMock.Request.RawUrl.Returns("123");
         
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
            var restult = l.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("123", restult);
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
            var httpcontext = Substitute.For<HttpContext>();
            ServiceLocator.ServiceProvider = serviceProviderMock;
#else
            var httpcontext = Substitute.For<HttpContextBase>();
            httpContextAccessorMock.HttpContext.Returns(httpcontext);
            AspNetLayoutRendererBase.DefaultHttpContextAccessor = httpContextAccessorMock;
#endif


            httpContextAccessorMock.HttpContext.Returns(httpcontext);
            return httpcontext;
        }
    }
}
