using System;
using System.Collections.Generic;
using System.Globalization;
#if !ASP_NET_CORE
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;


namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetMvcActionRendererTests : LayoutRenderersTestBase<AspNetMvcActionLayoutRenderer>
    {
        [Fact]
        public void NullRoutesRenderersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Empty(result);
        }

#if ASP_NET_CORE
        [Fact]
        public void ActionKeyRendersRouteParameter()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            SetupRouteParameters(httpContext);

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("actionName", result);
        }

        private void SetupRouteParameters(HttpContext httpContext)
        {
            var collection = new FeatureCollection();
            var routeData = new RouteData();
            var routingFeature = Substitute.For<IRoutingFeature>();
            collection.Set(routingFeature);
#if NETCOREAPP3_0_OR_GREATER
            var routingValuesFeature = Substitute.For<IRouteValuesFeature>();
            routingValuesFeature.RouteValues.Returns(routeData.Values);
            collection.Set(routingValuesFeature);
#endif
            httpContext.Features.Returns(collection);
            
            routeData.Values.Add("action", "actionName");
            routeData.Values.Add("controller", "controllerName");
            routingFeature.RouteData.Returns(routeData);
        }
#endif
    }
}
