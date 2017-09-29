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
    public class AspNetMvcControllerRendererTests : TestBase
    {
        [Fact]
        public void NullRoutesRenderersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
#if ASP_NET_CORE
            var routingFeature = Substitute.For<IRoutingFeature>();
            var collection = new FeatureCollection();
            collection.Set<IRoutingFeature>(routingFeature);
            httpContext.Features.Returns(collection);
#endif
            var renderer = new AspNetMvcControllerRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            string result = renderer.Render(LogEventInfo.CreateNullEvent());
            Assert.Empty(result);
        }
    }
}
