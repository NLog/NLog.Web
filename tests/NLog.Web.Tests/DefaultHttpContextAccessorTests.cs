#if !ASP_NET_CORE
using System.IO;
using System.Text;
using System.Web;
using NLog.Web.Tests.LayoutRenderers;
using Xunit;

namespace NLog.Web.Tests
{
    public class DefaultHttpContextAccessorTests: TestBase
    {
        [Fact]
        public void UnavailableHttpContextReturnsNull()
        {
            var httpContextAccessor = new DefaultHttpContextAccessor();
            Assert.Null(httpContextAccessor.HttpContext);
        }

        [Fact]
        public void AvailableHttpContextIsReturned()
        {
            var httpContextAccessor = new DefaultHttpContextAccessor();
            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "http://nlog-project.org", ""), 
                new HttpResponse(new StringWriter(new StringBuilder()))
            );

            Assert.NotNull(httpContextAccessor.HttpContext);
        }
    }
}
#endif