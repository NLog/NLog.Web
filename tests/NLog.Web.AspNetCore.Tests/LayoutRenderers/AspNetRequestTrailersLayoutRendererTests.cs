#if ASP_NET_CORE3
using System;
using System.Collections.Generic;
using NLog.Web.LayoutRenderers;
using NLog.Web.Enums;
using Xunit;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Microsoft.AspNetCore.Http;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestTrailersLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestTrailersLayoutRenderer>
    {
        [Fact]
        public void NullKeyRendersAllHeaders()
        {
            var expectedResult = "key=TEST,Key1=TEST1";
            var renderer = CreateRenderer();
            renderer.TrailerNames = null;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void NullKeyRendersAllHeadersExceptExcluded()
        {
            var expectedResult = "Key1=TEST1";
            var renderer = CreateRenderer();
            renderer.TrailerNames = null;
            renderer.Exclude.Add("key");

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Flat_Formatting()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.TrailerNames = new List<string> { "notfound" };

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;
            renderer.TrailerNames = new List<string> { "notfound" };

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
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Single_Header_Json_Formatting_no_array()
        {
            var expectedResult = "{\"key\":\"TEST\"}";

            var renderer = CreateRenderer(addSecondHeader: false);
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonDictionary;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Headers_Json_Formatting()
        {
            var expectedResult = "[{\"key\":\"TEST\"},{\"Key1\":\"TEST1\"}]";

            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Multiple_Headers_Json_Formatting_no_array()
        {
            var expectedResult = "{\"key\":\"TEST\",\"Key1\":\"TEST1\"}";

            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonDictionary;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Flat_Formatting_ValuesOnly()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.TrailerNames = new List<string> { "notfound" };
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting_ValuesOnly()
        {
            var renderer = CreateRenderer();
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;
            renderer.TrailerNames = new List<string> { "notfound" };
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
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;
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

            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonDictionary;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_Header_Multiple_Items_Json_Formatting_ValuesOnly()
        {
            var expectedResult = "[\"TEST\",\"TEST1\"]";

            var renderer = CreateRenderer();

            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonDictionary;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Create headers renderer with mocked HTTP context
        /// </summary>
        /// <param name="addSecondHeader">Add second header</param>
        /// <returns>Created headers layout renderer</returns>
        private static AspNetRequestTrailersLayoutRenderer CreateRenderer(bool addSecondHeader = true)
        {
            var (renderer, httpContext) = CreateWithHttpContext();

            var trailerNames = new List<string>();

            var requestTrailerFeature = Substitute.For<IHttpRequestTrailersFeature>();
            requestTrailerFeature.Available.Returns(true);

            var trailers = new HeaderDictionary();

            trailerNames.Add("key");
            trailers.Add("key", new StringValues("TEST"));

            if (addSecondHeader)
            {
                trailerNames.Add("Key1");
                trailers.Add("Key1", new StringValues("TEST1"));
            }

            requestTrailerFeature.Trailers.Returns(trailers);

            var featureCollection = new FeatureCollection();
            featureCollection.Set<IHttpRequestTrailersFeature>(requestTrailerFeature);
            httpContext.Features.Returns(featureCollection);

            renderer.TrailerNames = trailerNames;
            return renderer;
        }
    }
}
#endif