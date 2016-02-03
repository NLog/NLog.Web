using System.Web;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Web.LayoutRenderers;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestValueLayoutRenderTests : TestInvolvingAspNetHttpContext
    {
        public AspNetRequestValueLayoutRenderTests()
        {
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("aspnet-request", typeof(AspNetRequestValueLayoutRenderer));
        }

        protected override HttpRequest SetUpHttpRequest(string query = "")
        {
            var request = base.SetUpHttpRequest("a=b&num=4&odd");
            AddHeader(request, "content-type", "application/json");
            AddHeader(request, "null", null);
            return request;
        }

        [Fact]
        public void HeaderThatDoesNotExist()
        {
            var renderer = new AspNetRequestValueLayoutRenderer
            {
                Header = "junk"
            };
            ExecTest("", renderer);
        }

        [Fact]
        public void HeaderThatExistsButIsNull()
        {
            var renderer = new AspNetRequestValueLayoutRenderer
            {
                Header = "null"
            };
            ExecTest("", renderer);
        }

        [Fact]
        public void HeaderThatExistsWithValue()
        {
            var renderer = new AspNetRequestValueLayoutRenderer
            {
                Header = "content-type"
            };
            ExecTest("application/json", renderer);
        }

        [Fact]
        public void QuerystringValueThatDoesNotExist()
        {
            var renderer = new AspNetRequestValueLayoutRenderer
            {
                QueryString = "junk"
            };
            ExecTest("", renderer);
        }

        [Fact]
        public void QuerystringValueThatExistsButIsNull()
        {
            var renderer = new AspNetRequestValueLayoutRenderer
            {
                QueryString = "odd"
            };
            ExecTest("", renderer);
        }

        [Fact]
        public void QuerystringValueThatExistsWithValue()
        {
            var renderer = new AspNetRequestValueLayoutRenderer
            {
                QueryString = "a"
            };
            ExecTest("b", renderer);
        }

        [Fact]
        public void ViaLayoutHeaderThatDoesNotExist()
        {
            Layout layout = "${aspnet-request:header=junk}";
            ExecTest("", layout);
        }

        [Fact]
        public void ViaLayoutHeaderThatExistsButIsNull()
        {
            Layout layout = "${aspnet-request:header=null}";
            ExecTest("", layout);
        }

        [Fact]
        public void ViaLayoutHeaderThatExistsWithValue()
        {
            Layout layout = "${aspnet-request:header=content-type}";
            ExecTest("application/json", layout);
        }

        private void ExecTest(string expected, LayoutRenderer renderer)
        {
            var rendered = renderer.Render(LogEventInfo.CreateNullEvent());
            Assert.Equal(expected, rendered);
        }

        private void ExecTest(string expected, Layout layout)
        {
            var rendered = layout.Render(LogEventInfo.CreateNullEvent());
            Assert.Equal(expected, rendered);
        }
    }
}