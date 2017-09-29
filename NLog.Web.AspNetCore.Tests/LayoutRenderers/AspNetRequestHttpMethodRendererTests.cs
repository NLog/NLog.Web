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
    public class AspNetRequestHttpMethodRendererTests : TestBase
    {
        [Fact]
        public void NullUrlRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            
            var renderer = new AspNetRequestHttpMethodRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void HttpMethod_Set_Renderer()
        {
            var httpContext = Substitute.For<HttpContextBase>();
#if ASP_NET_CORE
            httpContext.Request.Method.Returns("POST");
#else
            httpContext.Request.HttpMethod.Returns("POST");
#endif

            var renderer = new AspNetRequestHttpMethodRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);


            string result = renderer.Render(new LogEventInfo());

            Assert.Equal("POST", result);
        }
    }
}
