using System.Security.Principal;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;
#if !ASP_NET_CORE
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetUserAuthTypeLayoutRendererTests : LayoutRenderersTestBase<AspNetUserAuthTypeLayoutRenderer>
    {
        [Fact]
        public void AuthenticatedUserRendersAuthenticationType()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var expectedResult = "value";
            SetIIdentity(expectedResult, httpContext, true);

            // Act
            var result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void NullUserIdentityRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.User.Identity.Returns(null as IIdentity);

            // Act
            var result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void UnauthenticatedUserRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            SetIIdentity(null, httpContext, false);

            // Act
            var result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Empty(result);
        }

        private static void SetIIdentity(string expectedResult, HttpContextBase httpContext, bool isAuthenticated)
        {
            var identity = Substitute.For<IIdentity>();
            identity.IsAuthenticated.Returns(isAuthenticated);
            identity.AuthenticationType.Returns(expectedResult);
            httpContext.User.Identity.Returns(identity);
        }
    }
}