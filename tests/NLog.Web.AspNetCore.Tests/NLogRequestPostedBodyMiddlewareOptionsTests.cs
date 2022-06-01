using System;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogRequestPostedBodyMiddlewareOptionsTests
    {
        [Fact]
        public void SetMaximumRequestSizeTest()
        {
            var config = new NLogRequestPostedBodyMiddlewareOptions();
            var size = new Random().Next();
            config.MaximumRequestSize = size;

            Assert.Equal(size, config.MaximumRequestSize);
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

            HttpRequest request = Substitute.For<HttpRequest>();

            request.ContentLength.Returns(NLogRequestPostedBodyMiddlewareOptions.Default.MaximumRequestSize - 1);

            httpContext.Request.Returns(request);

            Assert.True(config.ShouldCapture(httpContext));
        }

        [Fact]
        public void DefaultCaptureFalseNullContentLength()
        {
            var config = NLogRequestPostedBodyMiddlewareOptions.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpRequest request = Substitute.For<HttpRequest>();

            request.ContentLength.Returns((long?)null);

            httpContext.Request.Returns(request);

            Assert.False(config.ShouldCapture(httpContext));
        }

        [Fact]
        public void DefaultCaptureExcessiveContentLength()
        {
            var config = NLogRequestPostedBodyMiddlewareOptions.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpRequest request = Substitute.For<HttpRequest>();

            request.ContentLength.Returns(NLogRequestPostedBodyMiddlewareOptions.Default.MaximumRequestSize + 1);

            httpContext.Request.Returns(request);

            Assert.False(config.ShouldCapture(httpContext));
        }
    }
}
