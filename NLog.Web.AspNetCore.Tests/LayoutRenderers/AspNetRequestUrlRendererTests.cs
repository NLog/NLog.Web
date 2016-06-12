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
using Microsoft.AspNetCore.Http;
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

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Empty(result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http");

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal(result, "http://www.google.com/");
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http");

            renderer.IncludeQueryString = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal(result, "http://www.google.com/?t=1");
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString_IncludePort()
        {
            var expected = "http://www.google.com:80/Test.asp?t=1";

            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeQueryString = true;
            renderer.IncludePort = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal(result, expected);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString_ExcludePort()
        {
            var expected = "http://www.google.com/Test.asp?t=1";

            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");

            renderer.IncludeQueryString = true;
            renderer.IncludePort = false;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal(result, expected);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeQueryString_ExcludePort()
        {
            var expected = "http://www.google.com/Test.asp";

            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal(result, expected);
        }

        private static AspNetRequestUrlRenderer CreateRenderer(string hostBase, string queryString = "", string scheme = "http", string page = "/", string pathBase = "")
        {
            var httpContext = Substitute.For<HttpContextBase>();

#if !NETSTANDARD_1plus
            var url = $"{scheme}://{hostBase}{pathBase}{page}{queryString}";
            httpContext.Request.Url.Returns(new Uri(url));
#else
            httpContext.Request.Path.Returns(new PathString(page));
            httpContext.Request.PathBase.Returns(new PathString(pathBase));
            httpContext.Request.QueryString.Returns(new QueryString(queryString));
            httpContext.Request.Host.Returns(new HostString(hostBase));
            httpContext.Request.Scheme.Returns(scheme);
#endif
            var renderer = new AspNetRequestUrlRenderer();

            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            return renderer;
        }
    }
}
