#if !ASP_NET_CORE
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetSessionIDLayoutRendererTests : LayoutRenderersTestBase<AspNetSessionIdLayoutRenderer>
    {
        [Fact]
        public void NullSessionRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

#if ASP_NET_CORE
            httpContext.Session.Returns(null as Microsoft.AspNetCore.Http.ISession);
#else
            httpContext.Session.Returns(null as HttpSessionStateWrapper);
#endif
            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AvailableSessionRendersSessionId()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var expectedResult = "value";
#if ASP_NET_CORE
            httpContext.Session.Id.Returns(expectedResult);
#else
            httpContext.Session.SessionID.Returns(expectedResult);
#endif
            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}