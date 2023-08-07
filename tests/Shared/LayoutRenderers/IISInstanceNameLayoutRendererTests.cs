using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

#if !ASP_NET_CORE
using System.Web.Hosting;
using NLog.Web.Internal;
#elif NETCOREAPP3_0_OR_GREATER
using Microsoft.Extensions.Hosting;
#else
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class IISInstanceNameLayoutRendererTests : TestBase
    {
        [Fact]
        public void SuccessTest()
        {
            var renderer = new IISSiteNameLayoutRenderer();

#if !ASP_NET_CORE
            var hostEnvironment = new FakeHostEnvironment();
            hostEnvironment.SiteName = "NLogTestIISName";
#else
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ApplicationName.Returns("NLogTestIISName");
#endif
            renderer.HostEnvironment = hostEnvironment;

            string actual = renderer.Render(new LogEventInfo());

            Assert.Equal("NLogTestIISName", actual);
        }

        [Fact]
        public void NullTest()
        {
            var renderer = new IISSiteNameLayoutRenderer();

#if !ASP_NET_CORE
            var hostEnvironment = new FakeHostEnvironment();
            hostEnvironment.SiteName = null;
#else
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ApplicationName = null;
#endif
            renderer.HostEnvironment = hostEnvironment;

            string actual = renderer.Render(new LogEventInfo());

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void InitCloseTest()
        {
            var logFactory = new LogFactory().Setup().RegisterNLogWeb().LoadConfiguration(builder =>
            {
                builder.ForTarget().WriteTo(new NLog.Targets.MemoryTarget() { Layout = "${iis-site-name}" });
            }).LogFactory;
            Assert.NotNull(logFactory);
            logFactory.Shutdown();
        }
    }
}
