using System;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;
#if !ASP_NET_CORE
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestReferrerRendererTests : LayoutRenderersTestBase<AspNetRequestReferrerLayoutRenderer>
    {
        [Fact]
        public void ReferrerPresentRenderNonEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

#if !ASP_NET_CORE
            httpContext.Request.UrlReferrer.Returns(new Uri("http://www.google.com/"));
#else
            var headers = new HeaderDict();
            headers.Add("Referer", new StringValues("http://www.google.com/"));
            httpContext.Request.Headers.Returns(callinfo => headers);
#endif

            // Act
            var result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("http://www.google.com/", result);
        }
    }
}