﻿#if NETCOREAPP3_0_OR_GREATER
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetConnectionInherentKeepAliveLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestInherentKeepAliveLayoutRenderer>
    {
        [Fact]
        public void SuccessTrueTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var keepAliveFeature = Substitute.For<IConnectionInherentKeepAliveFeature>();
            keepAliveFeature.HasInherentKeepAlive.Returns(true);

            var collection = new FeatureCollection();
            collection.Set<IConnectionInherentKeepAliveFeature>(keepAliveFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("1", result);
        }

        [Fact]
        public void SuccessFalseTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var keepAliveFeature = Substitute.For<IConnectionInherentKeepAliveFeature>();
            keepAliveFeature.HasInherentKeepAlive.Returns(false);

            var collection = new FeatureCollection();
            collection.Set<IConnectionInherentKeepAliveFeature>(keepAliveFeature);
            httpContext.Features.Returns(collection);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("0", result);
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
            Assert.Equal("0", result);
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
#endif
