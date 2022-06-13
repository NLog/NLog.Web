using NLog.Web.LayoutRenderers;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public  class AspNetRequestContentTypeLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestContentTypeLayoutRenderer>
    {
        [Fact]
        public void SuccessText()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Request.ContentType = "text/json";

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("text/json", result);
        }

        [Fact]
        public void EmptyTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Request.ContentType = "";
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

            httpContext.Request.ContentType = null;
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("", result);
        }
    }
}
