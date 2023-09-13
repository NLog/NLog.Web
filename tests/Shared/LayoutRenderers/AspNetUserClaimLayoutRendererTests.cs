#if ASP_NET_CORE || NET46_OR_GREATER

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using NLog.Web.Enums;
#if ASP_NET_CORE
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;
using static System.Net.WebRequestMethods;

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

        [Fact]
        public void VerboseMultipleFlatTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.Verbose = true;

            var expectedResult =
                "Type=http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor,Value=Actorvalue,ValueType=Actorstring,Issuer=Actorissuer,OriginalIssuer=ActororiginalIssuer,Properties[claim1property1=claim1value1,claim1property2=claim1value2];Type=http://schemas.xmlsoap.org/ws/2005/05/identity/claims/anonymous,Value=Anonymousvalue,ValueType=Anonymousstring,Issuer=Anonymousissuer,OriginalIssuer=AnonymousoriginalIssuer;Type=http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication,Value=Authenticationvalue,ValueType=Authenticationstring,Issuer=Authenticationissuer,OriginalIssuer=AuthenticationoriginalIssuer";

            var principal = Substitute.For<ClaimsPrincipal>();

            var claim1 = new Claim(ClaimTypes.Actor, "Actorvalue", "Actorstring", "Actorissuer", "ActororiginalIssuer");
            var claim2 = new Claim(ClaimTypes.Anonymous, "Anonymousvalue", "Anonymousstring", "Anonymousissuer", "AnonymousoriginalIssuer");
            var claim3 = new Claim(ClaimTypes.Authentication, "Authenticationvalue", "Authenticationstring", "Authenticationissuer", "AuthenticationoriginalIssuer");

            claim1.Properties.Add("claim1property1","claim1value1");
            claim1.Properties.Add("claim1property2", "claim1value2");

            principal.Claims.Returns(new List<Claim>()
                {
                    claim1, claim2, claim3
                }
            );

            httpContext.User.Returns(principal);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void VerboseMultipleJsonArrayTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;
            renderer.Verbose = true;

            var expectedResult =
                "[{\"Type\":\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor\",\"Value\":\"Actorvalue\",\"ValueType\":\"Actorstring\",\"Issuer\":\"Actorissuer\",\"OriginalIssuer\":\"ActororiginalIssuer\",\"Properties\":[{\"claim1property1\":\"claim1value1\"},{\"claim1property2\":\"claim1value2\"}]},{\"Type\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/anonymous\",\"Value\":\"Anonymousvalue\",\"ValueType\":\"Anonymousstring\",\"Issuer\":\"Anonymousissuer\",\"OriginalIssuer\":\"AnonymousoriginalIssuer\"},{\"Type\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication\",\"Value\":\"Authenticationvalue\",\"ValueType\":\"Authenticationstring\",\"Issuer\":\"Authenticationissuer\",\"OriginalIssuer\":\"AuthenticationoriginalIssuer\"}]";

            var principal = Substitute.For<ClaimsPrincipal>();

            var claim1 = new Claim(ClaimTypes.Actor, "Actorvalue", "Actorstring", "Actorissuer", "ActororiginalIssuer");
            var claim2 = new Claim(ClaimTypes.Anonymous, "Anonymousvalue", "Anonymousstring", "Anonymousissuer", "AnonymousoriginalIssuer");
            var claim3 = new Claim(ClaimTypes.Authentication, "Authenticationvalue", "Authenticationstring", "Authenticationissuer", "AuthenticationoriginalIssuer");

            claim1.Properties.Add("claim1property1", "claim1value1");
            claim1.Properties.Add("claim1property2", "claim1value2");

            principal.Claims.Returns(new List<Claim>()
                {
                    claim1, claim2, claim3
                }
            );

            httpContext.User.Returns(principal);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void VerboseMultipleJsonDictionaryTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonDictionary;
            renderer.Verbose = true;

            var expectedResult =
                "{{\"Type\":\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor\",\"Value\":\"Actorvalue\",\"ValueType\":\"Actorstring\",\"Issuer\":\"Actorissuer\",\"OriginalIssuer\":\"ActororiginalIssuer\",\"Properties\":{\"claim1property1\":\"claim1value1\",\"claim1property2\":\"claim1value2\"}},{\"Type\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/anonymous\",\"Value\":\"Anonymousvalue\",\"ValueType\":\"Anonymousstring\",\"Issuer\":\"Anonymousissuer\",\"OriginalIssuer\":\"AnonymousoriginalIssuer\"},{\"Type\":\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication\",\"Value\":\"Authenticationvalue\",\"ValueType\":\"Authenticationstring\",\"Issuer\":\"Authenticationissuer\",\"OriginalIssuer\":\"AuthenticationoriginalIssuer\"}}";

            var principal = Substitute.For<ClaimsPrincipal>();

            var claim1 = new Claim(ClaimTypes.Actor, "Actorvalue", "Actorstring", "Actorissuer", "ActororiginalIssuer");
            var claim2 = new Claim(ClaimTypes.Anonymous, "Anonymousvalue", "Anonymousstring", "Anonymousissuer", "AnonymousoriginalIssuer");
            var claim3 = new Claim(ClaimTypes.Authentication, "Authenticationvalue", "Authenticationstring", "Authenticationissuer", "AuthenticationoriginalIssuer");

            claim1.Properties.Add("claim1property1", "claim1value1");
            claim1.Properties.Add("claim1property2", "claim1value2");

            principal.Claims.Returns(new List<Claim>()
                {
                    claim1, claim2, claim3
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