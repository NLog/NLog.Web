#if NETCOREAPP3_0_OR_GREATER
using System.Security.Authentication;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.Enums;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestTlsHandshakeLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestTlsHandshakeLayoutRenderer>
    {
        private static void SetupFeature(HttpContext httpContext)
        {
            var tlsHandshakeFeature = Substitute.For<ITlsHandshakeFeature>();

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SYSLIB0058 // Type or member is obsolete
            tlsHandshakeFeature.CipherAlgorithm.Returns(CipherAlgorithmType.Aes256);
            tlsHandshakeFeature.CipherStrength.Returns(256);

            tlsHandshakeFeature.HashAlgorithm.Returns(HashAlgorithmType.Sha512);
            tlsHandshakeFeature.HashStrength.Returns(512);

            tlsHandshakeFeature.KeyExchangeAlgorithm.Returns(ExchangeAlgorithmType.RsaSign);
            tlsHandshakeFeature.KeyExchangeStrength.Returns(1024);

            tlsHandshakeFeature.NegotiatedCipherSuite.Returns((System.Net.Security.TlsCipherSuite)1);
#pragma warning restore SYSLIB0058 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete

            tlsHandshakeFeature.Protocol.Returns(SslProtocols.Tls13);

            tlsHandshakeFeature.HostName.Returns("localhost");

            var featureCollection = new FeatureCollection();
            featureCollection.Set<ITlsHandshakeFeature>(tlsHandshakeFeature);

            httpContext.Features.Returns(featureCollection);
        }

        [Fact]
        public void CipherAlgorithmTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SYSLIB0058 // Type or member is obsolete
            renderer.Property = TlsHandshakeProperty.CipherAlgorithm;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(CipherAlgorithmType.Aes256.ToString(), result);
#pragma warning restore SYSLIB0058 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
        }


        [Fact]
        public void CipherStrengthTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SYSLIB0058 // Type or member is obsolete
            renderer.Property = TlsHandshakeProperty.CipherStrength;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("256", result);
#pragma warning restore SYSLIB0058 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Fact]
        public void HashAlgorithmTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SYSLIB0058 // Type or member is obsolete
            renderer.Property = TlsHandshakeProperty.HashAlgorithm;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(HashAlgorithmType.Sha512.ToString(), result);
#pragma warning restore SYSLIB0058 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Fact]
        public void HashStrengthTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SYSLIB0058 // Type or member is obsolete
            renderer.Property = TlsHandshakeProperty.HashStrength;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("512", result);
#pragma warning restore SYSLIB0058 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Fact]
        public void KeyExchangeAlgorithmTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable SYSLIB0058 // Type or member is obsolete
            renderer.Property = TlsHandshakeProperty.KeyExchangeAlgorithm;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(ExchangeAlgorithmType.RsaSign.ToString(), result);
#pragma warning restore SYSLIB0058 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Fact]
        public void KeyExchangeStrengthTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#pragma warning disable CS0618 // Type or member is obsolete
            renderer.Property = TlsHandshakeProperty.KeyExchangeStrength;
#pragma warning restore CS0618 // Type or member is obsolete
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

#if NET8_0_OR_GREATER
        [Fact]
        public void LocalHostTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TlsHandshakeProperty.HostName;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("localhost", result);
        }

        [Fact]
        public void NegotiatedCipherSuiteTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = TlsHandshakeProperty.NegotiatedCipherSuite;
            SetupFeature(httpContext);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(((System.Net.Security.TlsCipherSuite)1).ToString(), result);
        }
#endif
    }
}
#endif
