using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NLog.Web.LayoutRenderers;
using NLog.Web.Enums;
using Xunit;

using System.Reflection;

using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

#if !ASP_NET_CORE
using NSubstitute;
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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
#if ASP_NET_CORE
            var httpContext = this.HttpContext;
#else
            var httpContext = Substitute.For<HttpContextBase>();
#endif

            /*
            var renderer = new AspNetRequestCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            */
            var renderer = CreateRenderer();
            renderer.CookieNames = null;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Flat_Formatting()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.CookieNames = new List<string> { "notfound" };
            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }



        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.CookieNames = new List<string> { "notfound" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyFoundRendersValue_Cookie_Mulitple_Items_Flat_Formatting()
        {
#if ASP_NET_CORE
            //no multivalue keys in ASP.NET core
            var expectedResult = "key=TEST,Key1=TEST1";
#else
            var expectedResult = "key=TEST&Key1=TEST1";
#endif

            var renderer = CreateRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }


        [Fact]
        public void KeyFoundRendersValue_Cookie_Multiple_Items_Flat_Formatting_separators()
        {
#if ASP_NET_CORE
            //no multivalue keys in ASP.NET core
            var expectedResult = "key:TEST|Key1:TEST1";
#else
            var expectedResult = "key:TEST&Key1=TEST1"; 
#endif

            var renderer = CreateRenderer();
            renderer.ValueSeparator = ":";
            renderer.ItemSeparator = "|";

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Item_Flat_Formatting()
        {
            var expectedResult = "key=TEST";

            var renderer = CreateRenderer(addKey: false);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Item_Json_Formatting()
        {
            var expectedResult = "[{\"key\":\"TEST\"}]";

            var renderer = CreateRenderer(addKey: false);

            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Item_Json_Formatting_no_array()
        {
            var expectedResult = "{\"key\":\"TEST\"}";

            var renderer = CreateRenderer(addKey: false);

            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.SingleAsArray = false;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void KeyFoundRendersValue_Cookie_Mulitple_Items_Json_Formatting(bool singleAsArray)
        {
            var expectedResult = "[{\"key\":\"TEST\"},{\"Key1\":\"TEST1\"}]";

            var renderer = CreateRenderer();

            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.SingleAsArray = singleAsArray;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

//no multivalue keys in ASP.NET core
#if !ASP_NET_CORE

        [Fact]
        public void KeyFoundRendersVakue_Cookie_Mulitple_Cookies_Cookie_Items_Flat_Formatting()
        {
            var expectedResult = "key=TEST&Key1=TEST1,key2=Test&key3=Test456";

            var renderer = CreateRenderer(addCookie2: true);

            renderer.CookieNames = new List<string> { "key", "key2" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersVakue_Cookie_Mulitple_Cookies_Cookie_Items_Json_Formatting()
        {
            var expectedResult = "[{\"key\":\"TEST\"},{\"Key1\":\"TEST1\"},{\"key2\":\"Test\"},{\"key3\":\"Test456\"}]";
            var renderer = CreateRenderer(addCookie2: true);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
#endif

#if !ASP_NET_CORE //todo

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
            var expectedResult = "[{\"key\":\"TEST\"},{\"Key1\":\"TEST1\"}]";

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

#endif

        /// <summary>
        /// Create cookie renderer with mockup http context
        /// </summary>
        /// <param name="addKey">add key1 to cookie 1</param>
        /// <param name="addCookie2">add 2nd cookie</param>
        /// <returns></returns>
        private AspNetRequestCookieLayoutRenderer CreateRenderer(bool addKey = true, bool addCookie2 = false)
        {
            var cookieNames = new List<string>();
#if ASP_NET_CORE
            var httpContext = this.HttpContext;
#else
            var httpContext = Substitute.For<HttpContextBase>();
#endif

#if ASP_NET_CORE
            void AddCookie(string key, string result)
            {
                cookieNames.Add(key);

                var newCookieValues = new [] { $"{key}={result}" };
                if (!httpContext.Request.Headers.TryGetValue("Cookie", out var cookieHeaderValues))
                {
                    cookieHeaderValues = new StringValues(newCookieValues);
                }
                else
                {
                    cookieHeaderValues = new StringValues(cookieHeaderValues.ToArray().Union(newCookieValues).ToArray());
                }
                httpContext.Request.Headers["Cookie"] = cookieHeaderValues;
            }

            AddCookie("key", "TEST");

            if (addKey)
            {
                AddCookie("Key1", "TEST1");
            }

            if (addCookie2)
            {
                AddCookie("key2", "Test");
                AddCookie("key3", "Test456");
            }

#else

            var cookie1 = new HttpCookie("key", "TEST");
            cookieNames.Add("key");
            if (addKey)
            {
                cookie1["Key1"] = "TEST1";
            }
            var cookies = new HttpCookieCollection { cookie1 };
           
            if (addCookie2)
            {
                var cookie2 = new HttpCookie("key2", "Test");
                cookie2["key3"] = "Test456";
                cookies.Add(cookie2);
                cookieNames.Add("key2");
            }

            httpContext.Request.Cookies.Returns(cookies);
#endif

            var renderer = new AspNetRequestCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookieNames = cookieNames;
            return renderer;
        }
    }
}
