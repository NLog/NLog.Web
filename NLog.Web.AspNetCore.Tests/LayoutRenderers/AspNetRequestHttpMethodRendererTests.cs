using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
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
            httpContext.Request.HttpMethod.Returns("POST");

            var renderer = new AspNetRequestHttpMethodRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);


            string result = renderer.Render(new LogEventInfo());

            Assert.Equal("POST", result);
        }
    }
}
