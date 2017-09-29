using System;
using System.Collections.Generic;
using System.Globalization;
#if !ASP_NET_CORE
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
        public void UrlPresentRenderNonEmpty_Default()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http");

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/", result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http");

            renderer.IncludeQueryString = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/?t=1", result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString_IncludePort()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeQueryString = true;
            renderer.IncludePort = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com:80/Test.asp?t=1", result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_IncludeQueryString_ExcludePort()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");

            renderer.IncludeQueryString = true;
            renderer.IncludePort = false;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/Test.asp?t=1", result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeQueryString_ExcludePort()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/Test.asp", result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeScheme()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeScheme = false;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("www.google.com/Test.asp", result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeScheme_IncludePort_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeScheme = false;
            renderer.IncludePort = true;
            renderer.IncludeQueryString = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("www.google.com:80/Test.asp?t=1", result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeScheme_IncludePort()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeScheme = false;
            renderer.IncludePort = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("www.google.com:80/Test.asp", result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeScheme_ExcludeHost()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeScheme = false;
            renderer.IncludeHost = false;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("/Test.asp", result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeHost()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeHost = false;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http:///Test.asp", result);
        }

        [Fact]
        public void UrlPresentRenderNonEmpty_ExcludeHost_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeHost = false;
            renderer.IncludeQueryString = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http:///Test.asp?t=1", result);
        }

        private static AspNetRequestUrlRenderer CreateRenderer(string hostBase, string queryString = "", string scheme = "http", string page = "/", string pathBase = "")
        {
            var httpContext = Substitute.For<HttpContextBase>();

#if !ASP_NET_CORE
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
