using System.Collections;
using System.Collections.Generic;
using NLog.Web.Enums;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class
        AspNetRequestWebSocketRequestedProtocolsLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestWebSocketRequestedProtocolsLayoutRenderer>
    {

#if !ASP_NET_CORE && NET46_OR_GREATER
        [Fact]
        public void SuccessCaseFlat()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.WebSocketRequestedProtocols.Returns(
                new List<string>()
                {
                    "XML",
                    "Json",
                });
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("XML,Json", result);
        }

        [Fact]
        public void SuccessCaseJson()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            httpContext.WebSocketRequestedProtocols.Returns(
                new List<string>()
                {
                    "XML",
                    "Json",
                });
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("[\"XML\",\"Json\"]", result);
        }

        [Fact]
        public void NullCase()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.WebSocketRequestedProtocols.Returns((IList<string>)null);
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

            httpContext.WebSocketRequestedProtocols.Returns(new List<string>());
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(string.Empty, result);
        }
#endif


#if ASP_NET_CORE
        [Fact]
        public void SuccessCaseFlat()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.WebSockets.WebSocketRequestedProtocols.Returns(
                new List<string>()
                {
                    "XML",
                    "Json",
                });
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("XML,Json", result);
        }

        [Fact]
        public void SuccessCaseJson()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            httpContext.WebSockets.WebSocketRequestedProtocols.Returns(
                new List<string>()
                {
                    "XML",
                    "Json",
                });
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("[\"XML\",\"Json\"]", result);
        }

        [Fact]
        public void NullCase()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.WebSockets.WebSocketRequestedProtocols.Returns((IList<string>)null);
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

            httpContext.WebSockets.WebSocketRequestedProtocols.Returns(new List<string>());
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(string.Empty, result);
        }
#endif
    }
}
