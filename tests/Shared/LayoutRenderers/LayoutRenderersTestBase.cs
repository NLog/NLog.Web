using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#else
using System.Web;
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public abstract class LayoutRenderersTestBase<TLayoutRenderer> : TestBase
        where TLayoutRenderer : AspNetLayoutRendererBase, new()
    {
        [Fact]
        public void NullRendersEmptyStringTest()
        {
            NullRendersEmptyString();
        }

        protected virtual void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Empty(result);
        }

        protected static (TLayoutRenderer renderer, HttpContextBase httpContext) CreateWithHttpContext()
        {
            var httpContext = Substitute.For<HttpContextBase>();
#if ASP_NET_CORE
            var collection = new FeatureCollection();
            var routingFeature = Substitute.For<IRoutingFeature>();
            routingFeature.RouteData.Returns(new RouteData());
            collection.Set<IRoutingFeature>(routingFeature);
            var sessionFeature = Substitute.For<ISessionFeature>();
            sessionFeature.Session.Returns(Substitute.For<ISession>());
            collection.Set<ISessionFeature>(sessionFeature);
            httpContext.Features.Returns(collection);
#endif
#if NETCOREAPP3_0_OR_GREATER
            httpContext.Request.RouteValues.Returns(new RouteValueDictionary());
#endif
            var renderer = new TLayoutRenderer
            {
                HttpContextAccessor = new FakeHttpContextAccessor(httpContext)
            };
            return (renderer, httpContext);
        }
    }
}