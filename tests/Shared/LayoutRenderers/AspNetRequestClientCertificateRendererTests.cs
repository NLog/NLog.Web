using System;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestClientCertificateRendererTests : LayoutRenderersTestBase<AspNetRequestClientCertificateLayoutRenderer>
    {
#if ASP_NET_CORE
        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var certificate = new X509Certificate2(
                Convert.FromBase64String(
"MIIC7DCCAdSgAwIBAgIQJq2oGnSgP79FVWGrezYIBDANBgkqhkiG9w0BAQsFADAUMRIwEAYDVQQDEwlsb2NhbGhvc3QwHhcNMjIwMzExMDQyOTI4WhcNMjcwMzEwMDAwMDAwWjAUMRIwEAYDVQQDEwlsb2NhbGhvc3QwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCfSpPySBJetaQdagcgNMs6owXj6QwDv4BKB/qRG4AM5myL3T+7cTiUUOaHywIR+79k2K+LVOLKzfXLJvrzYZSj3yd02ScmM4BEJEcbauY7wCYjgFfB6K9Klh87UAP6+gUUHjIVfzI/Go3883c9D29S3PbO3z5Yz1hTVS0Hes5ZE0d7TDevVSXm2ZpUZSPz7W50+FBq2z3uI3pSBg2oZYHhUvbFIhMI0VIFAPSiyU9XIo+RCv3eN27Fq8g3Qo0z+8wnk7zSldncVEZko5WGKNL781U/TDuhBigkvEme9goaRPRW8oNjm//v+vyJwo/WDzghSwc6jCIdjTXUIqxw4THBAgMBAAGjOjA4MAsGA1UdDwQEAwIEsDATBgNVHSUEDDAKBggrBgEFBQcDATAUBgNVHREEDTALgglsb2NhbGhvc3QwDQYJKoZIhvcNAQELBQADggEBAEqAzHpQgeiW1abdOj4LClM+8uU813tvFhepneLL59yvtZ2NT6ruSd7Fa15CT8bIKACgxzUOaB7N/KseORqLFrNiwvu2A1vvEkhueH0TLC2KJt7SvlEKw5QooiLdOIfNBaL8h/YE/TK9hXCdTgmsPuXuc47vxSyLUUzMGFlW4olFkOEHOP0havplJvPabLY/WpPTV0+mKUe+CKKEKNduH8il9heXguak06XGufP8UQP1fE+GVDFJDqX0S2TMcaoohxL2lV4VqNnadGJ/VA97ZDTWKFteDdTNwZYyb0KvxLtUCc6cHak9ZRs1E7+SZyNx/pcB4vgpnWPXKX8WDr3VGw0="));

            httpContext.Connection.ClientCertificate.Returns(certificate);
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(certificate.ToString(), result);
        }

        [Fact]
        public void SuccessVerboseTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Verbose = true;

            var certificate = new X509Certificate2(
                Convert.FromBase64String(
"MIIC7DCCAdSgAwIBAgIQJq2oGnSgP79FVWGrezYIBDANBgkqhkiG9w0BAQsFADAUMRIwEAYDVQQDEwlsb2NhbGhvc3QwHhcNMjIwMzExMDQyOTI4WhcNMjcwMzEwMDAwMDAwWjAUMRIwEAYDVQQDEwlsb2NhbGhvc3QwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCfSpPySBJetaQdagcgNMs6owXj6QwDv4BKB/qRG4AM5myL3T+7cTiUUOaHywIR+79k2K+LVOLKzfXLJvrzYZSj3yd02ScmM4BEJEcbauY7wCYjgFfB6K9Klh87UAP6+gUUHjIVfzI/Go3883c9D29S3PbO3z5Yz1hTVS0Hes5ZE0d7TDevVSXm2ZpUZSPz7W50+FBq2z3uI3pSBg2oZYHhUvbFIhMI0VIFAPSiyU9XIo+RCv3eN27Fq8g3Qo0z+8wnk7zSldncVEZko5WGKNL781U/TDuhBigkvEme9goaRPRW8oNjm//v+vyJwo/WDzghSwc6jCIdjTXUIqxw4THBAgMBAAGjOjA4MAsGA1UdDwQEAwIEsDATBgNVHSUEDDAKBggrBgEFBQcDATAUBgNVHREEDTALgglsb2NhbGhvc3QwDQYJKoZIhvcNAQELBQADggEBAEqAzHpQgeiW1abdOj4LClM+8uU813tvFhepneLL59yvtZ2NT6ruSd7Fa15CT8bIKACgxzUOaB7N/KseORqLFrNiwvu2A1vvEkhueH0TLC2KJt7SvlEKw5QooiLdOIfNBaL8h/YE/TK9hXCdTgmsPuXuc47vxSyLUUzMGFlW4olFkOEHOP0havplJvPabLY/WpPTV0+mKUe+CKKEKNduH8il9heXguak06XGufP8UQP1fE+GVDFJDqX0S2TMcaoohxL2lV4VqNnadGJ/VA97ZDTWKFteDdTNwZYyb0KvxLtUCc6cHak9ZRs1E7+SZyNx/pcB4vgpnWPXKX8WDr3VGw0="));

            httpContext.Connection.ClientCertificate.Returns(certificate);

            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(certificate.ToString(true), result);
        }

        [Fact]
        public void NullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Connection.ClientCertificate.ReturnsNull();
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(string.Empty, result);
        }
#else
        [Fact]
        public void SuccessTest()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var renderer = new AspNetRequestClientCertificateLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            // Arrange
            var byteArray = Convert.FromBase64String(
                "MIIC7DCCAdSgAwIBAgIQJq2oGnSgP79FVWGrezYIBDANBgkqhkiG9w0BAQsFADAUMRIwEAYDVQQDEwlsb2NhbGhvc3QwHhcNMjIwMzExMDQyOTI4WhcNMjcwMzEwMDAwMDAwWjAUMRIwEAYDVQQDEwlsb2NhbGhvc3QwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCfSpPySBJetaQdagcgNMs6owXj6QwDv4BKB/qRG4AM5myL3T+7cTiUUOaHywIR+79k2K+LVOLKzfXLJvrzYZSj3yd02ScmM4BEJEcbauY7wCYjgFfB6K9Klh87UAP6+gUUHjIVfzI/Go3883c9D29S3PbO3z5Yz1hTVS0Hes5ZE0d7TDevVSXm2ZpUZSPz7W50+FBq2z3uI3pSBg2oZYHhUvbFIhMI0VIFAPSiyU9XIo+RCv3eN27Fq8g3Qo0z+8wnk7zSldncVEZko5WGKNL781U/TDuhBigkvEme9goaRPRW8oNjm//v+vyJwo/WDzghSwc6jCIdjTXUIqxw4THBAgMBAAGjOjA4MAsGA1UdDwQEAwIEsDATBgNVHSUEDDAKBggrBgEFBQcDATAUBgNVHREEDTALgglsb2NhbGhvc3QwDQYJKoZIhvcNAQELBQADggEBAEqAzHpQgeiW1abdOj4LClM+8uU813tvFhepneLL59yvtZ2NT6ruSd7Fa15CT8bIKACgxzUOaB7N/KseORqLFrNiwvu2A1vvEkhueH0TLC2KJt7SvlEKw5QooiLdOIfNBaL8h/YE/TK9hXCdTgmsPuXuc47vxSyLUUzMGFlW4olFkOEHOP0havplJvPabLY/WpPTV0+mKUe+CKKEKNduH8il9heXguak06XGufP8UQP1fE+GVDFJDqX0S2TMcaoohxL2lV4VqNnadGJ/VA97ZDTWKFteDdTNwZYyb0KvxLtUCc6cHak9ZRs1E7+SZyNx/pcB4vgpnWPXKX8WDr3VGw0=");

            var certificate = new X509Certificate2(byteArray);

            // We need Microsoft Fakes here, but we only have Visual Studio 2022 Community, not Enterprise
            // and we also cannot require NLog.Web community of developer to have Enterprise edition
            //httpContext.Request.ClientCertificate = new HttpClientCertificate(byteArray);

            // This throws NullReferenceException
            //httpContext.Request.ClientCertificate.Certificate.Returns(byteArray);

            var httpRequest = Substitute.For<HttpRequestBase>();
            httpContext.Request.Returns(httpRequest);

            // This throws NotSupportedException due to no public constructors, from Castle.DynamicProxy.Generators
            //var clientCertificate = Substitute.For<HttpClientCertificate>();

            //clientCertificate.Certificate.Returns(byteArray);
            //httpRequest.ClientCertificate.Returns(clientCertificate);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            //Assert.Equal(certificate.ToString(), result);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void SuccessVerboseTest()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var renderer = new AspNetRequestClientCertificateLayoutRenderer();
            renderer.Verbose = true;
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            // Arrange
            var byteArray = Convert.FromBase64String(
                "MIIC7DCCAdSgAwIBAgIQJq2oGnSgP79FVWGrezYIBDANBgkqhkiG9w0BAQsFADAUMRIwEAYDVQQDEwlsb2NhbGhvc3QwHhcNMjIwMzExMDQyOTI4WhcNMjcwMzEwMDAwMDAwWjAUMRIwEAYDVQQDEwlsb2NhbGhvc3QwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCfSpPySBJetaQdagcgNMs6owXj6QwDv4BKB/qRG4AM5myL3T+7cTiUUOaHywIR+79k2K+LVOLKzfXLJvrzYZSj3yd02ScmM4BEJEcbauY7wCYjgFfB6K9Klh87UAP6+gUUHjIVfzI/Go3883c9D29S3PbO3z5Yz1hTVS0Hes5ZE0d7TDevVSXm2ZpUZSPz7W50+FBq2z3uI3pSBg2oZYHhUvbFIhMI0VIFAPSiyU9XIo+RCv3eN27Fq8g3Qo0z+8wnk7zSldncVEZko5WGKNL781U/TDuhBigkvEme9goaRPRW8oNjm//v+vyJwo/WDzghSwc6jCIdjTXUIqxw4THBAgMBAAGjOjA4MAsGA1UdDwQEAwIEsDATBgNVHSUEDDAKBggrBgEFBQcDATAUBgNVHREEDTALgglsb2NhbGhvc3QwDQYJKoZIhvcNAQELBQADggEBAEqAzHpQgeiW1abdOj4LClM+8uU813tvFhepneLL59yvtZ2NT6ruSd7Fa15CT8bIKACgxzUOaB7N/KseORqLFrNiwvu2A1vvEkhueH0TLC2KJt7SvlEKw5QooiLdOIfNBaL8h/YE/TK9hXCdTgmsPuXuc47vxSyLUUzMGFlW4olFkOEHOP0havplJvPabLY/WpPTV0+mKUe+CKKEKNduH8il9heXguak06XGufP8UQP1fE+GVDFJDqX0S2TMcaoohxL2lV4VqNnadGJ/VA97ZDTWKFteDdTNwZYyb0KvxLtUCc6cHak9ZRs1E7+SZyNx/pcB4vgpnWPXKX8WDr3VGw0=");

            var certificate = new X509Certificate2(byteArray);

            // We need Microsoft Fakes here, but we only have Visual Studio 2022 Community, not Enterprise
            // and we also cannot require NLog.Web community of developer to have Enterprise edition
            //httpContext.Request.ClientCertificate = new HttpClientCertificate(byteArray);

            // This throws NullReferenceException
            //httpContext.Request.ClientCertificate.Certificate.Returns(byteArray);

            var httpRequest = Substitute.For<HttpRequestBase>();
            httpContext.Request.Returns(httpRequest);

            // This throws NotSupportedException due to no public constructors, from Castle.DynamicProxy.Generators
            //var clientCertificate = Substitute.For<HttpClientCertificate>();

            //clientCertificate.Certificate.Returns(byteArray);
            //httpRequest.ClientCertificate.Returns(clientCertificate);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            //Assert.Equal(certificate.ToString(true), result);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void NullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Request.ClientCertificate.ReturnsNull();
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(string.Empty, result);
        }
#endif
    }
}
