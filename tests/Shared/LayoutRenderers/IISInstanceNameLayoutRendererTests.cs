using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

#if !ASP_NET_CORE
using System.Web.Hosting;
using NLog.Web.Internal;
#else
#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class IISInstanceNameLayoutRendererTests
    {
        [Fact]
        public void SuccessTest()
        {
            var renderer = new IISInstanceNameLayoutRenderer();
            var hostEnvironment = new FakeHostEnvironment();

#if !ASP_NET_CORE
            hostEnvironment.SiteName = "NLogTestIISName";
#else
            hostEnvironment.ApplicationName = "NLogTestIISName";
#endif
            renderer.HostEnvironment = hostEnvironment;

            string actual = renderer.Render(new LogEventInfo());

            Assert.Equal("NLogTestIISName", actual);
        }

        [Fact]
        public void NullTest()
        {
            var renderer = new IISInstanceNameLayoutRenderer();
            var hostEnvironment = new FakeHostEnvironment();

#if !ASP_NET_CORE
            hostEnvironment.SiteName = null;
#else
            hostEnvironment.ApplicationName = null;
#endif
            renderer.HostEnvironment = hostEnvironment;

            string actual = renderer.Render(new LogEventInfo());

            Assert.Equal(string.Empty, actual);
        }
    }
}
