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
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.Features;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;


namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetMvcControllerRendererTests : LayoutRenderersTestBase<AspNetMvcControllerRenderer>
    {
        [Fact]
        public void NullRoutesRenderersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            AddRoutingFeature(httpContext);
            
            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Empty(result);
        }
    }
}
