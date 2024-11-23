#if ASP_NET_CORE || NET46_OR_GREATER

using System;
using System.Collections.Generic;
using System.Security.Claims;
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

        [Theory]
        [InlineData("ClaimType.ObjectId", "oid")]
        [InlineData("ClaimType.TenantId", "tid")]
        [InlineData("ClaimType.AppId", "azp")]
        public void UserClaimTypeNameRendersAzureClaims(string claimType, string claimId)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.ClaimType = claimType;

            var identity = Substitute.For<System.Security.Claims.ClaimsIdentity>();
            identity.FindFirst(claimId).Returns(new System.Security.Claims.Claim(claimId, claimType));
            httpContext.User.Identity.Returns(identity);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(claimType, result);
        }

        [Fact]
        public void AllRendersAllValue()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var expectedResult = "http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor=ActorValue1,http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor=ActorValue2,http://schemas.xmlsoap.org/ws/2005/05/identity/claims/country=CountryValue";

            var principal = Substitute.For<System.Security.Claims.ClaimsPrincipal>();

            principal.Claims.Returns(new List<Claim>()
                {
                    new System.Security.Claims.Claim(ClaimTypes.Actor, "ActorValue1"),
                    new System.Security.Claims.Claim(ClaimTypes.Actor, "ActorValue2"),
                    new System.Security.Claims.Claim(ClaimTypes.Country, "CountryValue")
                }
            );

            httpContext.User.Returns(principal);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}

#endif