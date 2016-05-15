using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NLog.Web.Enums;
using Xunit;
using System.Collections.Specialized;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetQueryStringLayoutRendererTests
    {
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
            var expectedResult = "{\"Id\":\"1\"}";
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
