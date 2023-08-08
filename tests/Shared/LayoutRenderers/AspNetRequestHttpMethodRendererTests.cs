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
    public class AspNetRequestMethodLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestMethodLayoutRenderer>
    {
        [Fact]
        public void HttpMethod_Set_Renderer()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

#if ASP_NET_CORE
            httpContext.Request.Method.Returns("POST");
#else
            httpContext.Request.HttpMethod.Returns("POST");
#endif
            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("POST", result);
        }
    }
}
