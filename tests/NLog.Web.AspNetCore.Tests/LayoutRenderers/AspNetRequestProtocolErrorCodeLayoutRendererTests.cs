#if NET5_0_OR_GREATER
using NLog.Web.LayoutRenderers;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestProtocolErrorCodeLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestProtocolErrorCodeLayoutRenderer>
    {
        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var errorCodeFeature = Substitute.For<IProtocolErrorCodeFeature>();
            errorCodeFeature.Error.Returns(257);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IProtocolErrorCodeFeature>(errorCodeFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("257", result);
        }

        [Fact]
        public void NullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var errorCodeFeature = Substitute.For<IProtocolErrorCodeFeature>();
            errorCodeFeature.Error.Returns(257);

            httpContext.Features.Returns(new FeatureCollection());
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("-1", result);
        }

        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("0", result);
        }
    }
}
#endif
