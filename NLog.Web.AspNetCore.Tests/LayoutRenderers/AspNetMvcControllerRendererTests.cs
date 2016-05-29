using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;
using System.Web.Routing;
using NLog.Targets;
using NLog.Layouts;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetMvcControllerRendererTests
    {
        [Fact]
        public void NullRoutesRenderersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetMvcControllerRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }
    }
}
