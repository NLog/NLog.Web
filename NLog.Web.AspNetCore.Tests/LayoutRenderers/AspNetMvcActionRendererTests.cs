using System;
using System.Collections.Generic;
using System.Globalization;
#if !NETSTANDARD_1plus
using System.Web;
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
    public class AspNetMvcActionRendererTests : TestBase
    {
        [Fact]
        public void NullRoutesRenderersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetMvcActionRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }        
    }
}
