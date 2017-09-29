using System.Security.Principal;
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
    public class AspNetUserIdentityLayoutRendererTests : TestBase
    {
        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var renderer = new AspNetUserIdentityLayoutRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void NullUserIdentityRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.User.Identity.Returns(null as IIdentity);

            var renderer = new AspNetUserIdentityLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }


        [Fact]
        public void UserIdentityNameRendersName()
        {
            var expectedResult = "value";
            var httpContext = Substitute.For<HttpContextBase>();
            var identity = Substitute.For<IIdentity>();
            identity.Name.Returns(expectedResult);
            httpContext.User.Identity.Returns(identity);

            var renderer = new AspNetUserIdentityLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
    }
}
