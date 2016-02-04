using System.Collections.Specialized;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestValueLayoutRendererTests
    {
        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var renderer = new AspNetRequestValueLayoutRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        public class ItemTests
        {
            [Fact]
            public void NullKeyRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Item = null;

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyNotFoundRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Item = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyFoundRendersValue()
            {
                var expectedResult = "value";
                var httpContext = Substitute.For<HttpContextBase>();
                httpContext.Request["key"].Returns(expectedResult);

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Item = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
        }

        public class QueryStringTests
        {
            [Fact]
            public void NullKeyRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.QueryString = null;

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyNotFoundRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.QueryString = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyFoundRendersValue()
            {
                var expectedResult = "value";
                var httpContext = Substitute.For<HttpContextBase>();
                httpContext.Request.QueryString.Returns(new NameValueCollection { {"key", expectedResult} });

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.QueryString = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
        }

        public class HeaderTests
        {
            [Fact]
            public void NullKeyRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Header = null;

                var result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyNotFoundRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Header = "key";

                var result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyFoundRendersValue()
            {
                var expectedResult = "value";
                var httpContext = Substitute.For<HttpContextBase>();
                httpContext.Request.Headers.Returns(new NameValueCollection { { "key", expectedResult } });

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Header = "key";

                var result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
        }

        public class FormTests
        {
            [Fact]
            public void NullKeyRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Form = null;

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyNotFoundRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Form = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyFoundRendersValue()
            {
                var expectedResult = "value";
                var httpContext = Substitute.For<HttpContextBase>();
                httpContext.Request.Form.Returns(new NameValueCollection { { "key", expectedResult } });

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Form = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
        }

        public class ServerVariablesTests
        {
            [Fact]
            public void NullKeyRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.ServerVariable = null;

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyNotFoundRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.ServerVariable = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyFoundRendersValue()
            {
                var expectedResult = "value";
                var httpContext = Substitute.For<HttpContextBase>();
                httpContext.Request.ServerVariables.Returns(new NameValueCollection { { "key", expectedResult } });

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.ServerVariable = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
        }

        public class CookieTests
        {
            [Fact]
            public void NullKeyRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Cookie = null;

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyNotFoundRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Cookie = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyFoundRendersValue()
            {
                var expectedResult = "value";
                var httpContext = Substitute.For<HttpContextBase>();
                httpContext.Request.Cookies.Returns(new HttpCookieCollection {new HttpCookie("key", expectedResult) });

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Cookie = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
        }
    }
}
