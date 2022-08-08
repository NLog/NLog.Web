#if ASP_NET_CORE3
using NLog.Web.LayoutRenderers;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.Enums;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestMaxBodySizeLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestMaxBodySizeLayoutRenderer>
    {
        [Fact]
        public void IsReadOnlySuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = MaxBodyRequestSizeProperty.IsReadOnly;

            var maxBodySizeFeature = Substitute.For<IHttpMaxRequestBodySizeFeature>();
            maxBodySizeFeature.IsReadOnly.Returns(true);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IHttpMaxRequestBodySizeFeature>(maxBodySizeFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void IsWriteableSuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = MaxBodyRequestSizeProperty.IsReadOnly;

            var maxBodySizeFeature = Substitute.For<IHttpMaxRequestBodySizeFeature>();
            maxBodySizeFeature.IsReadOnly.Returns(false);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IHttpMaxRequestBodySizeFeature>(maxBodySizeFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("0", result);
        }

        [Fact]
        public void IsReadOnlyNullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = MaxBodyRequestSizeProperty.IsReadOnly;

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
            renderer.Property = MaxBodyRequestSizeProperty.IsReadOnly;

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("0", result);
        }

        [Fact]
        public void MaxBodySizeSuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = MaxBodyRequestSizeProperty.MaxBodyRequestSize;

            var maxBodySizeFeature = Substitute.For<IHttpMaxRequestBodySizeFeature>();
            maxBodySizeFeature.MaxRequestBodySize.Returns(457);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IHttpMaxRequestBodySizeFeature>(maxBodySizeFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("457", result);
        }

        [Fact]
        public void MaxBodySizeNullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = MaxBodyRequestSizeProperty.MaxBodyRequestSize;

            httpContext.Features.Returns(new FeatureCollection());
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("", result);
        }
    }
}
#endif
