#if NET5_0_OR_GREATER
using NLog.Web.LayoutRenderers;
using Microsoft.AspNetCore.Http.Features;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestHasBodyLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestHasBodyLayoutRenderer>
    {
        [Fact]
        public void TrueTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
 
            var bodyDetectionFeature = Substitute.For<IHttpRequestBodyDetectionFeature>();
            bodyDetectionFeature.CanHaveBody.Returns(true);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IHttpRequestBodyDetectionFeature>(bodyDetectionFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void FalseTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var bodyDetectionFeature = Substitute.For<IHttpRequestBodyDetectionFeature>();
            bodyDetectionFeature.CanHaveBody.Returns(false);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IHttpRequestBodyDetectionFeature>(bodyDetectionFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("0", result);
        }

        [Fact]
        public void NullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Features.Returns(new FeatureCollection());
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("0", result);
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
