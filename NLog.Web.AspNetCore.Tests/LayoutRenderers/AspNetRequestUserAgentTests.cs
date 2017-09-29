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
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestUserAgentTests : TestBase
    {
        [Fact]
        public void NullUserAgentRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetRequestUserAgent();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void NotNullUserAgentRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            

#if !ASP_NET_CORE
             httpContext.Request.UserAgent.Returns("TEST");
#else
            var headers = new HeaderDict();
            headers.Add("User-Agent", new StringValues("TEST"));
            httpContext.Request.Headers.Returns((callinfo) => headers);
#endif

            var renderer = new AspNetRequestUserAgent();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal("TEST", result);
        }
    }
}
