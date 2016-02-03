using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetItemValueLayoutRendererTests
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

        [Theory, MemberData("VariableFoundData")]
        public void VariableFoundRendersValue(object expectedValue)
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Items["key"].Returns(expectedValue);

            var renderer = new AspNetItemValueLayoutRenderer();
            renderer.Variable = "key";
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(Convert.ToString(expectedValue, CultureInfo.CurrentUICulture), result);
        }

        [Theory, MemberData("NestedPropertyData")]
        public void NestedPropertyRendersValue(string itemKey, string variable, object data, object expectedValue)
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Items[itemKey].Returns(data);

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
