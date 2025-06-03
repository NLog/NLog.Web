using NLog.Web.LayoutRenderers;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetResponseContentLengthLayoutRendererTests : LayoutRenderersTestBase<AspNetResponseContentLengthLayoutRenderer>
    {
        [Fact]
        public void SuccessText()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Response.ContentLength = 57;
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

            httpContext.Response.ContentLength = 0;

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

            httpContext.Response.ReturnsNull();
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("", result);
        }
    }
}