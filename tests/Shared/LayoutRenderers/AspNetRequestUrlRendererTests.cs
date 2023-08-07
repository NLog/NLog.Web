using System;
using NLog.Web.Enums;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

#if ASP_NET_CORE
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestUrlRendererTests : LayoutRenderersTestBase<AspNetRequestUrlLayoutRenderer>
    {
        [Fact]
        public void UrlPresentRenderNonEmpty_Default()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http");

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/", result);
        }

        [Fact]
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public void UrlPresentRenderNonEmpty_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http");

            renderer.IncludeQueryString = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/?t=1", result);
        }

        [Fact]
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public void UrlPresentRenderNonEmpty_IncludeQueryString_IncludePort()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeQueryString = true;
            renderer.IncludePort = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com:80/Test.asp?t=1", result);
        }

        [Fact]
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
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
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public void UrlPresentRenderNonEmpty_ExcludeScheme()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeScheme = false;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("www.google.com/Test.asp", result);
        }

        [Fact]
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
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
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public void UrlPresentRenderNonEmpty_ExcludeScheme_IncludePort()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeScheme = false;
            renderer.IncludePort = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("www.google.com:80/Test.asp", result);
        }

        [Fact]
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public void UrlPresentRenderNonEmpty_ExcludeScheme_ExcludeHost()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeScheme = false;
            renderer.IncludeHost = false;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("/Test.asp", result);
        }

        [Fact]
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public void UrlPresentRenderNonEmpty_ExcludeHost()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeHost = false;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http:///Test.asp", result);
        }

        [Fact]
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public void UrlPresentRenderNonEmpty_ExcludeHost_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.IncludeHost = false;
            renderer.IncludeQueryString = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http:///Test.asp?t=1", result);
        }
#if ASP_NET_CORE
        [Fact]
        public void UrlPresentRenderNonEmpty_UseRawTarget()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp", rawTarget: "/rawTarget");
            renderer.UseRawTarget = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/rawTarget", result);
        }

        [Fact]
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public void UrlPresentRenderNonEmpty_UseRawTarget_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp", rawTarget: "/rawTarget");
            renderer.UseRawTarget = true;
            renderer.IncludeQueryString = true;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/rawTarget", result);
        }
#endif

        private static AspNetRequestUrlLayoutRenderer CreateRenderer(string hostBase, string queryString = "", string scheme = "http", string page = "/", string pathBase = "", string rawTarget = null)
        {
            var (renderer, httpContext) = CreateWithHttpContext();

#if !ASP_NET_CORE
            var url = $"{scheme}://{hostBase}{pathBase}{page}{queryString}";
            httpContext.Request.Url.Returns(new Uri(url));
#else
            httpContext.Request.Path.Returns(new PathString(page));
            httpContext.Request.PathBase.Returns(new PathString(pathBase));
            httpContext.Request.QueryString.Returns(new QueryString(queryString));
            httpContext.Request.Host.Returns(new HostString(hostBase));
            httpContext.Request.Scheme.Returns(scheme);

            if (rawTarget != null)
            {
                httpContext.Request.HttpContext.Returns(httpContext);

                var httpRequestFeature = new HttpRequestFeatureMock();
                httpRequestFeature.RawTarget = rawTarget;
                var collection = new FeatureCollection();
                collection.Set<IHttpRequestFeature>(httpRequestFeature);
                httpContext.Features.Returns(collection);
            }
#endif
            return renderer;
        }

#if ASP_NET_CORE

        private class HttpRequestFeatureMock : IHttpRequestFeature
        {
            #region Implementation of IHttpRequestFeature

            public Stream Body { get; set; }
            public IHeaderDictionary Headers { get; set; }
            public string Method { get; set; }
            public string Path { get; set; }
            public string PathBase { get; set; }
            public string Protocol { get; set; }
            public string QueryString { get; set; }
            public string RawTarget { get; set; }
            public string Scheme { get; set; }

            #endregion Implementation of IHttpRequestFeature
        }

#endif

        //////////////////////////////////////////////////////////////////////////////////////////////
        // This section has the new unit tests for the Flags Enum based version of the layout renderer
        //////////////////////////////////////////////////////////////////////////////////////////////

        [Fact]
        public void EnumUrlPresentRenderNonEmpty_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http");

            renderer.Properties |= AspNetRequestUrlProperty.Query;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/?t=1", result);
        }

        [Fact]
        public void EnumUrlPresentRenderNonEmpty_IncludeQueryString_IncludePort()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            renderer.Properties |= AspNetRequestUrlProperty.Query;
            renderer.Properties |= AspNetRequestUrlProperty.Port;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com:80/Test.asp?t=1", result);
        }

        [Fact]
        public void EnumUrlPresentRenderNonEmpty_IncludeQueryString_ExcludePort()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");

            renderer.Properties |= AspNetRequestUrlProperty.Query;
            renderer.Properties &= ~AspNetRequestUrlProperty.Port;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/Test.asp?t=1", result);
        }

        [Fact]
        public void EnumUrlPresentRenderNonEmpty_ExcludeScheme()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");

            renderer.Properties &= ~AspNetRequestUrlProperty.Scheme;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("www.google.com/Test.asp", result);
        }

        [Fact]
        public void EnumUrlPresentRenderNonEmpty_ExcludeScheme_IncludePort_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");

            renderer.Properties &= ~AspNetRequestUrlProperty.Scheme;
            renderer.Properties |= AspNetRequestUrlProperty.Query;
            renderer.Properties |= AspNetRequestUrlProperty.Port;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("www.google.com:80/Test.asp?t=1", result);
        }

        [Fact]
        public void EnumUrlPresentRenderNonEmpty_ExcludeScheme_IncludePort()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            
            renderer.Properties &= ~AspNetRequestUrlProperty.Scheme;
            renderer.Properties |= AspNetRequestUrlProperty.Port;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("www.google.com:80/Test.asp", result);
        }

        [Fact]
        public void EnumUrlPresentRenderNonEmpty_ExcludeScheme_ExcludeHost()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");

            renderer.Properties &= ~AspNetRequestUrlProperty.Scheme;
            renderer.Properties &= ~AspNetRequestUrlProperty.Host;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("/Test.asp", result);
        }

        [Fact]
        public void EnumUrlPresentRenderNonEmpty_ExcludeHost()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");
            
            renderer.Properties &= ~AspNetRequestUrlProperty.Host;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http:///Test.asp", result);
        }

        [Fact]
        public void EnumUrlPresentRenderNonEmpty_ExcludeHost_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp");

            renderer.Properties &= ~AspNetRequestUrlProperty.Host;
            renderer.Properties |= AspNetRequestUrlProperty.Query;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http:///Test.asp?t=1", result);
        }
#if ASP_NET_CORE

        [Fact]
        public void EnumUrlPresentRenderNonEmpty_UseRawTarget_IncludeQueryString()
        {
            var renderer = CreateRenderer("www.google.com:80", "?t=1", "http", "/Test.asp", rawTarget: "/rawTarget");
            
            renderer.UseRawTarget = true;

            renderer.Properties |= AspNetRequestUrlProperty.Query;

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("http://www.google.com/rawTarget", result);
        }
#endif
    }
}
