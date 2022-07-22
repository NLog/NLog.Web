#if ASP_NET_CORE3
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.LayoutRenderers;
using NLog.Web.Enums;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestTransferFormatLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestTransferFormatLayoutRenderer>
    {
        [Fact]
        public void ActiveFormatSuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TransferFormatProperty.ActiveFormat;

            var transferFormatFeature = Substitute.For<ITransferFormatFeature>();
            transferFormatFeature.ActiveFormat.Returns(TransferFormat.Binary);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<ITransferFormatFeature>(transferFormatFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("Binary", result);
        }

        [Fact]
        public void SupportedFormatSuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TransferFormatProperty.SupportedFormats;

            var transferFormatFeature = Substitute.For<ITransferFormatFeature>();
            transferFormatFeature.SupportedFormats.Returns(TransferFormat.Binary);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<ITransferFormatFeature>(transferFormatFeature);

            httpContext.Features.Returns(featureCollection);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("Binary", result);
        }

        [Fact]
        public void ActiveFormatNullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TransferFormatProperty.ActiveFormat;

            httpContext.Features.Returns(new FeatureCollection());
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void SupportedFormatNullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TransferFormatProperty.SupportedFormats;

            httpContext.Features.Returns(new FeatureCollection());
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("", result);
        }

        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();
            renderer.Property = TransferFormatProperty.SupportedFormats;

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("0", result);
        }
    }
}
#endif
