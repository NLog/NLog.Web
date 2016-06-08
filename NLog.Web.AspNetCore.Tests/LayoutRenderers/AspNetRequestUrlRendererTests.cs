using System;
using System.Collections.Generic;
using System.Globalization;
#if !NETSTANDARD_1plus
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
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
            var renderer = CreateRenderer("http://www.google.com/?t=1");

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, "http://www.google.com/");
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString()
        {
            var renderer = CreateRenderer("http://www.google.com/?t=1");

            renderer.IncludeQueryString = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, "http://www.google.com/?t=1");
        }

        private static AspNetRequestUrlRenderer CreateRenderer(string url)
        {
            var httpContext = Substitute.For<HttpContextBase>();
#if !NETSTANDARD_1plus
            httpContext.Request.Url.Returns(new Uri(url));
#else
            httpContext.Request.Host.Host.Returns((callinfo) => url);
#endif
            var renderer = new AspNetRequestUrlRenderer();

            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            return renderer;
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString_IncludePort()
        {
            var expected = "http://www.google.com:80/Test.asp?t=1";
           
            var renderer = CreateRenderer(expected);
            renderer.IncludeQueryString = true;
            renderer.IncludePort = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, expected);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString_ExcludePort()
        {
            var testUrl = "http://www.google.com:80/Test.asp?t=1";
            var expected = "http://www.google.com/Test.asp?t=1";

            var renderer = CreateRenderer(testUrl);

          
            renderer.IncludeQueryString = true;
            renderer.IncludePort = false;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, expected);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeQueryString_ExcludePort()
        {
            var testUrl = "http://www.google.com:80/Test.asp?t=1";
            var expected = "http://www.google.com/Test.asp";

            var renderer = CreateRenderer(testUrl);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, expected);
        }
    }
}
