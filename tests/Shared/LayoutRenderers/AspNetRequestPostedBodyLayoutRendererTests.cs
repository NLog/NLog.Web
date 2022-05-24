using System.Collections.Generic;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestPostedBodyLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestPostedBodyLayoutRenderer>
    {
        [Fact]
        public void RequestPostedBodyPresentRenderNonEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            string expected = "This is a test of the request posted body layout renderer.";
            var items = new Dictionary<object, object>();
            items.Add(AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey, expected);
            httpContext.Items.Returns(items);
            // Act
            var result = renderer.Render(new LogEventInfo());
            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NullItemsRendersEmptyString()
        {
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Items.ReturnsNull();

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void EmptyItemsRendersEmptyString()
        {
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Items.Returns(new Dictionary<object, object>());

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void NonEmptyItemsWithoutPostedBodyRendersEmptyString()
        {
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Items.Returns(new Dictionary<object, object>
            {
                {AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey + "X","Not the Posted Body Value"}
            });

            string result = renderer.Render(new LogEventInfo());

            Assert.NotEmpty(httpContext.Items);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void NotStringTypeRendersEmptyString()
        {
            var (renderer, httpContext) = CreateWithHttpContext();

            httpContext.Items.Returns(new Dictionary<object, object>
            {
                {AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey, 42}
            });

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var (renderer, httpContext) = CreateWithHttpContext();

            renderer.HttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            renderer.HttpContextAccessor.HttpContext.ReturnsNull();

            string expected = "This is a test of the request posted body layout renderer.";
            var items = new Dictionary<object, object> {{AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey, expected}};
            httpContext.Items.Returns(items);

            // Act
            var result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(string.Empty, result);
        }
    }
}
