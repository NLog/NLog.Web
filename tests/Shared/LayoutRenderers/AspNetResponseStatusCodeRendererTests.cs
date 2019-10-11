using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetResponseStatusCodeRendererTests : LayoutRenderersTestBase<AspNetResponseStatusCodeRenderer>
    {
        [Fact]
        public void StatusCode_Set_Renderer()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.Response.StatusCode.Returns(200);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("200", result);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(99, false)]
        [InlineData(100, true)]
        [InlineData(599, true)]
        [InlineData(600, false)]
        public void Only_Render_Valid_StatusCodes(int statusCode, bool shouldBeRendered)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.Response.StatusCode.Returns(statusCode);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            if (shouldBeRendered)
            {
                Assert.Equal($"{statusCode}", result);
            }
            else
            {
                Assert.Empty(result);
            }
        }
    }
}
