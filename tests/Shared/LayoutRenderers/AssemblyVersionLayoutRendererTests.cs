using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog.Layouts;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AssemblyVersionLayoutRendererTests : TestBase
    {
        [Fact]
        public void AssemblyNameVersionTest()
        {
#if ASP_NET_CORE
            Layout layout = "${assembly-version:NLog.Web.AspNetCore.Tests}";
#else
            Layout layout = "${assembly-version:NLog.Web.Tests}";
#endif
            var result = layout.Render(LogEventInfo.CreateNullEvent());
            Assert.Equal("1.2.3.0", result);
        }
        
        [Fact]
        public void AssemblyNameFileVersionTest()
        {
#if ASP_NET_CORE
            Layout layout = "${assembly-version:name=NLog.Web.AspNetCore.Tests:type=file}";
#else
            Layout layout = "${assembly-version:NLog.Web.Tests:type=file}";
#endif
            var result = layout.Render(LogEventInfo.CreateNullEvent());
            Assert.Equal("1.2.3.1", result);
        }
        
        [Fact]
        public void AssemblyNameInformationalVersionTest()
        {
#if ASP_NET_CORE
            Layout layout = "${assembly-version:name=NLog.Web.AspNetCore.Tests:type=informational}";
#else
            Layout layout = "${assembly-version:NLog.Web.Tests:type=informational}";
#endif
            var result = layout.Render(LogEventInfo.CreateNullEvent());
            Assert.Equal("1.2.3.2", result);
        }
    }
}
