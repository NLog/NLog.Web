using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestHasPostedBodyLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestHasPostedBodyLayoutRenderer>
    {
        [Fact]
        public void TrueTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

#if NET5_0_OR_GREATER
            var bodyDetectionFeature = Substitute.For<Microsoft.AspNetCore.Http.Features.IHttpRequestBodyDetectionFeature>();
            bodyDetectionFeature.CanHaveBody.Returns(true);

            var featureCollection = new Microsoft.AspNetCore.Http.Features.FeatureCollection();
            featureCollection.Set<Microsoft.AspNetCore.Http.Features.IHttpRequestBodyDetectionFeature>(bodyDetectionFeature);
            httpContext.Features.Returns(featureCollection);
#elif ASP_NET_CORE
            httpContext.Request.ContentLength = 42;
#else
            httpContext.Request.ContentLength.Returns(42);
#endif

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

#if NET5_0_OR_GREATER
            var bodyDetectionFeature = Substitute.For<Microsoft.AspNetCore.Http.Features.IHttpRequestBodyDetectionFeature>();
            bodyDetectionFeature.CanHaveBody.Returns(false);

            var featureCollection = new Microsoft.AspNetCore.Http.Features.FeatureCollection();
            featureCollection.Set<Microsoft.AspNetCore.Http.Features.IHttpRequestBodyDetectionFeature>(bodyDetectionFeature);
            httpContext.Features.Returns(featureCollection);
#elif ASP_NET_CORE
            httpContext.Request.ContentLength = 0;
#else
            httpContext.Request.ContentLength.Returns(0);
#endif

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
#if NET5_0_OR_GREATER
            httpContext.Features.Returns(new Microsoft.AspNetCore.Http.Features.FeatureCollection());
#endif
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