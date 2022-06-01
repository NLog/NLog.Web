using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class
        AspNetRequestWebSocketNegotiatedProtocolLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestWebSocketNegotiatedProtocolLayoutRenderer>
    {

#if !ASP_NET_CORE && NET46_OR_GREATER
        [Fact]
        public void SuccessCase()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.WebSocketNegotiatedProtocol.Returns("Json");
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("Json", result);
        }

        [Fact]
        public void NullCase()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.WebSocketNegotiatedProtocol.Returns((string)null);
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void EmptyCase()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.WebSocketNegotiatedProtocol.Returns("");
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(string.Empty, result);
        }
#endif
    }
}
