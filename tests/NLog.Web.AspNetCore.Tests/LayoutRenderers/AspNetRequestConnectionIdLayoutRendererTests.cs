using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestConnectionIdLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestConnectionIdLayoutRenderer>
    {
        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Connection.Id.Returns("My Connection Id");
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("My Connection Id", result);
        }

        [Fact]
        public void EmptyTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Connection.Id.Returns("");
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void NullTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Connection.Id.ReturnsNull();
            // Act
            string result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal("", result);
        }
    }
}
