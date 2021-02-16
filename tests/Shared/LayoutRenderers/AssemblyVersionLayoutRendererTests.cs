using System;
using System.Linq;
using NLog.Layouts;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AssemblyVersionLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
#if ASP_NET_CORE
        private const string AssemblyName = "NLog.Web.AspNetCore.Tests";
#else
        private const string AssemblyName = "NLog.Web.Tests";
#endif

        [Fact]
        public void AssemblyNameVersionTest()
        {
            Layout layout = "${assembly-version:" + AssemblyName + "}";

            var result = layout.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("1.2.3.0", result);
        }

        [Fact]
        public void AssemblyNameFileVersionTest()
        {
            Layout layout = "${assembly-version:" + AssemblyName + ":type=file}";

            var result = layout.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("1.2.3.1", result);
        }

        [Fact]
        public void AssemblyNameInformationalVersionTest()
        {
            Layout layout = "${assembly-version:" + AssemblyName + ":type=informational}";

            var result = layout.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal("1.2.3.2", result);
        }

#if !ASP_NET_CORE
        class TestAplication : System.Web.HttpApplication
        {
        }

        [Fact]
        public void HttpContextAssemblyNameVersionTest()
        {
            HttpContext.ApplicationInstance = new TestAplication();

            var logFactory = new LogFactory().Setup().SetupExtensions(evt => evt.RegisterAssembly(typeof(IHttpContextAccessor).Assembly)).LoadConfigurationFromXml(@"
                <nlog throwConfigExceptions='true'>
                    <targets async='true'>
                        <target name='memory' type='memory' layout='${assembly-version}' />
                    </targets>
                    <rules>
                        <logger name='*' writeTo='memory' />
                    </rules>
                </nlog>
            ").LogFactory;
            var target = logFactory.Configuration.AllTargets.OfType<NLog.Targets.MemoryTarget>().First();
            var logger = logFactory.GetCurrentClassLogger();
            logger.Info("Hello");
            logFactory.Flush();
            Assert.Equal("1.2.3.0", target.Logs.First());
        }
#endif
    }
}
