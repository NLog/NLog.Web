using System.Collections.Specialized;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestLocalPortRendererTests : LayoutRenderersTestBase<AspNetRequestLocalPortLayoutRenderer>
    {
#if ASP_NET_CORE
        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Connection.LocalPort.Returns(8080);
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("8080", result);
        }
        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("0",result);
        }
#else
        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Request.ServerVariables.Returns(new NameValueCollection
            {
                {"LOCAL_PORT", "8080"}
            });

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("8080", result);
        }
#endif
    }
}
