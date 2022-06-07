using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogResponseBodyMiddlewareTests
    {
        /// <summary>
        /// This acts as a parameter for the RequestDelegate parameter for the middleware InvokeAsync method
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task Next(HttpContext context)
        {
            byte[] bodyBytes = Encoding.UTF8.GetBytes("This is a test response body");
            context.Response.Body.Write(bodyBytes, 0, bodyBytes.Length);
            return Task.CompletedTask;
        }

        /// <summary>
        /// This acts as a parameter for the RequestDelegate parameter for the middleware InvokeAsync method
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task NextNone(HttpContext context)
        {
            return Task.CompletedTask;
        }

        [Fact]
        public void SuccessTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Response.Body = new MemoryStream();
            defaultContext.Response.ContentLength = "This is a test response body".Length;
            defaultContext.Response.ContentType = "text/plain";

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(Next, NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Single(defaultContext.Items);
            Assert.NotNull(defaultContext.Items[AspNetResponseBodyLayoutRenderer.NLogResponseBodyKey]);
            Assert.True(defaultContext.Items[AspNetResponseBodyLayoutRenderer.NLogResponseBodyKey] is string);
            Assert.Equal("This is a test response body", defaultContext.Items[AspNetResponseBodyLayoutRenderer.NLogResponseBodyKey] as string);
        }

        [Fact]
        public void NullContextTest()
        {
            // Arrange
            HttpContext defaultContext = null;

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(Next, NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            // Assert that we got to this point without NullReferenceException
            Assert.True(true);
        }

        [Fact]
        public void EmptyBodyTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Response.Body = new MemoryStream();

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(NextNone, NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }

        [Fact]
        public void NullResponseTest()
        {
            // Arrange
            HttpContext defaultContext = Substitute.For<HttpContext>();
            defaultContext.Response.Body.Returns((Stream) null);

            var middlewareInstance = new NLogRequestPostedBodyMiddleware(NextNone, NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }
    }
}
