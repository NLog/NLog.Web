#if NETCOREAPP3_0_OR_GREATER
using NLog.Web.LayoutRenderers;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestEndPointNameLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestEndPointNameLayoutRenderer>
    {
        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var endPointFeature = Substitute.For<IEndpointFeature>();
            var endPointMetaData = new Microsoft.AspNetCore.Http.EndpointMetadataCollection(new Microsoft.AspNetCore.Routing.EndpointNameMetadata("42"));
            endPointFeature.Endpoint.Returns(new Microsoft.AspNetCore.Http.Endpoint(null, endPointMetaData, "DisplayName"));

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
