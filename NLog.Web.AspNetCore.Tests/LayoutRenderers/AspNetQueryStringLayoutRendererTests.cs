

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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NLog.Web.Enums;
using Xunit;


namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetQueryStringLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void NullKeyRendersEmptyString()
        {
            var renderer = CreateAndMockRenderer();
            renderer.QueryStringKeys = null;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Flat_Formatting()
        {
            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.QueryStringKeys = new List<string> { "key" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void KeyNotFoundRendersEmptyString_Json_Formatting()
        {
            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.QueryStringKeys = new List<string> { "key" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }




        [Fact]
        public void KeyFoundRendersValue_QueryString_Single_Item_Flat_Formatting()
        {
            var expectedResult = "Id=1";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.QueryStringKeys = new List<string> { "Id" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Single_Item_Json_Formatting()
        {
            var expectedResult = "[{\"Id\":\"1\"}]";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"));

            renderer.QueryStringKeys = new List<string> { "Id" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Multiple_Item_Flat_Formatting()
        {
            var expectedResult = "Id=1,Id2=2";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.QueryStringKeys = new List<string> { "Id", "Id2" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void EmptyProperyShouldListAll()
        {
            var expectedResult = "Id=1,Id2=2";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.QueryStringKeys = new List<string> { };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void NullProperyShouldListAll()
        {
            var expectedResult = "Id=1,Id2=2";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.QueryStringKeys = null;
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
        [Fact]
        public void MultipleValuesForOneKeyShouldWork()
        {
            var expectedResult = "Id=1,2,3";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1", "2", "3"));

            renderer.QueryStringKeys = null;
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Flat;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void KeyFoundRendersValue_QueryString_Multiple_Item_Json_Formatting()
        {
            var expectedResult = "[{\"Id\":\"1\"},{\"Id2\":\"2\"}]";

            var renderer = CreateAndMockRenderer(CreateTuple("Id", "1"), CreateTuple("Id2", "2"));

            renderer.QueryStringKeys = new List<string> { "Id", "Id2" };
            renderer.OutputFormat = AspNetRequestLayoutOutputFormat.Json;

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


        private static AspNetQueryStringLayoutRenderer CreateAndMockRenderer(params Tuple<string, string[]>[] values)
        {
            var renderer = new AspNetQueryStringLayoutRenderer();

#if !NETSTANDARD_1plus

            var httpContext = Substitute.For<HttpContextBase>();
            var namedClollection = new NameValueCollection();
            foreach (var tuple in values)
            {
                foreach (var value in tuple.Item2)
                {
                    namedClollection.Add(tuple.Item1, value);
                }

            }

            httpContext.Request.QueryString.Returns(namedClollection);

            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
#else
            var httpContext = Substitute.For<HttpContextBase>();
            var dict = new Dictionary<string, StringValues>();
            foreach (var tuple in values)
            {
                dict.Add(tuple.Item1, new StringValues(tuple.Item2));

            }
            IQueryCollection querystringValues = new QueryCollection(dict);

            httpContext.Request.Query.Returns(querystringValues);

            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
#endif

            return renderer;



        }


    }
}
