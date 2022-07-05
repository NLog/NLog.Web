using Microsoft.AspNetCore.Http.Features;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestMaxBodySizeLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestMaxBodySizeLayoutRenderer>
    {
        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var maxRequestBodySizeFeature = Substitute.For<IHttpMaxRequestBodySizeFeature>();
            maxRequestBodySizeFeature.MaxRequestBodySize.Returns(435);

            var collection = new FeatureCollection();
            collection.Set<IHttpMaxRequestBodySizeFeature>(maxRequestBodySizeFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("435", result);
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
            Assert.Equal(string.Empty, result);
        }
    }
}
