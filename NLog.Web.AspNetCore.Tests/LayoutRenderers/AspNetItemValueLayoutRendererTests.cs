using System;
using System.Collections.Generic;
using System.Globalization;
#if !ASP_NET_CORE
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetItemValueLayoutRendererTests : TestBase
    {
        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var renderer = new AspNetItemValueLayoutRenderer();

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void NullVariableRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetItemValueLayoutRenderer();
            renderer.Variable = null;
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Fact]
        public void VariableNotFoundRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetItemValueLayoutRenderer();
            renderer.Variable = "key";
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Theory, MemberData(nameof(VariableFoundData))]
        public void CulturedVariableFoundRendersValue(object expectedValue)
        {
            var httpContext = Substitute.For<HttpContextBase>();
#if ASP_NET_CORE
			httpContext.Items = new Dictionary<object, object>();
			httpContext.Items.Add("key", expectedValue);
#else
            httpContext.Items.Count.Returns(1);
            httpContext.Items.Contains("key").Returns(true);
            httpContext.Items["key"].Returns(expectedValue);
#endif
            var cultureInfo = new CultureInfo("nl-NL");
            var renderer = new AspNetItemValueLayoutRenderer();
            renderer.Variable = "key";
            renderer.Culture = cultureInfo;
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(Convert.ToString(expectedValue, cultureInfo), result);
        }


        [Theory, MemberData(nameof(VariableFoundData))]
        public void VariableFoundRendersValue(object expectedValue)
        {
            var httpContext = Substitute.For<HttpContextBase>();
#if ASP_NET_CORE
            httpContext.Items = new Dictionary<object, object>();
            httpContext.Items.Add("key", expectedValue);
#else
            httpContext.Items.Count.Returns(1);
            httpContext.Items.Contains("key").Returns(true);
            httpContext.Items["key"].Returns(expectedValue);
#endif

            var renderer = new AspNetItemValueLayoutRenderer();
            renderer.Variable = "key";
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(Convert.ToString(expectedValue, CultureInfo.CurrentUICulture), result);
        }

        [Theory, MemberData(nameof(NestedPropertyData))]
        public void NestedPropertyRendersValue(string itemKey, string variable, object data, object expectedValue)
        {
            var httpContext = Substitute.For<HttpContextBase>();
#if ASP_NET_CORE
            httpContext.Items = new Dictionary<object, object>();
            httpContext.Items.Add(itemKey, data);
#else
            httpContext.Items.Count.Returns(1);
            httpContext.Items.Contains(itemKey).Returns(true);
            httpContext.Items[itemKey].Returns(data);
#endif

            var renderer = new AspNetItemValueLayoutRenderer();
            renderer.Variable = variable;
            renderer.EvaluateAsNestedProperties = true;
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(Convert.ToString(expectedValue, CultureInfo.CurrentUICulture), result);
        }

        public static IEnumerable<object[]> VariableFoundData
        {
            get
            {
                yield return new object[] { "string" };
                yield return new object[] { 1 };
                yield return new object[] { 1.5 };
                yield return new object[] { DateTime.Now };
                yield return new object[] { Tuple.Create("a", 1) };
            }
        }

        public static IEnumerable<object[]> NestedPropertyData
        {
            get
            {
                yield return new object[] { "key", "key.Item1", Tuple.Create("value"), "value" };
                yield return new object[] { "key", "key.Item1.Item1", Tuple.Create(Tuple.Create(1)), 1 };
            }
        }
    }
}
