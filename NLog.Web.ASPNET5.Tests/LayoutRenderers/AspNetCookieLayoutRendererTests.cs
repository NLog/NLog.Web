using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NLog.Web.Enums;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetCookieLayoutRendererTests
    {
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
            renderer.CookiesNames = "key";
            renderer.CookiesNames = "FLAT";

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
            renderer.CookiesNames = "key";
            renderer.OutputFormat = AspNetCookieLayoutOutputFormat.Json;

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
            renderer.CookiesNames = "key";

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
            renderer.CookiesNames = "key";

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
            renderer.CookiesNames = "key";
            renderer.OutputFormat = AspNetCookieLayoutOutputFormat.Json;

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
            renderer.CookiesNames = "key, key2";

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
            renderer.CookiesNames = "key, key2";
            renderer.OutputFormat = AspNetCookieLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
    }
}
