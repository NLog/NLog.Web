#if !ASP_NET_CORE
using System;
using System.Collections.Generic;
using System.Globalization;

using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetApplicationValueLayoutRendererTests : TestBase
    {
        [Fact]
        public void NullHttpContextRendersEmptyString()
        {
            var renderer = new AspNetApplicationLayoutRenderer();
            renderer.Item = string.Empty;

            string result = renderer.Render(new LogEventInfo());
            Assert.Empty(result);

            renderer.Item = null;
            result = renderer.Render(new LogEventInfo());
            Assert.Empty(result);
        }

        [Fact]
        public void VariableNotFoundRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetApplicationLayoutRenderer();
            renderer.Item = "key";
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
        }

        [Theory, MemberData(nameof(VariableFoundData))]
        public void VariableFoundRendersValue(object expectedValue)
        {
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Application["key"].Returns(expectedValue);

            var culture = CultureInfo.CurrentUICulture;
            var renderer = new AspNetApplicationLayoutRenderer();
            renderer.Item = "key";
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.Culture = culture;

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(Convert.ToString(expectedValue, culture), result);
        }

        [Fact]
        public void NestedObjectPath()
        {
            var expectedValue = "a";

            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Application["key"].Returns(Tuple.Create(expectedValue, 1));

            var renderer = new AspNetApplicationLayoutRenderer();
            renderer.Item = "key";
            renderer.ObjectPath = "Item1";
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedValue, result);
        }

        public static IEnumerable<object[]> VariableFoundData
        {
            get
            {
                yield return new object[] { "string"};
                yield return new object[] { 1 };
                yield return new object[] { 1.5 };
                yield return new object[] { DateTime.Now };
                yield return new object[] { Tuple.Create("a", 1) };
            }
        }
    }
}
#endif