using System;
using System.Collections.Generic;
using System.Globalization;

#if !ASP_NET_CORE
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestReferrerRendererTests : TestBase
    {

        [Fact]
        public void NullReferrerRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetRequestReferrerRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void ReferrerPresentRenderNonEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
#if !ASP_NET_CORE
            httpContext.Request.UrlReferrer.Returns(new Uri("http://www.google.com/"));
#else
            var headers = new HeaderDict();
            headers.Add("Referer", new StringValues("http://www.google.com/"));
            httpContext.Request.Headers.Returns((callinfo) => headers);
#endif
            var renderer = new AspNetRequestReferrerRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal("http://www.google.com/", result);
        }


    }


}
