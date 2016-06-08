#if !NETSTANDARD_1plus
//todo nsubstitute

using System;
using System.Collections.Generic;
using System.Globalization;
#if !NETSTANDARD_1plus
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
using HttpSessionState = Microsoft.AspNetCore.Http.ISession;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NLog.Web.Enums;
using Xunit;
using System.Collections.Specialized;
using System.Reflection;
using NLog.Targets;
using NLog.Layouts;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetQueryStringLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        public AspNetQueryStringLayoutRendererTests() : base()
        {
            this.SetUp();
        }

        public void SetUp()
        {
            //auto load won't work yet (in DNX), so use <extensions>            
            SetupFakeSession();
        }

        protected override void CleanUp()
        {
            Session.Clear();
        }

        private HttpSessionState Session
        {
            get
            {
#if NETSTANDARD_1plus
                return HttpContext.Session;
#else
                return HttpContext.Current.Session;
#endif
            }
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

            var renderer = new AspNetQueryStringLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.QueryStringKeys = null;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Flat_Formatting()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var namedClollection = new NameValueCollection();
            namedClollection.Add("Id", "1");
            httpContext.Request.QueryString.Returns(namedClollection);

            var renderer = new AspNetQueryStringLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.QueryStringKeys = new List<string> { "key" };
            renderer.OutputFormat = AspNetLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var namedClollection = new NameValueCollection();
            namedClollection.Add("Id", "1");
            httpContext.Request.QueryString.Returns(namedClollection);

            var renderer = new AspNetQueryStringLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.QueryStringKeys = new List<string> { "key" };
            renderer.OutputFormat = AspNetLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Single_Item_Flat_Formatting()
        {
            var expectedResult = "Id:1";
            var httpContext = Substitute.For<HttpContextBase>();
            var namedClollection = new NameValueCollection();
            namedClollection.Add("Id", "1");
            httpContext.Request.QueryString.Returns(namedClollection);

            var renderer = new AspNetQueryStringLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.QueryStringKeys = new List<string> { "Id" };
            renderer.OutputFormat = AspNetLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Single_Item_Json_Formatting()
        {
            var expectedResult = "[{\"Id\":\"1\"}]";
            var httpContext = Substitute.For<HttpContextBase>();
            var namedClollection = new NameValueCollection();
            namedClollection.Add("Id", "1");
            httpContext.Request.QueryString.Returns(namedClollection);

            var renderer = new AspNetQueryStringLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.QueryStringKeys = new List<string> { "Id" };
            renderer.OutputFormat = AspNetLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Multiple_Item_Flat_Formatting()
        {
            var expectedResult = "Id:1," + Environment.NewLine + "Id2:2";
            var httpContext = Substitute.For<HttpContextBase>();
            var namedClollection = new NameValueCollection();
            namedClollection.Add("Id", "1");
            namedClollection.Add("Id2", "2");
            httpContext.Request.QueryString.Returns(namedClollection);

            var renderer = new AspNetQueryStringLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.QueryStringKeys = new List<string> { "Id", "Id2" };
            renderer.OutputFormat = AspNetLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Multiple_Item_Json_Formatting()
        {
            var expectedResult = "[" + "{\"Id\":\"1\"}," + Environment.NewLine + "{\"Id2\":\"2\"}" + "]";
            var httpContext = Substitute.For<HttpContextBase>();
            var namedClollection = new NameValueCollection();
            namedClollection.Add("Id", "1");
            namedClollection.Add("Id2", "2");
            httpContext.Request.QueryString.Returns(namedClollection);

            var renderer = new AspNetQueryStringLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.QueryStringKeys = new List<string> { "Id", "Id2" };
            renderer.OutputFormat = AspNetLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }       
    }
}
#endif