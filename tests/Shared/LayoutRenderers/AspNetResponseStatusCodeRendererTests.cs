using System.Net;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetResponseStatusCodeRendererTests : LayoutRenderersTestBase<AspNetResponseStatusCodeRenderer>
    {
        [Fact]
        public void StatusCode_Set_Renderer_DefaultFormat()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.Response.StatusCode.Returns(200);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("200", result);
        }

        [Fact]
        public void StatusCode_Set_Renderer_EnumFormat()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "e";

            httpContext.Response.StatusCode.Returns(200);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(((HttpStatusCode)200).ToString(), result);
        }

        [Fact]
        public void StatusCode_Set_Renderer_IntegerFormat()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "d";

            httpContext.Response.StatusCode.Returns(200);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("200", result);
        }

        [Fact]
        public void StatusCode_Set_Renderer_UpperCaseIntegerFormat()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "D";

            httpContext.Response.StatusCode.Returns(200);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("200", result);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(99, false)]
        [InlineData(100, true)]
        [InlineData(599, true)]
        [InlineData(600, false)]
        public void Only_Render_Valid_StatusCodes_DefaultFormat(int statusCode, bool shouldBeRendered)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            httpContext.Response.StatusCode.Returns(statusCode);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            if (shouldBeRendered)
            {
                Assert.Equal($"{statusCode}", result);
            }
            else
            {
                Assert.Empty(result);
            }
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(99, false)]
        [InlineData(100, true)]
        [InlineData(599, true)]
        [InlineData(600, false)]
        public void Only_Render_Valid_StatusCodes_EnumFormat(int statusCode, bool shouldBeRendered)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "e";
            httpContext.Response.StatusCode.Returns(statusCode);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            if (shouldBeRendered)
            {
                Assert.Equal(((HttpStatusCode)statusCode).ToString(), result);
            }
            else
            {
                Assert.Empty(result);
            }
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(99, false)]
        [InlineData(100, true)]
        [InlineData(599, true)]
        [InlineData(600, false)]
        public void Only_Render_Valid_StatusCodes_IntegerFormat(int statusCode, bool shouldBeRendered)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "d";
            httpContext.Response.StatusCode.Returns(statusCode);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            if (shouldBeRendered)
            {
                Assert.Equal($"{statusCode}", result);
            }
            else
            {
                Assert.Empty(result);
            }
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(99, false)]
        [InlineData(100, true)]
        [InlineData(599, true)]
        [InlineData(600, false)]
        public void Only_Render_Valid_StatusCodes_UpperCaseIntegerFormat(int statusCode, bool shouldBeRendered)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "D";
            httpContext.Response.StatusCode.Returns(statusCode);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            if (shouldBeRendered)
            {
                Assert.Equal($"{statusCode}", result);
            }
            else
            {
                Assert.Empty(result);
            }
        }
    }
}
