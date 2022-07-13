#if ASP_NET_CORE3
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetResponseHttpsCompressionLayoutRendererTests : LayoutRenderersTestBase<AspNetResponseHttpsCompressionLayoutRenderer>
    {
        [Fact]
        public void SuccessDefaultTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var httpsCompressionFeature = Substitute.For<IHttpsCompressionFeature>();
            httpsCompressionFeature.Mode = HttpsCompressionMode.Default;

            var collection = new FeatureCollection();
            collection.Set<IHttpsCompressionFeature>(httpsCompressionFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(HttpsCompressionMode.Default.ToString(), result);
        }

        [Fact]
        public void SuccessCompressTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var httpsCompressionFeature = Substitute.For<IHttpsCompressionFeature>();
            httpsCompressionFeature.Mode = HttpsCompressionMode.Compress;

            var collection = new FeatureCollection();
            collection.Set<IHttpsCompressionFeature>(httpsCompressionFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(HttpsCompressionMode.Compress.ToString(), result);
        }

        [Fact]
        public void SuccessDoNotCompressTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var httpsCompressionFeature = Substitute.For<IHttpsCompressionFeature>();
            httpsCompressionFeature.Mode = HttpsCompressionMode.DoNotCompress;

            var collection = new FeatureCollection();
            collection.Set<IHttpsCompressionFeature>(httpsCompressionFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(HttpsCompressionMode.DoNotCompress.ToString(), result);
        }

        [Fact]
        public void NullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var collection = new FeatureCollection();
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(HttpsCompressionMode.Default.ToString(), result);
        }

        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal(HttpsCompressionMode.Default.ToString(), result);
        }
    }
}
#endif
