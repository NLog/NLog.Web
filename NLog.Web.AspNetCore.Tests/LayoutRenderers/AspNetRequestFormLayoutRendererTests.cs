using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
#else
using System.Web;
#endif
using NLog.Web.LayoutRenderers;
using NLog.Web.Tests;
using NLog.Web.Tests.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestFormLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void ShouldReturnEmptyForNonValidContentTypes()
        {
            // Arrange
            var expectedResult = "";
#if ASP_NET_CORE
            var httpContext = this.HttpContext;
#else
            var httpContext = Substitute.For<HttpContextBase>();
#endif
            var renderer = new AspNetRequestFormLayoutRenderer
            {
                HttpContextAccessor = new FakeHttpContextAccessor(httpContext)
            };

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldReturnEmptyIfFormCollectionIsEmpty()
        {
            // Arrange
            var expectedResult = "";
            var renderer = CreateRenderer(false);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldReturnAllIfDefaultsAreUsed()
        {
            // Arrange
            var expectedResult = "id=1,name=Test Person,token=86abe8fe-2237-4f87-81af-0a4e522b4140";
            var renderer = CreateRenderer();

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldReturnOnlySpecifiedIfIncludeIsUsed()
        {
            // Arrange
            var expectedResult = "id=1,name=Test Person";
            var renderer = CreateRenderer();
            renderer.Include.Add("id");
            renderer.Include.Add("name");

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldNotReturnKeysSpecidiedInExclude()
        {
            // Arrange
            var expectedResult = "id=1,name=Test Person";
            var renderer = CreateRenderer();
            renderer.Exclude.Add("token");

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldUseTheSpecifiedSeparator()
        {
            // Arrange
            var expectedResult = "id=1\r\nname=Test Person\r\ntoken=86abe8fe-2237-4f87-81af-0a4e522b4140";
            var renderer = CreateRenderer();
            renderer.ItemSeparator = "${newline}";

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ExcludeShouldTakePrecedenceOverInclude()
        {
            // Arrange
            var expectedResult = "name=Test Person";
            var renderer = CreateRenderer();
            renderer.Include.Add("id");
            renderer.Include.Add("name");
            renderer.Exclude.Add("id");

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(expectedResult, result);
        }

        private AspNetRequestFormLayoutRenderer CreateRenderer(bool hasFormValues = true)
        {
#if ASP_NET_CORE
            var httpContext = this.HttpContext;
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
#else
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.ContentType.Returns("application/x-www-form-urlencoded");
#endif

            if (hasFormValues)
            {
#if ASP_NET_CORE
                var formCollection = new FormCollection(new Dictionary<string, StringValues>{
                    { "id","1" },
                    { "name","Test Person" },
                    { "token","86abe8fe-2237-4f87-81af-0a4e522b4140" }
                });
                httpContext.Request.Form = formCollection;
#else
                var formCollection = new NameValueCollection(){
                    { "id","1" },
                    { "name","Test Person" },
                    { "token","86abe8fe-2237-4f87-81af-0a4e522b4140" }
                };
                httpContext.Request.Form.Returns(formCollection);
#endif
            }

            return new AspNetRequestFormLayoutRenderer
            {
                HttpContextAccessor = new FakeHttpContextAccessor(httpContext)
            };
        }
    }
}
