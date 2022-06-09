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
    public class AspNetRequestUserAgentTests : LayoutRenderersTestBase<AspNetRequestUserAgentLayoutRenderer>
    {
        [Fact]
        public void NotNullUserAgentRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();


#if !ASP_NET_CORE
             httpContext.Request.UserAgent.Returns("TEST");
#else
            var headers = new HeaderDict {{"User-Agent", new StringValues("TEST")}};
            httpContext.Request.Headers.Returns((callinfo) => headers);
#endif
            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("TEST", result);
        }
    }
}
