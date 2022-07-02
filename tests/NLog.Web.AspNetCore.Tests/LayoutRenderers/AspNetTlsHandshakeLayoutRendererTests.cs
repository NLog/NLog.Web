#if ASP_NET_CORE3
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.Enums;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using System.Security.Authentication;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetTlsHandshakeLayoutRendererTests : LayoutRenderersTestBase<AspNetTlsHandshakeLayoutRenderer>
    {
        private static void SetupFeature(HttpContext httpContext)
        {
            var tlsHandshakeFeature = Substitute.For<ITlsHandshakeFeature>();

            tlsHandshakeFeature.CipherAlgorithm.Returns(CipherAlgorithmType.Aes256);
            tlsHandshakeFeature.CipherStrength.Returns(256);

            tlsHandshakeFeature.HashAlgorithm.Returns(HashAlgorithmType.Sha512);
            tlsHandshakeFeature.HashStrength.Returns(512);

            tlsHandshakeFeature.KeyExchangeAlgorithm.Returns(ExchangeAlgorithmType.RsaSign);
            tlsHandshakeFeature.KeyExchangeStrength.Returns(1024);

            tlsHandshakeFeature.Protocol.Returns(SslProtocols.Tls13);


            var featureCollection = new FeatureCollection();
            featureCollection.Set<ITlsHandshakeFeature>(tlsHandshakeFeature);

            httpContext.Features.Returns(featureCollection);
        }

        [Fact]
        public void CipherAlgorithmTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TlsHandshakeProperty.CipherAlgorithm;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(CipherAlgorithmType.Aes256.ToString(), result);
        }


        [Fact]
        public void CipherStrengthTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TlsHandshakeProperty.CipherStrength;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("256", result);
        }

        [Fact]
        public void HashAlgorithmTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TlsHandshakeProperty.HashAlgorithm;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(HashAlgorithmType.Sha512.ToString(), result);
        }

        [Fact]
        public void HashStrengthTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TlsHandshakeProperty.HashStrength;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("512", result);
        }

        [Fact]
        public void KeyExchangeAlgorithmTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TlsHandshakeProperty.KeyExchangeAlgorithm;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(ExchangeAlgorithmType.RsaSign.ToString(), result);
        }

        [Fact]
        public void KeyExchangeStrengthTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TlsHandshakeProperty.KeyExchangeStrength;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("1024", result);
        }

        [Fact]
        public void ProtocolTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TlsHandshakeProperty.Protocol;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(SslProtocols.Tls13.ToString(), result);
        }


        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("None", result);
        }
    }
}
#endif