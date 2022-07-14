using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestTrackingConsentLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestTrackingConsentLayoutRenderer>
    {
        [Fact]
        public void SuccessCanTrackTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = Enums.TrackingConsentProperty.CanTrack;

            var trackingConsent = Substitute.For<ITrackingConsentFeature>();

            trackingConsent.CanTrack.Returns(true);
            trackingConsent.HasConsent.Returns(false);
            trackingConsent.IsConsentNeeded.Returns(false);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<ITrackingConsentFeature>(trackingConsent);

            httpContext.Features.Returns(featureCollection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void SuccessHasConsentTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = Enums.TrackingConsentProperty.HasConsent;

            var trackingConsent = Substitute.For<ITrackingConsentFeature>();

            trackingConsent.CanTrack.Returns(false);
            trackingConsent.HasConsent.Returns(true);
            trackingConsent.IsConsentNeeded.Returns(false);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<ITrackingConsentFeature>(trackingConsent);

            httpContext.Features.Returns(featureCollection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void SuccessHasIsConsentNeededTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Property = Enums.TrackingConsentProperty.IsConsentNeeded;

            var trackingConsent = Substitute.For<ITrackingConsentFeature>();

            trackingConsent.CanTrack.Returns(false);
            trackingConsent.HasConsent.Returns(false);
            trackingConsent.IsConsentNeeded.Returns(true);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<ITrackingConsentFeature>(trackingConsent);

            httpContext.Features.Returns(featureCollection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("1", result);
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
            Assert.Equal("1", result);
        }

        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("0", result);
        }
    }
}
