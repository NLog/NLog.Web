using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestUrlRendererTests
    {

        [Fact]
        public void NullUrlRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetRequestUrlRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeQueryString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Url.Returns(new Uri("http://www.google.com/?t=1"));
            var renderer = new AspNetRequestUrlRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            
            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, "http://www.google.com/");
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Url.Returns(new Uri("http://www.google.com/?t=1"));
            var renderer = new AspNetRequestUrlRenderer();
            renderer.IncludeQueryString = true;
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, "http://www.google.com/?t=1");
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString_IncludePort()
        {
            var expected = "http://www.google.com:80/Test.asp?t=1";
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Url.Returns(new Uri(expected));
            var renderer = new AspNetRequestUrlRenderer();
            renderer.IncludeQueryString = true;
            renderer.IncludePort = true;
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, expected);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString_ExcludePort()
        {
            var testUrl = "http://www.google.com:80/Test.asp?t=1";
            var expected = "http://www.google.com/Test.asp?t=1";


            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Url.Returns(new Uri(testUrl));
            var renderer = new AspNetRequestUrlRenderer();
            renderer.IncludeQueryString = true;
            renderer.IncludePort = false;
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, expected);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeQueryString_ExcludePort()
        {
            var testUrl = "http://www.google.com:80/Test.asp?t=1";
            var expected = "http://www.google.com/Test.asp";


            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Url.Returns(new Uri(testUrl));
            var renderer = new AspNetRequestUrlRenderer();
            
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, expected);
        }
    }
}
