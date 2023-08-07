using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestContentLengthLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestContentLengthLayoutRenderer>
    {
        [Fact]
        public void SuccessText()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

#if ASP_NET_CORE
            httpContext.Request.ContentLength = 57;
#else
            httpContext.Request.ContentLength.Returns(57);
#endif
            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("57", result);
        }

        [Fact]
        public void EmptyTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

#if ASP_NET_CORE
            httpContext.Request.ContentLength = 0;
#else
            httpContext.Request.ContentLength.Returns(0);
#endif
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void NullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Request.ReturnsNull();
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("", result);
        }
    }
}
