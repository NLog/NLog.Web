using System;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogResponseBodyMiddlewareOptionsTests
    {
        [Fact]
        public void SetMaximumRequestSizeTest()
        {
            var config = new NLogRequestPostedBodyMiddlewareOptions();
            var size = new Random().Next();
            config.MaxResponseContentLength = size;

            Assert.Equal(size, config.MaxResponseContentLength);
        }

        [Fact]
        public void GetDefault()
        {
            var config = NLogRequestPostedBodyMiddlewareOptions.Default;

            Assert.NotNull(config);
        }

        [Fact]
        public void DefaultCaptureTrue()
        {
            var config = NLogRequestPostedBodyMiddlewareOptions.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpResponse response = Substitute.For<HttpResponse>();

            response.ContentLength.Returns(NLogRequestPostedBodyMiddlewareOptions.Default.MaxResponseContentLength - 1);

            response.ContentType.Returns("application/json");

            httpContext.Response.Returns(response);

            Assert.True(config.ShouldRetainResponse(httpContext));
        }

        [Fact]
        public void DefaultCaptureFalseNullContentLength()
        {
            var config = NLogRequestPostedBodyMiddlewareOptions.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpResponse response = Substitute.For<HttpResponse>();

            response.ContentLength.Returns((long?)null);

            httpContext.Response.Returns(response);

            Assert.False(config.ShouldRetainResponse(httpContext));
        }

        [Fact]
        public void DefaultCaptureExcessiveContentLength()
        {
            var config = NLogRequestPostedBodyMiddlewareOptions.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpResponse response = Substitute.For<HttpResponse>();

            response.ContentLength.Returns(NLogRequestPostedBodyMiddlewareOptions.Default.MaxResponseContentLength + 1);

            httpContext.Response.Returns(response);

            Assert.False(config.ShouldRetainResponse(httpContext));
        }
    }
}
