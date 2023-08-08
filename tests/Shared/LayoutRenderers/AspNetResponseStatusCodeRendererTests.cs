using System.Net;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetResponseStatusCodeLayoutRendererTests : LayoutRenderersTestBase<AspNetResponseStatusCodeLayoutRenderer>
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
        public void StatusCode_Set_Renderer_EnumFormatF()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "f";

            httpContext.Response.StatusCode.Returns(200);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(((HttpStatusCode)200).ToString(), result);
        }

        [Fact]
        public void StatusCode_Set_Renderer_EnumFormatG()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "g";

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
        public void StatusCode_Set_Renderer_HexFormat()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "x";

            httpContext.Response.StatusCode.Returns(200);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal(((HttpStatusCode)200).ToString("x"), result);
        }

        [Fact]
        public void StatusCode_Set_Renderer_InvalidFormat()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "invalid";

            httpContext.Response.StatusCode.Returns(200);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("", result);
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
        [InlineData(1000, false)]
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
        [InlineData(1000, false)]
        public void Only_Render_Valid_StatusCodes_EnumFormatF(int statusCode, bool shouldBeRendered)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "f";
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
        [InlineData(1000, false)]
        public void Only_Render_Valid_StatusCodes_EnumFormatG(int statusCode, bool shouldBeRendered)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "g";
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
        [InlineData(1000, false)]
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
        [InlineData(1000, false)]
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

        [Theory]
        [InlineData(0, false)]
        [InlineData(99, false)]
        [InlineData(100, true)]
        [InlineData(599, true)]
        [InlineData(1000, false)]
        public void Only_Render_Valid_StatusCodes_HexFormat(int statusCode, bool shouldBeRendered)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Format = "x";
            httpContext.Response.StatusCode.Returns(statusCode);

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            if (shouldBeRendered)
            {
                Assert.Equal(((HttpStatusCode)statusCode).ToString("x"), result);
            }
            else
            {
                Assert.Empty(result);
            }
        }
    }
}
