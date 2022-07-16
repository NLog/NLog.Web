#if ASP_NET_CORE3
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetConnectionHttpTransportTypeLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestHttpTransportTypeLayoutRenderer>
    {
        [Fact]
        public void SuccessNoneTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var httpTransportFeature = Substitute.For<IHttpTransportFeature>();
            httpTransportFeature.TransportType.Returns(HttpTransportType.None);

            var collection = new FeatureCollection();
            collection.Set<IHttpTransportFeature>(httpTransportFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(HttpTransportType.None.ToString(), result);
        }

        [Fact]
        public void SuccessWebSocketTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var httpTransportFeature = Substitute.For<IHttpTransportFeature>();
            httpTransportFeature.TransportType.Returns(HttpTransportType.WebSockets);

            var collection = new FeatureCollection();
            collection.Set<IHttpTransportFeature>(httpTransportFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(HttpTransportType.WebSockets.ToString(), result);
        }

        [Fact]
        public void SuccessServerSentEventsTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var httpTransportFeature = Substitute.For<IHttpTransportFeature>();
            httpTransportFeature.TransportType.Returns(HttpTransportType.ServerSentEvents);

            var collection = new FeatureCollection();
            collection.Set<IHttpTransportFeature>(httpTransportFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(HttpTransportType.ServerSentEvents.ToString(), result);
        }

        [Fact]
        public void SuccessLongPollingTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var httpTransportFeature = Substitute.For<IHttpTransportFeature>();
            httpTransportFeature.TransportType.Returns(HttpTransportType.LongPolling);

            var collection = new FeatureCollection();
            collection.Set<IHttpTransportFeature>(httpTransportFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(HttpTransportType.LongPolling.ToString(), result);
        }

        [Fact]
        public void NullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var collection = new FeatureCollection();
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(HttpTransportType.None.ToString(), result);
        }

        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("None", result);
        }
    }
}
#endif