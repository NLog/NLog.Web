using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NLog.Web.Enums;
using Xunit;
using System.Web.SessionState;
using System.Reflection;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetCookieLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        public AspNetCookieLayoutRendererTests() : base()
        {
            this.SetUp();
        }

        public void SetUp()
        {
            //auto load won't work yet (in DNX), so use <extensions>
            LogManager.Configuration = CreateConfigurationFromString(@"
<nlog throwExceptions='true'>
    <extensions>
        <add assembly='NLog.Web' />
    </extensions>
</nlog>");
            SetupFakeSession();
        }

        protected override void CleanUp()
        {
            Session.Clear();
        }

        private HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        public void SetupFakeSession()
        {
            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            HttpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });
        }

        [Fact]
        public void NullKeyRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookiesNames = null;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Flat_Formatting()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Cookies.Returns(new HttpCookieCollection { new HttpCookie("key1", "TEST") });
            var renderer = new AspNetCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookiesNames = new List<string> { "key" };
            renderer.OutputFormat = AspNetLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Cookies.Returns(new HttpCookieCollection { new HttpCookie("key1", "TEST") });
            var renderer = new AspNetCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookiesNames = new List<string> { "key" };
            renderer.OutputFormat = AspNetLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyFoundRendersValue_Cookie_Mulitple_Items_Flat_Formatting()
        {
            var expectedResult = "key=TEST&Key1=TEST1";
            var httpContext = Substitute.For<HttpContextBase>();
            var cookie = new HttpCookie("key", "TEST");
            cookie["Key1"] = "TEST1";
            var cookies = new HttpCookieCollection();
            cookies.Add(cookie);
            httpContext.Request.Cookies.Returns(cookies);

            var renderer = new AspNetCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookiesNames = new List<string> { "key" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Item_Flat_Formatting()
        {
            var expectedResult = "key=TEST";
            var httpContext = Substitute.For<HttpContextBase>();
            var cookie = new HttpCookie("key", "TEST");
            var cookies = new HttpCookieCollection();
            cookies.Add(cookie);
            httpContext.Request.Cookies.Returns(cookies);

            var renderer = new AspNetCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookiesNames = new List<string> { "key" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Cookie_Mulitple_Items_Json_Formatting()
        {
            var expectedResult = "{\"key=TEST&Key1=TEST1\"}";
            var httpContext = Substitute.For<HttpContextBase>();
            var cookie = new HttpCookie("key", "TEST");
            cookie["Key1"] = "TEST1";
            var cookies = new HttpCookieCollection();
            cookies.Add(cookie);
            httpContext.Request.Cookies.Returns(cookies);

            var renderer = new AspNetCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookiesNames = new List<string> { "key" };
            renderer.OutputFormat = AspNetLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersVakue_Cookie_Mulitple_Cookies_Cookie_Items_Flat_Formatting()
        {
            var expectedResult = "key=TEST&Key1=TEST1,key2=Test&key3=Test456";
            var httpContext = Substitute.For<HttpContextBase>();
            var cookie = new HttpCookie("key", "TEST");
            cookie["Key1"] = "TEST1";
            var cookies = new HttpCookieCollection();
            cookies.Add(cookie);

            cookie = new HttpCookie("key2", "Test");
            cookie["key3"] = "Test456";
            cookies.Add(cookie);

            httpContext.Request.Cookies.Returns(cookies);

            var renderer = new AspNetCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookiesNames = new List<string> { "key", "key2" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersVakue_Cookie_Mulitple_Cookies_Cookie_Items_Json_Formatting()
        {
            var expectedResult = "{\"key=TEST&Key1=TEST1\"},{\"key2=Test&key3=Test456\"}";
            var httpContext = Substitute.For<HttpContextBase>();
            var cookie = new HttpCookie("key", "TEST");
            cookie["Key1"] = "TEST1";
            var cookies = new HttpCookieCollection();
            cookies.Add(cookie);

            cookie = new HttpCookie("key2", "Test");
            cookie["key3"] = "Test456";
            cookies.Add(cookie);

            httpContext.Request.Cookies.Returns(cookies);

            var renderer = new AspNetCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookiesNames = new List<string> { "key", "key2" };
            renderer.OutputFormat = AspNetLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void CommaSeperatedCookieNamesTest()
        {
            LogManager.Configuration = CreateConfigurationFromString(@"
<nlog throwExceptions='true'>
    <extensions>
        <add assembly='NLog.Web' />
    </extensions>
<targets><target name='debug' type='Debug' layout='${aspnet-request-cookie:CookiesNames=test1}' /></targets>
    <rules>
        <logger name='*' minlevel='Debug' writeTo='debug' />
    </rules>
</nlog>");

            var cookie = new HttpCookie("key", "TEST");
            cookie["Key1"] = "TEST1";

            this.HttpContext.Request.Cookies.Add(cookie);
            var t = (DebugTarget)LogManager.Configuration.AllTargets[0];
            var renderer = ((SimpleLayout)t.Layout).Renderers[0] as AspNetCookieLayoutRenderer;

            var result = renderer.Render(LogEventInfo.CreateNullEvent());

        }
    }
}
