using System.Collections.Specialized;
using System.Net;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestLocalIpRendererTests : LayoutRenderersTestBase<AspNetRequestLocalIpLayoutRenderer>
    {
        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#if ASP_NET_CORE
            httpContext.Connection.LocalIpAddress.Returns(IPAddress.Parse("127.0.0.1"));
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("127.0.0.1", result);
#else
            httpContext.Request.ServerVariables.Returns(new NameValueCollection
            {
                {"LOCAL_ADDR","127.0.0.1"}
            });
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("127.0.0.1", result);
#endif
        }
    }
}
