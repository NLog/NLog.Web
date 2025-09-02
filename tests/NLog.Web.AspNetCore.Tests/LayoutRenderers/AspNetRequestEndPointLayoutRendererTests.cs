#if NETCOREAPP3_0_OR_GREATER
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestEndPointLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestEndPointLayoutRenderer>
    {
        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var endPointFeature = Substitute.For<IEndpointFeature>();
            endPointFeature.Endpoint.Returns(new Microsoft.AspNetCore.Http.Endpoint(null, null, "42"));

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IEndpointFeature>(endPointFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("42", result);
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
            Assert.Equal(string.Empty, result);
        }
    }
}
#endif
