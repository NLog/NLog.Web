using System;
using System.Collections.Generic;
using NLog.Web.LayoutRenderers;
using NLog.Web.Enums;
using Xunit;
using System.Collections.Specialized;

#if !ASP_NET_CORE
using NSubstitute;
using System.Web;
#else
using Microsoft.Extensions.Primitives;
#endif

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestHeadersLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void NullKeyRendersAllHeaders()
        {
            var expectedResult = "key=TEST,Key1=TEST1";
            var renderer = CreateRenderer();
            renderer.HeaderNames = null;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void NullKeyRendersAllHeadersExceptExcluded()
        {
            var expectedResult = "Key1=TEST1";
            var renderer = CreateRenderer();
            renderer.HeaderNames = null;
            renderer.Exclude.Add("key");

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Flat_Formatting()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.HeaderNames = new List<string> { "notfound" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.HeaderNames = new List<string> { "notfound" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Headers_Flat_Formatting()
        {
            var expectedResult = "key=TEST,Key1=TEST1";

            var renderer = CreateRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Headers_Flat_Formatting_separators()
        {
            var expectedResult = "key:TEST|Key1:TEST1";

            var renderer = CreateRenderer();
            renderer.ValueSeparator = ":";
            renderer.ItemSeparator = "|";

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Headers_Flat_Formatting_separators_layouts()
        {
            var expectedResult = "key>TEST" + Environment.NewLine + "Key1>TEST1";

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
        public void KeyFoundRendersValue_Single_Header_Flat_Formatting()
        {
            var expectedResult = "key=TEST";

            var renderer = CreateRenderer(addSecondHeader: false);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Header_Json_Formatting()
        {
            var expectedResult = "[{\"key\":\"TEST\"}]";

            var renderer = CreateRenderer(addSecondHeader: false);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Header_Json_Formatting_no_array()
        {
            var expectedResult = "{\"key\":\"TEST\"}";

            var renderer = CreateRenderer(addSecondHeader: false);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.SingleAsArray = false;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void KeyFoundRendersValue_Multiple_Headers_Json_Formatting(bool singleAsArray)
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
            renderer.HeaderNames = new List<string> { "notfound" };
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting_ValuesOnly()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.HeaderNames = new List<string> { "notfound" };
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyFoundRendersValue_Header_Multiple_Items_Flat_Formatting_ValuesOnly()
        {
            var expectedResult = "TEST,TEST1";

            var renderer = CreateRenderer();
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Header_Multiple_Items_Flat_Formatting_separators_ValuesOnly()
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

            var renderer = CreateRenderer(addSecondHeader: false);
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Item_Json_Formatting_ValuesOnly()
        {
            var expectedResult = "[\"TEST\"]";

            var renderer = CreateRenderer(addSecondHeader: false);
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

            var renderer = CreateRenderer(addSecondHeader: false);

            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.SingleAsArray = false;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void KeyFoundRendersValue_Header_Multiple_Items_Json_Formatting_ValuesOnly(bool singleAsArray)
        {
            var expectedResult = "[\"TEST\",\"TEST1\"]";

            var renderer = CreateRenderer();

            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;
            renderer.SingleAsArray = singleAsArray;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Create headers renderer with mocked HTTP context
        /// </summary>
        /// <param name="addSecondHeader">Add second header</param>
        /// <returns>Created headers layout renderer</returns>
        private AspNetRequestHeadersLayoutRenderer CreateRenderer(bool addSecondHeader = true)
        {
            var headerNames = new List<string>();
#if ASP_NET_CORE
            var httpContext = HttpContext;
#else
            var httpContext = Substitute.For<HttpContextBase>();
#endif

#if ASP_NET_CORE
            headerNames.Add("key");
            httpContext.Request.Headers.Add("key", new StringValues("TEST"));

            if (addSecondHeader)
            {
                headerNames.Add("Key1");
                httpContext.Request.Headers.Add("Key1", new StringValues("TEST1"));
            }
#else
            var headers = new NameValueCollection();
            headers.Add("key", "TEST");
            headerNames.Add("key");

            if (addSecondHeader)
            {
                headers.Add("Key1", "TEST1");
                headerNames.Add("Key1");
            }

            httpContext.Request.Headers.Returns(headers);
#endif

            var renderer = new AspNetRequestHeadersLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.HeaderNames = headerNames;
            return renderer;
        }
    }
}
