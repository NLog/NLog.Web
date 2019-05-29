using NLog.Web.LayoutRenderers;
using NLog.Web.Tests;
using NSubstitute;
using Xunit;
#if ASP_NET_CORE
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
        public void NullRendersEmptyString()
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

            var renderer = new TLayoutRenderer
            {
                HttpContextAccessor = new FakeHttpContextAccessor(httpContext)
            };
            return (renderer, httpContext);
        }

        protected static void AddRoutingFeature(HttpContextBase httpContext)
        {
#if ASP_NET_CORE
            var routingFeature = Substitute.For<IRoutingFeature>();
            var collection = new FeatureCollection();
            collection.Set<IRoutingFeature>(routingFeature);
            httpContext.Features.Returns(collection);
#endif
        }
    }
}