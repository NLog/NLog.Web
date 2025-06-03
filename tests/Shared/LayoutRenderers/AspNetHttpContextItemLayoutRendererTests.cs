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
    public class AspNetHttpContextItemLayoutRendererTests : LayoutRenderersTestBase<AspNetHttpContextItemLayoutRenderer>
    {
        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Item = string.Empty;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Empty(result);

            // Bonus assert
            renderer.Item = null;
            result = renderer.Render(new LogEventInfo());
            Assert.Empty(result);
        }

        [Fact]
        public void VariableNotFoundRendersEmptyString()
        {
            // Arrange
            var (renderer, _) = CreateWithHttpContext();
            renderer.Item = "key";

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Empty(result);
        }

        [Theory, MemberData(nameof(VariableFoundData))]
        public void CulturedVariableFoundRendersValue(object expectedValue)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

#if ASP_NET_CORE
            httpContext.Items = new Dictionary<object, object>();
            httpContext.Items.Add("key", expectedValue);
#else
            httpContext.Items.Count.Returns(1);
            httpContext.Items.Contains("key").Returns(true);
            httpContext.Items["key"].Returns(expectedValue);
#endif
            var cultureInfo = new CultureInfo("nl-NL");
            renderer.Item = "key";
            renderer.Culture = cultureInfo;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(Convert.ToString(expectedValue, cultureInfo), result);
        }


        [Theory, MemberData(nameof(VariableFoundData))]
        public void VariableFoundRendersValue(object expectedValue)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

#if ASP_NET_CORE
            httpContext.Items = new Dictionary<object, object> {{"key", expectedValue}};
#else
            httpContext.Items.Count.Returns(1);
            httpContext.Items.Contains("key").Returns(true);
            httpContext.Items["key"].Returns(expectedValue);
#endif
            var culture = CultureInfo.CurrentUICulture;
            renderer.Item = "key";
            renderer.Culture = culture;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(Convert.ToString(expectedValue, culture), result);
        }

        [Theory, MemberData(nameof(NestedPropertyData))]
        public void NestedPropertyRendersValueItem(string itemKey, string variable, object data, object expectedValue)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#if ASP_NET_CORE
            httpContext.Items = new Dictionary<object, object> {{itemKey, data}};
#else
            httpContext.Items.Count.Returns(1);
            httpContext.Items.Contains(itemKey).Returns(true);
            httpContext.Items[itemKey].Returns(data);
#endif
            var culture = CultureInfo.CurrentUICulture;
            renderer.Item = variable;
#pragma warning disable CS0618 // Type or member is obsolete
            renderer.EvaluateAsNestedProperties = true;
#pragma warning restore CS0618 // Type or member is obsolete
            renderer.Culture = culture;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(Convert.ToString(expectedValue, culture), result);
        }

        [Theory, MemberData(nameof(NestedPropertyData))]
        public void NestedPropertyRendersValueObjectPath(string itemKey, string variable, object data,
            object expectedValue)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
#if ASP_NET_CORE
            httpContext.Items = new Dictionary<object, object> {{itemKey, data}};
#else
            httpContext.Items.Count.Returns(1);
            httpContext.Items.Contains(itemKey).Returns(true);
            httpContext.Items[itemKey].Returns(data);
#endif
            var culture = CultureInfo.CurrentUICulture;
            renderer.Item = variable;
#pragma warning disable CS0618 // Type or member is obsolete
            renderer.EvaluateAsNestedProperties = true;
#pragma warning restore CS0618 // Type or member is obsolete
            renderer.Culture = culture;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(Convert.ToString(expectedValue, culture), result);
        }

        public static IEnumerable<object[]> VariableFoundData
        {
            get
            {
                yield return new object[] {"string"};
                yield return new object[] {1};
                yield return new object[] {1.5};
                yield return new object[] {DateTime.Now};
                yield return new object[] {Tuple.Create("a", 1)};
            }
        }

        public static IEnumerable<object[]> NestedPropertyData
        {
            get
            {
                yield return new object[] {"key", "key.Item1", Tuple.Create("value"), "value"};
                yield return new object[] {"key", "key.Item1.Item1", Tuple.Create(Tuple.Create(1)), 1};
            }
        }


        // Nested Properties Tests
        internal class Person
        {
            public Name Name { get; set; }
        }

        internal class Name
        {
            public string First { get; set; }
            public string Last { get; set; }
        }

        [Fact]
        public void NestedItemRendersProperly()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            string expectedValue = "John";

            Person person = new Person
            {
                Name = new Name
                {
                    First = "John",
                    Last = "Smith"
                }
            };

#if ASP_NET_CORE
            httpContext.Items = new Dictionary<object, object>();
            httpContext.Items.Add("person", person);
#else
            httpContext.Items.Count.Returns(1);
            httpContext.Items.Contains("person").Returns(true);
            httpContext.Items["person"].Returns(person);
#endif
            renderer.Item = "person.Name.First";

#pragma warning disable CS0618 // Type or member is obsolete
            renderer.EvaluateAsNestedProperties = true;
#pragma warning restore CS0618 // Type or member is obsolete

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedValue, result);
        }


        [Fact]
        public void NestedObjectPathRendersProperly()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            string expectedValue = "Smith";

            Person person = new Person
            {
                Name = new Name
                {
                    First = "John",
                    Last = "Smith"
                }
            };

#if ASP_NET_CORE
            httpContext.Items = new Dictionary<object, object>();
            httpContext.Items.Add("person", person);
#else
            httpContext.Items.Count.Returns(1);
            httpContext.Items.Contains("person").Returns(true);
            httpContext.Items["person"].Returns(person);
#endif
            renderer.Item = "person";
            renderer.ObjectPath = "Name.Last";

#pragma warning disable CS0618 // Type or member is obsolete
            renderer.EvaluateAsNestedProperties = false;
#pragma warning restore CS0618 // Type or member is obsolete

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void NestedObjectPathRendersProperlyII()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            string expectedValue = "Smith";

            Person person = new Person
            {
                Name = new Name
                {
                    First = "John",
                    Last = "Smith"
                }
            };

#if ASP_NET_CORE
            httpContext.Items = new Dictionary<object, object>();
            httpContext.Items.Add("person", person);
#else
            httpContext.Items.Count.Returns(1);
            httpContext.Items.Contains("person").Returns(true);
            httpContext.Items["person"].Returns(person);
#endif
            renderer.Item = "person";
            renderer.ObjectPath = "Name.Last";

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedValue, result);
        }
    }
}
