using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestUserAgentTests
    {
        [Fact]
        public void NullUserAgentRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetRequestUserAgent();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void NotNullUserAgentRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.UserAgent.Returns("TEST");
            var renderer = new AspNetRequestUserAgent();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, "TEST");
        }
    }
}
