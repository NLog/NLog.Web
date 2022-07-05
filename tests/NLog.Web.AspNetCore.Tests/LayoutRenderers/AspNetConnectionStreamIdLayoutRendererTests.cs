#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetConnectionStreamIdLayoutRendererTests : LayoutRenderersTestBase<AspNetConnectionStreamIdLayoutRenderer>
    {
        [Fact]
        public void SuccessTextTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var transferFormatFeature = Substitute.For<IStreamIdFeature>();
            transferFormatFeature.StreamId.Returns(527);

            var collection = new FeatureCollection();
            collection.Set<IStreamIdFeature>(transferFormatFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("527", result);
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