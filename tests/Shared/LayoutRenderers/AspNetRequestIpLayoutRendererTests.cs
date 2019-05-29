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
using System.Net;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestIpLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestIpLayoutRenderer>
    {
        private const string ForwardedForHeader = "X-Forwarded-For";

        [Fact]
        public void ForwardedForHeaderNotPresentRenderRemoteAddress()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#if !ASP_NET_CORE
            httpContext.Request.ServerVariables.Returns(new NameValueCollection {{"REMOTE_ADDR", "192.0.0.0"}});
            httpContext.Request.Headers.Returns(new NameValueCollection());
#else
            var headers = new HeaderDict();
            httpContext.Request.Headers.Returns(callinfo => headers);
            httpContext.Connection.RemoteIpAddress.Returns(callinfo => IPAddress.Parse("192.0.0.0"));
#endif
            renderer.CheckForwardedForHeader = true;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("192.0.0.0", result);
        }

        [Fact]
        public void ForwardedForHeaderPresentRenderForwardedValue()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

#if !ASP_NET_CORE
            httpContext.Request.ServerVariables.Returns(new NameValueCollection {{"REMOTE_ADDR", "192.0.0.0"}});
            httpContext.Request.Headers.Returns(new NameValueCollection {{ForwardedForHeader, "127.0.0.1"}});
#else
            var headers = new HeaderDict();
            headers.Add(ForwardedForHeader, new StringValues("127.0.0.1"));
            httpContext.Request.Headers.Returns(callinfo => headers);
#endif
            renderer.CheckForwardedForHeader = true;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("127.0.0.1", result);
        }

        [Fact]
        public void ForwardedForHeaderContainsMultipleEntriesRenderFirstValue()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

#if !ASP_NET_CORE
            httpContext.Request.ServerVariables.Returns(new NameValueCollection {{"REMOTE_ADDR", "192.0.0.0"}});
            httpContext.Request.Headers.Returns(
                new NameValueCollection {{ForwardedForHeader, "127.0.0.1, 192.168.1.1"}});
#else
            var headers = new HeaderDict();
            headers.Add(ForwardedForHeader, new StringValues("127.0.0.1, 192.168.1.1"));
            httpContext.Request.Headers.Returns(callinfo => headers);
#endif
            renderer.CheckForwardedForHeader = true;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("127.0.0.1", result);
        }
    }
}