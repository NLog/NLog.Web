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
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetAppBasePathLayoutRendererTests 
    {
        [Fact]
        public void SuccessTest()
        {
            var renderer = new AspNetAppBasePathLayoutRenderer();
            var hostEnvironment = new FakeHostEnvironment();

#if !ASP_NET_CORE
            hostEnvironment.MappedPath = "NLogTestContentRootPath";
#else
            hostEnvironment.ContentRootPath= "NLogTestContentRootPath";
#endif
            renderer.HostEnvironment = hostEnvironment;

            string actual = renderer.Render(new LogEventInfo());

            Assert.Equal("NLogTestContentRootPath", actual);
        }

        [Fact]
        public void NullTest()
        {
            var renderer = new AspNetAppBasePathLayoutRenderer();
            var hostEnvironment = new FakeHostEnvironment();

#if !ASP_NET_CORE
            hostEnvironment.MappedPath = null;
#else
            hostEnvironment.ContentRootPath = null;
#endif
            renderer.HostEnvironment = hostEnvironment;

            string actual = renderer.Render(new LogEventInfo());

#if !ASP_NET_CORE
            Assert.Equal(System.IO.Directory.GetCurrentDirectory(), actual);
#else
            Assert.Equal(AppContext.BaseDirectory, actual);
#endif
        }
    }
}
