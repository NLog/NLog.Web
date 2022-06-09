using System.Collections.Specialized;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestRemotePortRendererTests : LayoutRenderersTestBase<AspNetRequestRemotePortLayoutRenderer>
    {
        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#if ASP_NET_CORE
            httpContext.Connection.RemotePort.Returns(8080);
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("8080", result);
#else
            httpContext.Request.ServerVariables.Returns(new NameValueCollection
            {
                {"REMOTE_PORT","8080"}
            });
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("8080", result);
#endif
        }

#if ASP_NET_CORE
        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("0",result);
        }
#endif

    }
}
