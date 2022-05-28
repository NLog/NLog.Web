using System;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogRequestPostedBodyMiddlewareConfigurationTests
    {
        [Fact]
        public void SetMaximumRequestSizeTest()
        {
            var config = new NLogRequestPostedBodyMiddlewareConfiguration();
            var size = new Random().Next();
            config.MaximumRequestSize = size;

            Assert.Equal(size, config.MaximumRequestSize);
        }

        [Fact]
        public void SetByteOrderMarkTest()
        {
            var config = new NLogRequestPostedBodyMiddlewareConfiguration();
            var bom = true;
            config.DetectEncodingFromByteOrderMark = bom;

            Assert.Equal(bom, config.DetectEncodingFromByteOrderMark);

            bom = false;
            config.DetectEncodingFromByteOrderMark = bom;

            Assert.Equal(bom, config.DetectEncodingFromByteOrderMark);
        }

        [Fact]
        public void GetDefault()
        {
            var config = NLogRequestPostedBodyMiddlewareConfiguration.Default;

            Assert.NotNull(config);
        }

        [Fact]
        public void DefaultCaptureTrue()
        {
            var config = NLogRequestPostedBodyMiddlewareConfiguration.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpRequest request = Substitute.For<HttpRequest>();

            request.ContentLength.Returns(NLogRequestPostedBodyMiddlewareConfiguration.Default.MaximumRequestSize - 1);

            httpContext.Request.Returns(request);

            Assert.True(config.ShouldCapture(httpContext));
        }

        [Fact]
        public void DefaultCaptureFalseNullContentLength()
        {
            var config = NLogRequestPostedBodyMiddlewareConfiguration.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpRequest request = Substitute.For<HttpRequest>();

            request.ContentLength.Returns((long?)null);

            httpContext.Request.Returns(request);

            Assert.False(config.ShouldCapture(httpContext));
        }

        [Fact]
        public void DefaultCaptureExcessiveContentLength()
        {
            var config = NLogRequestPostedBodyMiddlewareConfiguration.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpRequest request = Substitute.For<HttpRequest>();

            request.ContentLength.Returns(NLogRequestPostedBodyMiddlewareConfiguration.Default.MaximumRequestSize + 1);

            httpContext.Request.Returns(request);

            Assert.False(config.ShouldCapture(httpContext));
        }
    }
}
