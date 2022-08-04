using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetEnvironmentLayoutRendererTests
    {
        [Fact]
        public void SuccessTest()
        {
            var renderer = new AspNetEnvironmentLayoutRenderer();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.EnvironmentName.Returns("NLogTestEnvironmentName");

            renderer.HostEnvironment = hostEnvironment;
            string actual = renderer.Render(new LogEventInfo());
            Assert.Equal("NLogTestEnvironmentName", actual);
        }

        [Fact]
        public void NullTest()
        {
            var renderer = new AspNetEnvironmentLayoutRenderer();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.EnvironmentName.ReturnsNull();

            renderer.HostEnvironment = hostEnvironment;
            string actual = renderer.Render(new LogEventInfo());
            Assert.Equal(string.Empty, actual);
        }
    }
}
