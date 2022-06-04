using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestIsWebSocketLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestIsWebSocketLayoutRenderer>
    {
#if !ASP_NET_CORE && NET46_OR_GREATER
        [Fact]
        public void TrueCase()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.IsWebSocketRequest.Returns(true);
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void FalseCase()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.IsWebSocketRequest.Returns(false);
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("0", result);
        }

#endif


#if ASP_NET_CORE
        [Fact]
        public void TrueCase()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.WebSockets.IsWebSocketRequest.Returns(true);
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void FalseCase()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.WebSockets.IsWebSocketRequest.Returns(false);
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("0", result);
        }

        [Fact]
        public void NullCase()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.WebSockets.ReturnsNull();
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("0", result);
        }
#endif

#if ASP_NET_CORE || NET46_OR_GREATER
        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("0" ,result);
        }
#endif
    }
}
