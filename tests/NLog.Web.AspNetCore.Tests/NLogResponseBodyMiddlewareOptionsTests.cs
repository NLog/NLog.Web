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
            var config = new NLogResponseBodyMiddlewareOptions();
            var size = new Random().Next();
            config.MaxContentLength = size;

            Assert.Equal(size, config.MaxContentLength);
        }

        [Fact]
        public void GetDefault()
        {
            var config = NLogResponseBodyMiddlewareOptions.Default;

            Assert.NotNull(config);
        }

        [Fact]
        public void DefaultCaptureTrue()
        {
            var config = NLogResponseBodyMiddlewareOptions.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpResponse response = Substitute.For<HttpResponse>();

            response.ContentLength.Returns(NLogResponseBodyMiddlewareOptions.Default.MaxContentLength - 1);

            response.ContentType.Returns("application/json");

            httpContext.Response.Returns(response);

            Assert.True(config.ShouldRetain(httpContext));
        }

        [Fact]
        public void DefaultCaptureFalseNullContentLength()
        {
            var config = NLogResponseBodyMiddlewareOptions.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpResponse response = Substitute.For<HttpResponse>();

            response.ContentLength.Returns((long?)null);

            httpContext.Response.Returns(response);

            Assert.False(config.ShouldRetain(httpContext));
        }

        [Fact]
        public void DefaultCaptureExcessiveContentLength()
        {
            var config = NLogResponseBodyMiddlewareOptions.Default;

            HttpContext httpContext = Substitute.For<HttpContext>();

            HttpResponse response = Substitute.For<HttpResponse>();

            response.ContentLength.Returns(NLogResponseBodyMiddlewareOptions.Default.MaxContentLength + 1);

            httpContext.Response.Returns(response);

            Assert.False(config.ShouldRetain(httpContext));
        }
    }
}
