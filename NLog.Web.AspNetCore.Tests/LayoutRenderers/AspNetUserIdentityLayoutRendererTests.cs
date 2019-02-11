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
    public class AspNetUserIdentityLayoutRendererTests : LayoutRenderersTestBase<AspNetUserIdentityLayoutRenderer>
    {
        [Fact]
        public void NullUserIdentityRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.User.Identity.Returns(null as IIdentity);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void UserIdentityNameRendersName()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var expectedResult = "value";
            var identity = Substitute.For<IIdentity>();
            identity.Name.Returns(expectedResult);
            httpContext.User.Identity.Returns(identity);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
