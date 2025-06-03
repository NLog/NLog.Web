using System;
using System.Collections.Generic;
using System.Globalization;
#if !ASP_NET_CORE
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
using HttpSessionState = Microsoft.AspNetCore.Http.ISession;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NLog.Web.Enums;
using Xunit;


namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestQueryStringLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void NullKeyRendersEmptyString()
        {
            var renderer = CreateAndMockRenderer();
            renderer.Items = null;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Flat_Formatting()
        {
            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.Items = new List<string> { "key" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting()
        {
            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.Items = new List<string> { "key" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Single_Item_Flat_Formatting()
        {
            var expectedResult = "Id=1";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.Items = new List<string> { "Id" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Single_Item_Json_Formatting()
        {
            var expectedResult = "[{\"Id\":\"1\"}]";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.Items = new List<string> { "Id" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Single_Item_Json_Formatting_KeyLowercase()
        {
            var expectedResult = "[{\"id\":\"1\"}]";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.Items = new List<string> { "Id" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;
            renderer.LowerCaseKeys = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Multiple_Item_Flat_Formatting()
        {
            var expectedResult = "Id=1,Id2=2";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.Items = new List<string> { "Id", "Id2" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Multiple_Item_Flat_Formatting_KeyLowercase()
        {
            var expectedResult = "id=1,id2=2";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.Items = new List<string> { "Id", "Id2" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.LowerCaseKeys = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EmptyProperyShouldListAll()
        {
            var expectedResult = "Id=1,Id2=2";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.Items = new List<string> { };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void NullProperyShouldListAll()
        {
            var expectedResult = "Id=1,Id2=2";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.Items = null;
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void NullKeyRendersAllExceptExcluded()
        {
            var expectedResult = "Id=1,Id2=2";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.Items = null;
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.Exclude.Add("1");

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void MultipleValuesForOneKeyShouldWork()
        {
            var expectedResult = "Id=1,2,3";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1", "2", "3"));

            renderer.Items = null;
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void MultipleValuesJsonQuoted()
        {
            var expectedResult = @"{""Id"":""a'b,\""c\""""}";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "a'b", "\"c\""));

            renderer.Items = null;
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonDictionary;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Multiple_Item_Json_Formatting()
        {
            var expectedResult = "[{\"Id\":\"1\"},{\"Id2\":\"2\"}]";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.Items = new List<string> { "Id", "Id2" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Single_Item_Flat_Formatting_ValuesOnly()
        {
            var expectedResult = "1";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.Items = new List<string> { "Id" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Single_Item_Json_Formatting_ValuesOnly()
        {
            var expectedResult = "[\"1\"]";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.Items = new List<string> { "Id" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Multiple_Item_Flat_Formatting_ValuesOnly()
        {
            var expectedResult = "1,2";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.Items = new List<string> { "Id", "Id2" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Multiple_Item_Json_Formatting_ValuesOnly()
        {
            var expectedResult = "[\"1\",\"2\"]";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.Items = new List<string> { "Id", "Id2" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void MultipleValuesForOneKeyShouldWork_ValuesOnly()
        {
            var expectedResult = "1,2,3";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1", "2", "3"));

            renderer.Items = null;
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;
            renderer.ValuesOnly = true;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }


        /// <summary>
        /// Create tuple with 1 or more values (with 1 key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static Tuple<string, string[]> CreateTuple(string key, params string[] values)
        {
            return new Tuple<string, string[]>(key, values);
        }

        private AspNetRequestQueryStringLayoutRenderer CreateAndMockRenderer(params Tuple<string, string[]>[] pairs)
        {
#if !ASP_NET_CORE
            var httpContext = Substitute.For<HttpContextBase>();
            var pairCollection = new NameValueCollection();
#else
            var httpContext = SetUpFakeHttpContext();
            var pairCollection = new QueryBuilder();
#endif

            foreach (var tuple in pairs)
            {
                foreach (var value in tuple.Item2)
                {
                    pairCollection.Add(tuple.Item1, value);
                }
            }

#if !ASP_NET_CORE
            httpContext.Request.QueryString.Returns(pairCollection);
#else
            httpContext.Request.QueryString = pairCollection.ToQueryString();
#endif

            var renderer = new AspNetRequestQueryStringLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            return renderer;
        }
    }
}
