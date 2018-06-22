using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog.Layouts;
using NLog.Web.Tests.LayoutRenderers;
using Xunit;

namespace NLog.Web.AspNetCore.Tests.LayoutRenderers
{
    public class AssemblyVersionLayoutRendererTests : TestBase
    {
        [Fact]
        public void AssemblyNameVersionTest()
        {
            Layout layout = "${assembly-version:NLog.Web.AspNetCore.Tests}";
            var result = layout.Render(LogEventInfo.CreateNullEvent());
            Assert.Equal("1.2.3.0", result);
        }
        
        [Fact]
        public void AssemblyNameFileVersionTest()
        {
            Layout layout = "${assembly-version:name=NLog.Web.AspNetCore.Tests:type=file}";
            var result = layout.Render(LogEventInfo.CreateNullEvent());
            Assert.Equal("1.2.3.1", result);
        }
        
        [Fact]
        public void AssemblyNameInformationalVersionTest()
        {
            Layout layout = "${assembly-version:name=NLog.Web.AspNetCore.Tests:type=informational}";
            var result = layout.Render(LogEventInfo.CreateNullEvent());
            Assert.Equal("1.2.3.2", result);
        }
    }
}
