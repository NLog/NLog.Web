using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestReferrerRendererTests : TestBase
    {

        [Fact]
        public void NullReferrerRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetRequestReferrerRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void ReferrerPresentRenderNonEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.UrlReferrer.Returns(new Uri("http://www.google.com/"));
            var renderer = new AspNetRequestReferrerRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            
            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(result, "http://www.google.com/");
        }        
    }
}
