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
using Microsoft.AspNetCore.Http.Features;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;

using Xunit;

using System.IO;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestHostLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestHostLayoutRenderer>
    {
        [Fact]
        public void RequestHostTest()
        {
            var renderer = CreateRenderer("nlog-project.org");

            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("nlog-project.org", result);
        }

        private static AspNetRequestHostLayoutRenderer CreateRenderer(string hostBase)
        {
            var (renderer, httpContext) = CreateWithHttpContext();

#if !ASP_NET_CORE
            httpContext.Request.UserHostName.Returns(hostBase);
#else
            httpContext.Request.Host.Returns(new HostString(hostBase));
#endif
            return renderer;
        }
    }
}
