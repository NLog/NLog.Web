using System;
using System.Collections.Generic;
using NLog.Web.LayoutRenderers;
using Xunit;
using NSubstitute;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestRouteParametersLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestRouteParametersLayoutRenderer>
    {
#if ASP_NET_CORE
        [Fact]
        public void NullKeyRendersAllRouteParameters()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Items = null;

            SetupRouteParameters(httpContext);

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("key1=value1,key2=value2", result);
        }

        [Fact]
        public void SingleKeyRendersRouteParameter()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Items = new List<string> { "key2" };

            SetupRouteParameters(httpContext);

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("key2=value2", result);
        }

        private void SetupRouteParameters(HttpContext httpContext)
        {
            var routeData = new RouteData();
            var routingFeature = Substitute.For<IRoutingFeature>();
            var collection = new FeatureCollection();
            collection.Set(routingFeature);
#if NETCOREAPP3_0_OR_GREATER
            var routingValuesFeature = Substitute.For<IRouteValuesFeature>();
            routingValuesFeature.RouteValues.Returns(routeData.Values);
            collection.Set(routingValuesFeature);
#endif
            httpContext.Features.Returns(collection);

            routeData.Values.Add("key1", "value1");
            routeData.Values.Add("key2", "value2");
            routingFeature.RouteData.Returns(routeData);
        }
#endif
    }
}
