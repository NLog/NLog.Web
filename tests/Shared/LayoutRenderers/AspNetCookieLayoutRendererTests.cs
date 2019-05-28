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
            var httpContext = HttpContext;
#else
            var httpContext = Substitute.For<HttpContextBase>();
#endif

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
        public void KeyFoundRendersValue_Multiple_Cookies_Flat_Formatting()
        {
            var expectedResult = "key=TEST,Key1=TEST1";

            var renderer = CreateRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Cookies_Flat_Formatting_separators()
        {
            var expectedResult = "key:TEST|Key1:TEST1";

            var renderer = CreateRenderer();
            renderer.ValueSeparator = ":";
            renderer.ItemSeparator = "|";

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Cookies_Flat_Formatting_separators_layouts()
        {
            try
            {
                var expectedResult = "key>TEST" + Environment.NewLine + "Key1>TEST1";
                GlobalDiagnosticsContext.Set("valueSeparator1", ">");

                var renderer = CreateRenderer();
                renderer.ValueSeparator = "${gdc:valueSeparator1}";
                renderer.ItemSeparator = "${newline}";

                string result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
            finally
            {
                //clean up
                GlobalDiagnosticsContext.Clear();
            }
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Cookie_Flat_Formatting()
        {
            var expectedResult = "key=TEST";

            var renderer = CreateRenderer(addSecondCookie: false);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Cookie_Json_Formatting()
        {
            var expectedResult = "[{\"key\":\"TEST\"}]";

            var renderer = CreateRenderer(addSecondCookie: false);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Cookie_Json_Formatting_no_array()
        {
            var expectedResult = "{\"key\":\"TEST\"}";

            var renderer = CreateRenderer(addSecondCookie: false);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.SingleAsArray = false;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void KeyFoundRendersValue_Multiple_Cookies_Json_Formatting(bool singleAsArray)
        {
            var expectedResult = "[{\"key\":\"TEST\"},{\"Key1\":\"TEST1\"}]";

            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.SingleAsArray = singleAsArray;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Flat_Formatting_ValuesOnly()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.CookieNames = new List<string> { "notfound" };
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting_ValuesOnly()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.CookieNames = new List<string> { "notfound" };
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyFoundRendersValue_Cookie_Multiple_Items_Flat_Formatting_ValuesOnly()
        {
            var expectedResult = "TEST,TEST1";

            var renderer = CreateRenderer();
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Cookie_Multiple_Items_Flat_Formatting_separators_ValuesOnly()
        {
            var expectedResult = "TEST|TEST1";

            var renderer = CreateRenderer();
            renderer.ValueSeparator = ":";
            renderer.ItemSeparator = "|";
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Item_Flat_Formatting_ValuesOnly()
        {
            var expectedResult = "TEST";

            var renderer = CreateRenderer(addSecondCookie: false);
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Item_Json_Formatting_ValuesOnly()
        {
            var expectedResult = "[\"TEST\"]";

            var renderer = CreateRenderer(addSecondCookie: false);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Item_Json_Formatting_no_array_ValuesOnly()
        {
            // With ValuesOnly enabled, only arrays are valid
            var expectedResult = "[\"TEST\"]";

            var renderer = CreateRenderer(addSecondCookie: false);

            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.SingleAsArray = false;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void KeyFoundRendersValue_Cookie_Multiple_Items_Json_Formatting_ValuesOnly(bool singleAsArray)
        {
            var expectedResult = "[\"TEST\",\"TEST1\"]";

            var renderer = CreateRenderer();

            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.SingleAsArray = singleAsArray;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        //no multivalue cookie keys in ASP.NET core
#if !ASP_NET_CORE

        [Fact]
        public void KeyFoundRendersValue_Multiple_Cookies_And_Cookie_Values_Flat_Formatting()
        {
            var expectedResult = "key=TEST,key2=Test&key3=Test456";

            var renderer = CreateRenderer(addSecondCookie: true, addMultiValueCookieKey: true);
            renderer.CookieNames = new List<string> { "key", "key2" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Cookies_And_Cookie_Values_Json_Formatting()
        {
            var expectedResult = "[{\"key\":\"TEST\"},{\"Key1\":\"TEST1\"},{\"key2\":\"Test\"},{\"key3\":\"Test456\"}]";
            var renderer = CreateRenderer(addSecondCookie: true, addMultiValueCookieKey: true);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
#endif

#if !ASP_NET_CORE //todo

        [Fact]
        public void CommaSeperatedCookieNamesTest_Multiple_Cookie_Values_Flat_Formatting()
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

            HttpContext.Request.Cookies.Add(cookie);
            var t = (DebugTarget)LogManager.Configuration.AllTargets[0];
            var renderer = ((SimpleLayout)t.Layout).Renderers[0] as AspNetRequestCookieLayoutRenderer;

            var result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void CommaSeperatedCookieNamesTest_Multiple_Cookie_Values_Json_Formatting()
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

            HttpContext.Request.Cookies.Add(cookie);
            var t = (DebugTarget)LogManager.Configuration.AllTargets[0];
            var renderer = ((SimpleLayout)t.Layout).Renderers[0] as AspNetRequestCookieLayoutRenderer;

            var result = renderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal(expectedResult, result);
        }

#endif

        /// <summary>
        /// Create cookie renderer with mocked HTTP context
        /// </summary>
        /// <param name="addSecondCookie">Add second cookie</param>
        /// <param name="addMultiValueCookieKey">Make cookie multi-value by adding a second value to it</param>
        /// <returns>Created cookie layout renderer</returns>
        /// <remarks>
        /// The parameter <paramref name="addMultiValueCookieKey"/> allows creation of multi-valued cookies with the same key,
        /// as provided in the HttpCookie API for backwards compatibility with classic ASP.
        /// This is not supported in ASP.NET Core. For further details, see:
        /// https://docs.microsoft.com/en-us/dotnet/api/system.web.httpcookie.item?view=netframework-4.7.1#System_Web_HttpCookie_Item_System_String_
        /// https://stackoverflow.com/a/43831482/6651
        /// https://github.com/aspnet/HttpAbstractions/issues/831
        /// </remarks>
        private AspNetRequestCookieLayoutRenderer CreateRenderer(bool addSecondCookie = true, bool addMultiValueCookieKey = false)
        {
            var cookieNames = new List<string>();
#if ASP_NET_CORE
            var httpContext = HttpContext;
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

            if (addSecondCookie)
            {
                AddCookie("Key1", "TEST1");
            }

            if (addMultiValueCookieKey)
            {
                throw new NotSupportedException("Multi-valued cookie keys are not supported in ASP.NET Core");
            }

#else

            var cookie1 = new HttpCookie("key", "TEST");
            var cookies = new HttpCookieCollection { cookie1 };
            cookieNames.Add("key");

            if (addSecondCookie)
            {
                var cookie2 = new HttpCookie("Key1", "TEST1");
                cookies.Add(cookie2);
                cookieNames.Add("Key1");
            }

            if (addMultiValueCookieKey)
            {
                var multiValueCookie = new HttpCookie("key2", "Test");
                multiValueCookie["key3"] = "Test456";
                cookies.Add(multiValueCookie);
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
