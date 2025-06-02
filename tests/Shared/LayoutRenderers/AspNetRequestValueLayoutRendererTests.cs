using System.IO;
#if !ASP_NET_CORE
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
using NLog.Common;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using System.Collections.Generic;


namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestValueLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var renderer = new AspNetRequestValueLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor((HttpContext)null);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

#if !ASP_NET_CORE
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
            Assert.True(string.IsNullOrEmpty(internalLog.ToString()));
        }
#endif

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
#if !ASP_NET_CORE
                httpContext.Request["key"].Returns(expectedResult);
#else
                httpContext.Request.HttpContext.Items.Returns(new Dictionary<object, object>() { { "key", expectedResult } });
#endif

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
#if !ASP_NET_CORE
                httpContext.Request.QueryString.Returns(new NameValueCollection { {"key", expectedResult} });
#else
                var queryCollection = Substitute.For<IQueryCollection>();
                queryCollection.Keys.Returns(x => new [] { "key" });
                queryCollection.Count.Returns(x => 1);
                queryCollection.TryGetValue("key", out Arg.Any<Microsoft.Extensions.Primitives.StringValues>()).Returns(x => {
                    x[1] = new Microsoft.Extensions.Primitives.StringValues(expectedResult);
                    return true;
                });
                queryCollection.ContainsKey("key").Returns(true);
                httpContext.Request.Query.Returns(queryCollection);
#endif

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
#if !ASP_NET_CORE
                httpContext.Request.Headers.Returns(new NameValueCollection { { "key", expectedResult } });
#else
                var headerDictionary = new HeaderDictionary(new Dictionary<string, StringValues>() { { "key", expectedResult } });
                httpContext.Request.Headers.Returns(headerDictionary);
#endif

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
#if !ASP_NET_CORE
                httpContext.Request.Form.Returns(new NameValueCollection { { "key", expectedResult } });
#else
                httpContext.Request.HasFormContentType.Returns(true);
                var formCollection = new FormCollection(new Dictionary<string, StringValues>{ { "key", expectedResult } });
                httpContext.Request.Form.Returns(formCollection);
#endif

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Form = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
        }

#if !ASP_NET_CORE
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
#endif

#if NETCOREAPP3_0_OR_GREATER
        public class ServerVariablesTests
        {
            [Fact]
            public void NullKeyRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var serverVariablesFeature = Substitute.For<IServerVariablesFeature>();
                var featureCollection = new FeatureCollection();
                featureCollection.Set<IServerVariablesFeature>(serverVariablesFeature);
                httpContext.Features.Returns(featureCollection);

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

                var serverVariablesFeature = Substitute.For<IServerVariablesFeature>();
                var featureCollection = new FeatureCollection();
                featureCollection.Set<IServerVariablesFeature>(serverVariablesFeature);
                httpContext.Features.Returns(featureCollection);

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

                var serverVariablesFeature = Substitute.For<IServerVariablesFeature>();
                serverVariablesFeature["key"].Returns(expectedResult);
                var featureCollection = new FeatureCollection();
                featureCollection.Set<IServerVariablesFeature>(serverVariablesFeature);
                httpContext.Request.HttpContext.Features.Returns(featureCollection);

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.ServerVariable = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
        }
#endif
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
#if !ASP_NET_CORE
                httpContext.Request.Cookies.Returns(new HttpCookieCollection {new HttpCookie("key", expectedResult) });
#else
                var cookieCollection = Substitute.For<IRequestCookieCollection>();
                cookieCollection.Keys.Returns(x => new [] { "key" });
                cookieCollection.Count.Returns(x => 1);
                cookieCollection.TryGetValue("key", out Arg.Any<string>()).Returns(x => {
                    x[1] = expectedResult;
                    return true;
                });
                cookieCollection.ContainsKey("key").Returns(true);
                httpContext.Request.Cookies.Returns(cookieCollection);
#endif

                var renderer = new AspNetRequestValueLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Cookie = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
        }
    }
}