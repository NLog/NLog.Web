using System;
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
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.AspNetCore.Hosting;
#endif
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetWebRootPathLayoutRendererTests
    {
        [Fact]
        public void SuccessTest()
        {
            var renderer = new AspNetWebRootPathLayoutRenderer();

#if !ASP_NET_CORE
            var hostEnvironment = new FakeHostEnvironment();
            hostEnvironment.MappedPath = "NLogTestContentRootPath";
#else
            var hostEnvironment = Substitute.For<IWebHostEnvironment>();
            hostEnvironment.WebRootPath.Returns("NLogTestContentRootPath");
#endif
            renderer.WebHostEnvironment = hostEnvironment;

            string actual = renderer.Render(new LogEventInfo());

            Assert.Equal("NLogTestContentRootPath", actual);
        }

        [Fact]
        public void NullTest()
        {
            var renderer = new AspNetWebRootPathLayoutRenderer();

#if !ASP_NET_CORE
            var hostEnvironment = new FakeHostEnvironment();
            hostEnvironment.MappedPath = null;
#else
            var hostEnvironment = Substitute.For<IWebHostEnvironment>();
            hostEnvironment.WebRootPath.ReturnsNull();
#endif
            renderer.WebHostEnvironment = hostEnvironment;
            string actual = renderer.Render(new LogEventInfo());

            Assert.Equal(string.Empty, actual);
        }
    }
}
