using System;
using System.Linq;
using NLog.Layouts;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AssemblyVersionLayoutRendererTests : TestBase
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
    }
}
