#if ASP_NET_CORE || NET46_OR_GREATER

using System.Security.Principal;
#if ASP_NET_CORE
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetUserClaimLayoutRendererTests : LayoutRenderersTestBase<AspNetUserClaimLayoutRenderer>
    {
        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.ClaimType = string.Empty;
            httpContext.User.Identity.Returns(null as IIdentity);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Empty(result);

            // Bonus assert
            renderer.ClaimType = null;
            result = renderer.Render(new LogEventInfo());
            Assert.Empty(result);
        }

        [Fact]
        public void NullUserIdentityRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.ClaimType = "Name";
            httpContext.User.Identity.Returns(null as IIdentity);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void UserClaimTypeNameRendersValue()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.ClaimType = "ClaimType.Name";

            var expectedResult = "value";
            var expectedName = System.Security.Claims.ClaimTypes.Name;
            var identity = Substitute.For<System.Security.Claims.ClaimsIdentity>();
            identity.FindFirst(expectedName).Returns(new System.Security.Claims.Claim(expectedName, expectedResult));
            httpContext.User.Identity.Returns(identity);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}

#endif