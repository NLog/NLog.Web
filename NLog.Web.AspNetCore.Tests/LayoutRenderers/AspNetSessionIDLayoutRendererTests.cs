#if !ASP_NET_CORE
//TODO test .NET Core
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
    public class AspNetSessionIDLayoutRendererTests : TestBase
    {
        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var renderer = new AspNetSessionIdLayoutRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void NullSessionRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Session.Returns(null as HttpSessionStateWrapper);

            var renderer = new AspNetSessionIdLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void AvailableSessionRendersSessionId()
        {
            var expectedResult = "value";
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Session.SessionID.Returns(expectedResult);

            var renderer = new AspNetSessionIdLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
    }
}
#endif