#if !ASP_NET_CORE
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetTraceIdentifierRendererTests : TestBase
    {
        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var renderer = new AspNetTraceIdentifierLayoutRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void EmptyGuidRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();
#if ASP_NET_CORE
            httpContext.TraceIdentifier.Returns(null as string);
#else 
            var httpWorker = Substitute.For<HttpWorkerRequest>();
            httpContext.GetService(typeof(System.Web.HttpWorkerRequest)).Returns(httpWorker);
#endif
            var renderer = new AspNetTraceIdentifierLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            string result = renderer.Render(new LogEventInfo());
            Assert.Empty(result);
        }

        [Fact]
        public void AvailableTraceIdentifierRendersGuid()
        {
            var expectedResult = System.Guid.NewGuid();
            var httpContext = Substitute.For<HttpContextBase>();
#if ASP_NET_CORE
            httpContext.TraceIdentifier.Returns(expectedResult.ToString());
#else
            var httpWorker = Substitute.For<HttpWorkerRequest>();
            httpWorker.RequestTraceIdentifier.Returns(expectedResult);
            httpContext.GetService(typeof(System.Web.HttpWorkerRequest)).Returns(httpWorker);
#endif
            var renderer = new AspNetTraceIdentifierLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult.ToString(), result);
        }
    }
}