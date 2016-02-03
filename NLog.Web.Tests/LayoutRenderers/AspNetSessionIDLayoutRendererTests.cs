using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetSessionIDLayoutRendererTests
    {
        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var renderer = new AspNetSessionIDLayoutRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void NullSessionRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Session.Returns(null as HttpSessionStateWrapper);

            var renderer = new AspNetSessionIDLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void AvailableSessionRendersSessionId()
        {
            var expectedResult = "value";
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Session.SessionID.Returns(expectedResult);

            var renderer = new AspNetSessionIDLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
    }
}
