
#if !NETSTANDARD_1plus
//TODO test .NET Core
using System;
using System.Collections.Generic;
using System.Globalization;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NLog.Web.Enums;
using Xunit;

using System.Reflection;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

#if !NETSTANDARD_1plus
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif



namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetCookieLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        public AspNetCookieLayoutRendererTests() : base()
        {
            
        }

        [Fact]
        public void NullKeyRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetRequestCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookieNames = null;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Flat_Formatting()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Cookies.Returns(new HttpCookieCollection { new HttpCookie("key1", "TEST") });
            var renderer = new AspNetRequestCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookieNames = new List<string> { "key" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Cookies.Returns(new HttpCookieCollection { new HttpCookie("key1", "TEST") });
            var renderer = new AspNetRequestCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookieNames = new List<string> { "key" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

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

            var renderer = new AspNetRequestCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookieNames = new List<string> { "key" };

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

            var renderer = new AspNetRequestCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookieNames = new List<string> { "key" };

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

            var renderer = new AspNetRequestCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookieNames = new List<string> { "key" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

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

            var renderer = new AspNetRequestCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookieNames = new List<string> { "key", "key2" };

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

            var renderer = new AspNetRequestCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookieNames = new List<string> { "key", "key2" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void CommaSeperatedCookieNamesTest_Mulitple_FLAT_Formatting()
        {
            var expectedResult = "key=TEST&Key1=TEST1";

            string config = @"<nlog>
    <extensions>
        <add assembly='NLog.Web' />
    </extensions>
<targets><target name='debug' type='Debug' layout='${aspnet-request-cookie:CookieNames=key,key1}' /></targets>
    <rules>
        <logger name='*' minlevel='Debug' writeTo='debug' />
    </rules>
</nlog>";
            LogManager.Configuration = CreateConfigurationFromString(config);

            var cookie = new HttpCookie("key", "TEST");
            cookie["Key1"] = "TEST1";

            this.HttpContext.Request.Cookies.Add(cookie);
            var t = (DebugTarget)LogManager.Configuration.AllTargets[0];
            var renderer = ((SimpleLayout)t.Layout).Renderers[0] as AspNetRequestCookieLayoutRenderer;

            var result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void CommaSeperatedCookieNamesTest_Mulitple_Json_Formatting()
        {
            var expectedResult = "{\"key=TEST&Key1=TEST1\"}";

            string config = @"<nlog>
    <extensions>
        <add assembly='NLog.Web' />
    </extensions>
<targets><target name='debug' type='Debug' layout='${aspnet-request-cookie:CookieNames=key,key1:OutputFormat=Json}' /></targets>
</nlog>";
            LogManager.Configuration = CreateConfigurationFromString(config);

            var cookie = new HttpCookie("key", "TEST");
            cookie["Key1"] = "TEST1";

            this.HttpContext.Request.Cookies.Add(cookie);
            var t = (DebugTarget)LogManager.Configuration.AllTargets[0];
            var renderer = ((SimpleLayout)t.Layout).Renderers[0] as AspNetRequestCookieLayoutRenderer;

            var result = renderer.Render(LogEventInfo.CreateNullEvent());           

            Assert.Equal(expectedResult, result);
        }
    }
}
#endif