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
    public class AspNetUserAuthTypeLayoutRendererTests : TestBase
    {
        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var renderer = new AspNetUserAuthTypeLayoutRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void NullUserIdentityRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.User.Identity.Returns(null as IIdentity);

            var renderer = new AspNetUserAuthTypeLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void UnauthenticatedUserRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var identity = Substitute.For<IIdentity>();
            identity.IsAuthenticated.Returns(false);
            httpContext.User.Identity.Returns(identity);

            var renderer = new AspNetUserAuthTypeLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void AuthenticatedUserRendersAuthenticationType()
        {
            var expectedResult = "value";
            var httpContext = Substitute.For<HttpContextBase>();
            var identity = Substitute.For<IIdentity>();
            identity.IsAuthenticated.Returns(true);
            identity.AuthenticationType.Returns(expectedResult);
            httpContext.User.Identity.Returns(identity);

            var renderer = new AspNetUserAuthTypeLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
    }
}
