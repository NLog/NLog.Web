using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetResponseContentTypeLayoutRendererTests : LayoutRenderersTestBase<AspNetResponseContentTypeLayoutRenderer>
    {
        [Fact]
        public void StatusCode_Set_Renderer()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.Response.ContentType.Returns("text/json");

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("text/json", result);
        }
    }
}
