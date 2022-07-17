#if ASP_NET_CORE3
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using System;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestTlsTokenBindingLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestTlsTokenBindingLayoutRenderer>
    {
        [Fact]
        public void SuccessReferrerHexTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var tokenBindingFeature = Substitute.For<ITlsTokenBindingFeature>();
            tokenBindingFeature.GetReferredTokenBindingId().Returns(new byte[] { 1, 2, 3, 4 });

            var collection = new FeatureCollection();
            collection.Set<ITlsTokenBindingFeature>(tokenBindingFeature);
            httpContext.Features.Returns(collection);

            renderer.Format = Enums.ByteArrayFormatProperty.Hex;
            renderer.Property = Enums.TlsTokenBindingProperty.Referrer;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(BitConverter.ToString(new byte[] { 1, 2, 3, 4 }), result);
        }

        [Fact]
        public void SuccessProviderHexTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var tokenBindingFeature = Substitute.For<ITlsTokenBindingFeature>();
            tokenBindingFeature.GetProvidedTokenBindingId().Returns(new byte[] { 5, 6, 7, 8 });

            var collection = new FeatureCollection();
            collection.Set<ITlsTokenBindingFeature>(tokenBindingFeature);
            httpContext.Features.Returns(collection);

            renderer.Format = Enums.ByteArrayFormatProperty.Hex;
            renderer.Property = Enums.TlsTokenBindingProperty.Provider;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(BitConverter.ToString(new byte[] { 5, 6, 7, 8 }), result);
        }

        [Fact]
        public void SuccessReferrerBase64Test()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var tokenBindingFeature = Substitute.For<ITlsTokenBindingFeature>();
            tokenBindingFeature.GetReferredTokenBindingId().Returns(new byte[] { 1, 2, 3, 4 });

            var collection = new FeatureCollection();
            collection.Set<ITlsTokenBindingFeature>(tokenBindingFeature);
            httpContext.Features.Returns(collection);

            renderer.Format = Enums.ByteArrayFormatProperty.Base64;
            renderer.Property = Enums.TlsTokenBindingProperty.Referrer;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }), result);
        }

        [Fact]
        public void SuccessProviderBase64Test()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var tokenBindingFeature = Substitute.For<ITlsTokenBindingFeature>();
            tokenBindingFeature.GetProvidedTokenBindingId().Returns(new byte[] { 5, 6, 7, 8 });

            var collection = new FeatureCollection();
            collection.Set<ITlsTokenBindingFeature>(tokenBindingFeature);
            httpContext.Features.Returns(collection);

            renderer.Format = Enums.ByteArrayFormatProperty.Base64;
            renderer.Property = Enums.TlsTokenBindingProperty.Provider;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(Convert.ToBase64String(new byte[] { 5, 6, 7, 8 }), result);
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
#endif
