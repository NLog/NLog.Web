using System;
using System.Linq;
using NLog.Layouts;
using NLog.Web.LayoutRenderers;
using NSubstitute;
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
    public class IISInstanceNameLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void SuccessTest()
        {
            var renderer = new IISInstanceNameLayoutRenderer();
            var hostEnvironment = Substitute.For<IHostEnvironment>();

#if !ASP_NET_CORE
            hostEnvironment.SiteName.Returns("NLogTestIISName");
#else
            hostEnvironment.ApplicationName.Returns("NLogTestIISName");
#endif
            renderer.HostEnvironment = hostEnvironment;

            string actual = renderer.Render(new LogEventInfo());

            Assert.Equal("NLogTestIISName", actual);
        }
    }
}
