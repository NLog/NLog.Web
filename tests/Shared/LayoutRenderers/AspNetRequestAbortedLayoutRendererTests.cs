using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestAbortedLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestAbortedLayoutRenderer>
    {
        [Fact]
        public void RequestNotAborted()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var tokenSource = new System.Threading.CancellationTokenSource();
#if ASP_NET_CORE
            httpContext.RequestAborted.Returns(tokenSource.Token);
#endif
            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("0", result);
        }

#if ASP_NET_CORE
        [Fact]
        public void RequestAborted()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            var tokenSource = new System.Threading.CancellationTokenSource();
            tokenSource.Cancel();
            httpContext.RequestAborted.Returns(tokenSource.Token);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("1", result);
        }
#endif

#if !NET35
        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("0", result);
        }
#endif
    }
}
