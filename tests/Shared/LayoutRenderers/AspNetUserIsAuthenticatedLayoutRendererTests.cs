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
    public class AspNetUserIsAuthenticatedLayoutRendererTests : LayoutRenderersTestBase<AspNetUserIsAuthenticatedLayoutRenderer>
    {
        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("0", result);
        }

        [Fact]
        public void UnauthenticatedUserRendersZero()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            SetIIdentity(null, httpContext, false);

            // Act
            var result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("0", result);
        }

        [Fact]
        public void AuthenticatedUserRendersOne()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            SetIIdentity(null, httpContext, true);

            // Act
            var result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("1", result);
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