#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Connections.Features;
using NLog.Web.LayoutRenderers;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.Enums;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestStreamDirectionLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestStreamDirectionLayoutRenderer>
    {
        [Fact]
        public void CanReadSuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = StreamDirectionProperty.CanRead;

            var streamDirectionFeature = Substitute.For<IStreamDirectionFeature>();
            streamDirectionFeature.CanRead.Returns(true);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IStreamDirectionFeature>(streamDirectionFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void CanReadNullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = StreamDirectionProperty.CanRead;

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
            renderer.Property = StreamDirectionProperty.CanRead;

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("0", result);
        }

        [Fact]
        public void CanWriteSuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = StreamDirectionProperty.CanWrite;

            var streamDirectionFeature = Substitute.For<IStreamDirectionFeature>();
            streamDirectionFeature.CanWrite.Returns(true);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IStreamDirectionFeature>(streamDirectionFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void CanWriteNullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = StreamDirectionProperty.CanWrite;

            httpContext.Features.Returns(new FeatureCollection());
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("0", result);
        }
    }
}
#endif
