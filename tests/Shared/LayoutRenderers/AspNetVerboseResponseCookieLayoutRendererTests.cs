using System;
using System.Collections.Generic;
using System.Linq;
using NLog.Web.LayoutRenderers;
using NLog.Web.Enums;
using Xunit;
#if !ASP_NET_CORE
using NSubstitute;
using System.Web;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetVerboseResponseCookieLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void NullKeyRendersAllCookies()
        {
#if ASP_NET_CORE
            var expectedResult = "Name=key,Value=TEST,Domain=www.nlog.com,Path=/nlog.web,Expires=2022-02-04 13:14:15Z,Secure=False,HttpOnly=True,SameSite=Strict;Name=Key1,Value=TEST1,Domain=www.nlog.com,Path=/nlog.web2,Expires=2022-02-04 16:17:18Z,Secure=True,HttpOnly=False,SameSite=Lax";
#else
            var expectedResult = "Name=key,Value=TEST,Domain=www.nlog.com,Path=/nlog.web,Expires=2022-02-04 13:14:15Z,Secure=False,HttpOnly=True;Name=Key1,Value=TEST1,Domain=www.nlog.com,Path=/nlog.web2,Expires=2022-02-04 16:17:18Z,Secure=True,HttpOnly=False";
#endif
            var renderer = CreateRenderer();
            renderer.CookieNames = null;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void NullKeyRendersAllCookiesExceptExcluded()
        {
#if ASP_NET_CORE
            var expectedResult = "Name=Key1,Value=TEST1,Domain=www.nlog.com,Path=/nlog.web2,Expires=2022-02-04 16:17:18Z,Secure=True,HttpOnly=False,SameSite=Lax";
#else
            var expectedResult = "Name=Key1,Value=TEST1,Domain=www.nlog.com,Path=/nlog.web2,Expires=2022-02-04 16:17:18Z,Secure=True,HttpOnly=False";
#endif
            var renderer = CreateRenderer();
            renderer.CookieNames = null;
            renderer.Exclude.Add("key");

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
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
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;
            renderer.CookieNames = new List<string> { "notfound" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_JsonDictionary_Formatting()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonDictionary;
            renderer.CookieNames = new List<string> { "notfound" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Cookies_Flat_Formatting()
        {
#if ASP_NET_CORE
            var expectedResult = "Name=key,Value=TEST,Domain=www.nlog.com,Path=/nlog.web,Expires=2022-02-04 13:14:15Z,Secure=False,HttpOnly=True,SameSite=Strict;Name=Key1,Value=TEST1,Domain=www.nlog.com,Path=/nlog.web2,Expires=2022-02-04 16:17:18Z,Secure=True,HttpOnly=False,SameSite=Lax";
#else
            var expectedResult = "Name=key,Value=TEST,Domain=www.nlog.com,Path=/nlog.web,Expires=2022-02-04 13:14:15Z,Secure=False,HttpOnly=True;Name=Key1,Value=TEST1,Domain=www.nlog.com,Path=/nlog.web2,Expires=2022-02-04 16:17:18Z,Secure=True,HttpOnly=False";
#endif
            var renderer = CreateRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Cookies_Flat_Formatting_separators()
        {
#if ASP_NET_CORE
            var expectedResult = "Name:key|Value:TEST|Domain:www.nlog.com|Path:/nlog.web|Expires:2022-02-04 13:14:15Z|Secure:False|HttpOnly:True|SameSite:Strict;Name:Key1|Value:TEST1|Domain:www.nlog.com|Path:/nlog.web2|Expires:2022-02-04 16:17:18Z|Secure:True|HttpOnly:False|SameSite:Lax";
#else
            var expectedResult = "Name:key|Value:TEST|Domain:www.nlog.com|Path:/nlog.web|Expires:2022-02-04 13:14:15Z|Secure:False|HttpOnly:True;Name:Key1|Value:TEST1|Domain:www.nlog.com|Path:/nlog.web2|Expires:2022-02-04 16:17:18Z|Secure:True|HttpOnly:False";
#endif
            var renderer = CreateRenderer();
            renderer.ValueSeparator = ":";
            renderer.ItemSeparator = "|";

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Cookies_Flat_Formatting_separators_layouts()
        {
#if ASP_NET_CORE
            var expectedResult =$"Name>key{Environment.NewLine}Value>TEST{Environment.NewLine}Domain>www.nlog.com{Environment.NewLine}Path>/nlog.web{Environment.NewLine}Expires>2022-02-04 13:14:15Z{Environment.NewLine}Secure>False{Environment.NewLine}HttpOnly>True{Environment.NewLine}SameSite>Strict;Name>Key1{Environment.NewLine}Value>TEST1{Environment.NewLine}Domain>www.nlog.com{Environment.NewLine}Path>/nlog.web2{Environment.NewLine}Expires>2022-02-04 16:17:18Z{Environment.NewLine}Secure>True{Environment.NewLine}HttpOnly>False{Environment.NewLine}SameSite>Lax";

#else
            var expectedResult =$"Name>key{Environment.NewLine}Value>TEST{Environment.NewLine}Domain>www.nlog.com{Environment.NewLine}Path>/nlog.web{Environment.NewLine}Expires>2022-02-04 13:14:15Z{Environment.NewLine}Secure>False{Environment.NewLine}HttpOnly>True;Name>Key1{Environment.NewLine}Value>TEST1{Environment.NewLine}Domain>www.nlog.com{Environment.NewLine}Path>/nlog.web2{Environment.NewLine}Expires>2022-02-04 16:17:18Z{Environment.NewLine}Secure>True{Environment.NewLine}HttpOnly>False";
#endif
            var renderer = CreateRenderer();
            renderer.ValueSeparator = "${event-properties:valueSeparator1}";
            renderer.ItemSeparator = "${newline}";

            var logEventInfo = new LogEventInfo();
            logEventInfo.Properties["valueSeparator1"] = ">";

            // Act
            string result = renderer.Render(logEventInfo);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Cookie_Flat_Formatting()
        {
#if ASP_NET_CORE
            var expectedResult = "Name=key,Value=TEST,Domain=www.nlog.com,Path=/nlog.web,Expires=2022-02-04 13:14:15Z,Secure=False,HttpOnly=True,SameSite=Strict";
#else
            var expectedResult = "Name=key,Value=TEST,Domain=www.nlog.com,Path=/nlog.web,Expires=2022-02-04 13:14:15Z,Secure=False,HttpOnly=True";
#endif
            var renderer = CreateRenderer(addSecondCookie: false);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Cookie_Json_Formatting()
        {
#if ASP_NET_CORE
            var expectedResult = "[{\"Name\":\"key\",\"Value\":\"TEST\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web\",\"Expires\":\"2022-02-04 13:14:15Z\",\"Secure\":\"False\",\"HttpOnly\":\"True\",\"SameSite\":\"Strict\"}]";
#else
            var expectedResult = "[{\"Name\":\"key\",\"Value\":\"TEST\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web\",\"Expires\":\"2022-02-04 13:14:15Z\",\"Secure\":\"False\",\"HttpOnly\":\"True\"}]";
#endif
            var renderer = CreateRenderer(addSecondCookie: false);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Cookie_JsonDictionary_Formatting()
        {
#if ASP_NET_CORE
            var expectedResult = "{{\"Name\":\"key\",\"Value\":\"TEST\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web\",\"Expires\":\"2022-02-04 13:14:15Z\",\"Secure\":\"False\",\"HttpOnly\":\"True\",\"SameSite\":\"Strict\"}}";
#else
            var expectedResult = "{{\"Name\":\"key\",\"Value\":\"TEST\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web\",\"Expires\":\"2022-02-04 13:14:15Z\",\"Secure\":\"False\",\"HttpOnly\":\"True\"}}";
#endif
            var renderer = CreateRenderer(addSecondCookie: false);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonDictionary;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Cookies_Json_Formatting()
        {
#if ASP_NET_CORE
            var expectedResult =
                "[{\"Name\":\"key\",\"Value\":\"TEST\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web\",\"Expires\":\"2022-02-04 13:14:15Z\",\"Secure\":\"False\",\"HttpOnly\":\"True\",\"SameSite\":\"Strict\"},{\"Name\":\"Key1\",\"Value\":\"TEST1\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web2\",\"Expires\":\"2022-02-04 16:17:18Z\",\"Secure\":\"True\",\"HttpOnly\":\"False\",\"SameSite\":\"Lax\"}]";
#else
            var expectedResult =
                "[{\"Name\":\"key\",\"Value\":\"TEST\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web\",\"Expires\":\"2022-02-04 13:14:15Z\",\"Secure\":\"False\",\"HttpOnly\":\"True\"},{\"Name\":\"Key1\",\"Value\":\"TEST1\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web2\",\"Expires\":\"2022-02-04 16:17:18Z\",\"Secure\":\"True\",\"HttpOnly\":\"False\"}]";
#endif
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        //no multivalue cookie keys in ASP.NET core
#if !ASP_NET_CORE

        [Fact]
        public void KeyFoundRendersValue_Multiple_Cookies_And_Cookie_Values_Flat_Formatting()
        {
            var expectedResult = "Name=key,Value=TEST,Domain=www.nlog.com,Path=/nlog.web,Expires=2022-02-04 13:14:15Z,Secure=False,HttpOnly=True;Name=key2,Value=Test&key3=Test456,Domain=www.nlog.com,Path=/nlog.web3,Expires=2022-02-04 19:20:21Z,Secure=True,HttpOnly=True";

            var renderer = CreateRenderer(addSecondCookie: true, addMultiValueCookieKey: true);
            renderer.CookieNames = new List<string> { "key", "key2" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Cookies_And_Cookie_Values_Json_Formatting()
        {
            var expectedResult = "[{\"Name\":\"key\",\"Value\":\"TEST\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web\",\"Expires\":\"2022-02-04 13:14:15Z\",\"Secure\":\"False\",\"HttpOnly\":\"True\"},{\"Name\":\"Key1\",\"Value\":\"TEST1\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web2\",\"Expires\":\"2022-02-04 16:17:18Z\",\"Secure\":\"True\",\"HttpOnly\":\"False\"},{\"Name\":\"key2\",\"Value\":\"Test\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web3\",\"Expires\":\"2022-02-04 19:20:21Z\",\"Secure\":\"True\",\"HttpOnly\":\"True\"},{\"Name\":\"key3\",\"Value\":\"Test456\",\"Domain\":\"www.nlog.com\",\"Path\":\"/nlog.web3\",\"Expires\":\"2022-02-04 19:20:21Z\",\"Secure\":\"True\",\"HttpOnly\":\"True\"}]";
            var renderer = CreateRenderer(addSecondCookie: true, addMultiValueCookieKey: true);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
#endif

#if !ASP_NET_CORE //todo

        [Fact]
        public void CommaSeperatedCookieNamesTest_Multiple_Cookie_Values_Flat_Formatting()
        {
            // Arrange
            var expectedResult = "key=TEST&Key1=TEST1";

            var cookie = new HttpCookie("key", "TEST") { ["Key1"] = "TEST1" };

            var layoutRender = new AspNetRequestCookieLayoutRenderer()
            {
                CookieNames = new List<string> { "key", "key1" }
            };

            var httpContextAccessorMock = CreateHttpContextAccessorMockWithCookie(cookie);
            layoutRender.HttpContextAccessor = httpContextAccessorMock;

            // Act
            var result = layoutRender.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void CommaSeperatedCookieNamesTest_Multiple_Cookie_Values_Json_Formatting()
        {
            var expectedResult = "[{\"key\":\"TEST\"},{\"Key1\":\"TEST1\"}]";

            var cookie = new HttpCookie("key", "TEST") { ["Key1"] = "TEST1" };

            var layoutRender = new AspNetRequestCookieLayoutRenderer()
            {
                CookieNames = new List<string> { "key", "key1" },
                OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray
            };

            var httpContextAccessorMock = CreateHttpContextAccessorMockWithCookie(cookie);
            layoutRender.HttpContextAccessor = httpContextAccessorMock;

            var result = layoutRender.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal(expectedResult, result);
        }

        private static IHttpContextAccessor CreateHttpContextAccessorMockWithCookie(HttpCookie cookie)
        {
            var httpCookieCollection = new HttpCookieCollection { cookie };
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            httpContextAccessorMock.HttpContext.Request.Cookies.Returns(httpCookieCollection);
            return httpContextAccessorMock;
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
        private AspNetResponseCookieLayoutRenderer CreateRenderer(bool addSecondCookie = true, bool addMultiValueCookieKey = false)
        {
            var cookieNames = new List<string>();
#if ASP_NET_CORE
            var httpContext = HttpContext;
#else
            var httpContext = Substitute.For<HttpContextBase>();
#endif

#if ASP_NET_CORE
            void AddCookie(string key, string value, CookieOptions options)
            {
                cookieNames.Add(key);

                httpContext.Response.Cookies.Append(key,value,options);
            }

            AddCookie("key", "TEST", new CookieOptions
            {
                Domain = "www.nlog.com",
                Path = "/nlog.web",
                Secure = false,
                HttpOnly = true,
                Expires = new DateTime(2022, 2, 4, 13, 14, 15, DateTimeKind.Utc),
                SameSite = SameSiteMode.Strict
            });

            if (addSecondCookie)
            {
                AddCookie("Key1", "TEST1", new CookieOptions
                {
                    Domain = "www.nlog.com",
                    Path = "/nlog.web2",
                    Secure = true,
                    HttpOnly = false,
                    Expires = new DateTime(2022, 2, 4, 16, 17, 18, DateTimeKind.Utc),
                    SameSite = SameSiteMode.Lax
                });
            }

            if (addMultiValueCookieKey)
            {
                throw new NotSupportedException("Multi-valued cookie keys are not supported in ASP.NET Core");
            }

#else

            var cookie1 = new HttpCookie("key", "TEST")
            {
                Domain = "www.nlog.com",
                Path = "/nlog.web",
                Secure = false,
                HttpOnly = true,
                Expires = new DateTime(2022,2,4,13,14,15,DateTimeKind.Utc)
            };
            var cookies = new HttpCookieCollection { cookie1 };
            cookieNames.Add("key");

            if (addSecondCookie)
            {
                var cookie2 = new HttpCookie("Key1", "TEST1")
                {
                    Domain = "www.nlog.com",
                    Path = "/nlog.web2",
                    Secure = true,
                    HttpOnly = false,
                    Expires = new DateTime(2022, 2, 4, 16, 17, 18, DateTimeKind.Utc)
                };

                cookies.Add(cookie2);
                cookieNames.Add("Key1");
            }

            if (addMultiValueCookieKey)
            {
                var multiValueCookie = new HttpCookie("key2", "Test")
                {
                    Domain = "www.nlog.com",
                    Path = "/nlog.web3",
                    Secure = true,
                    HttpOnly = true,
                    Expires = new DateTime(2022, 2, 4, 19, 20, 21, DateTimeKind.Utc)
                };
                multiValueCookie["key3"] = "Test456";
                cookies.Add(multiValueCookie);
                cookieNames.Add("key2");
            }

            httpContext.Response.Cookies.Returns(cookies);
#endif

            var renderer = new AspNetResponseCookieLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.CookieNames = cookieNames;
            renderer.Verbose = true;
            return renderer;
        }
    }
}
