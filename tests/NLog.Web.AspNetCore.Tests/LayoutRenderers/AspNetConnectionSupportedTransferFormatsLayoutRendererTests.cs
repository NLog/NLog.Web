#if ASP_NET_CORE3
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetConnectionSupportedTransferFormatsLayoutRendererTests : LayoutRenderersTestBase<AspNetConnectionSupportedTransferFormatsLayoutRenderer>
    {
        [Fact]
        public void SuccessBinaryTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var transferFormatFeature = Substitute.For<ITransferFormatFeature>();
            transferFormatFeature.SupportedFormats.Returns(TransferFormat.Binary);

            var collection = new FeatureCollection();
            collection.Set<ITransferFormatFeature>(transferFormatFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(TransferFormat.Binary.ToString(), result);
        }

        [Fact]
        public void SuccessTextTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var transferFormatFeature = Substitute.For<ITransferFormatFeature>();
            transferFormatFeature.SupportedFormats.Returns(TransferFormat.Text);

            var collection = new FeatureCollection();
            collection.Set<ITransferFormatFeature>(transferFormatFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(TransferFormat.Text.ToString(), result);
        }

        [Fact]
        public void SuccessBothTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var transferFormatFeature = Substitute.For<ITransferFormatFeature>();
            transferFormatFeature.SupportedFormats.Returns(TransferFormat.Text|TransferFormat.Binary);

            var collection = new FeatureCollection();
            collection.Set<ITransferFormatFeature>(transferFormatFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal((TransferFormat.Text | TransferFormat.Binary).ToString(), result);
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