using System;
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
    public class AspNetTraceIdentifierRendererTests : LayoutRenderersTestBase<AspNetTraceIdentifierLayoutRenderer>
    {
        [Fact]
        public void EmptyGuidRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            SetTraceIdentifier(httpContext, null);
            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AvailableTraceIdentifierRendersGuid()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var expectedResult = System.Guid.NewGuid();
            SetTraceIdentifier(httpContext, expectedResult);
            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult.ToString(), result);
        }

        private static void SetTraceIdentifier(HttpContextBase httpContext, Guid? expectedResult)
        {
#if ASP_NET_CORE
            httpContext.TraceIdentifier.Returns(expectedResult?.ToString());
#else
            var httpWorker = Substitute.For<HttpWorkerRequest>();
            if (expectedResult.HasValue)
                httpWorker.RequestTraceIdentifier.Returns(expectedResult.Value);
            httpContext.GetService(typeof(System.Web.HttpWorkerRequest)).Returns(httpWorker);
#endif
        }
    }
}