#if !ASP_NET_CORE
//TODO test .NET Core
using System.Collections.Specialized;
using System.IO;
#if !ASP_NET_CORE
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
#else
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
using NLog.Common;
using NLog.Config;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestValueLayoutRendererTests : TestBase
    {
        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var renderer = new AspNetRequestValueLayoutRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void NullRequestRendersEmptyStringWithoutLoggingError()
        {
            var internalLog = new StringWriter();
            InternalLogger.LogWriter = internalLog;
            InternalLogger.LogLevel = LogLevel.Error;

            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Returns(x => { throw new HttpException(); });

            var renderer = new AspNetRequestValueLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.Item = "key";

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
            Assert.Equal(true, string.IsNullOrEmpty(internalLog.ToString()));
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
#endif