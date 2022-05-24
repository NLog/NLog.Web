using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogRequestPostedBodyMiddlewareTests
    {
//#if !ASP_NET_CORE2

        /// <summary>
        /// This acts as a parameter for the RequestDelegate parameter for the middleware InvokeAsync method
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task Next(HttpContext context)
        {
            return Task.CompletedTask;
        }

        [Fact]
        public void SuccessTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Request.Body = new MemoryStream();
            byte[] bodyBytes = Encoding.UTF8.GetBytes("This is a test request body");
            defaultContext.Request.Body.Write(bodyBytes,0,bodyBytes.Length);
            defaultContext.Request.ContentLength = bodyBytes.Length;

            // Act

            long streamBeforePosition = defaultContext.Request.Body.Position;

            var middlewareInstance = new NLogRequestPostedBodyMiddleware(NLogRequestPostedBodyMiddlewareConfiguration.Default);
            middlewareInstance.InvokeAsync(defaultContext, Next).ConfigureAwait(false).GetAwaiter().GetResult();

            long streamAfterPosition = defaultContext.Request.Body.Position;

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Single(defaultContext.Items);
            Assert.NotNull(defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey]);
            Assert.True(defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] is string);
            Assert.Equal("This is a test request body", defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] as string);
            Assert.Equal(streamBeforePosition, streamAfterPosition);
        }

        [Fact]
        public void SuccessWithCustomConfigurationTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Request.Body = new MemoryStream();
            byte[] bodyBytes = Encoding.ASCII.GetBytes("This is a test request body");
            defaultContext.Request.Body.Write(bodyBytes, 0, bodyBytes.Length);
            defaultContext.Request.ContentLength = bodyBytes.Length;

            // Act

            long streamBeforePosition = defaultContext.Request.Body.Position;

            var configuration = new NLogRequestPostedBodyMiddlewareConfiguration
            {
                BufferSize = 4096,
                DetectEncodingFromByteOrderMark = false,
                Encoding = Encoding.ASCII
            };

            var middlewareInstance = new NLogRequestPostedBodyMiddleware(NLogRequestPostedBodyMiddlewareConfiguration.Default);
            middlewareInstance.InvokeAsync(defaultContext, Next).ConfigureAwait(false).GetAwaiter().GetResult();

            long streamAfterPosition = defaultContext.Request.Body.Position;

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Single(defaultContext.Items);
            Assert.NotNull(defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey]);
            Assert.True(defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] is string);
            Assert.Equal("This is a test request body", defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] as string);
            Assert.Equal(streamBeforePosition, streamAfterPosition);
        }

        [Fact]
        public void EmptyBodyTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Request.Body = new MemoryStream();
            defaultContext.Request.ContentLength = 0;

            // Act
            long streamBeforePosition = defaultContext.Request.Body.Position;

            var middlewareInstance = new NLogRequestPostedBodyMiddleware(NLogRequestPostedBodyMiddlewareConfiguration.Default);
            middlewareInstance.InvokeAsync(defaultContext, Next).ConfigureAwait(false).GetAwaiter().GetResult();

            long streamAfterPosition = defaultContext.Request.Body.Position;

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Single(defaultContext.Items);
            Assert.NotNull(defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey]);
            Assert.True(defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] is string);
            Assert.Equal(string.Empty, defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] as string);
            Assert.Equal(streamBeforePosition, streamAfterPosition);
        }

        [Fact]
        public void NullBodyTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Request.Body = null;

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(NLogRequestPostedBodyMiddlewareConfiguration.Default);
            middlewareInstance.InvokeAsync(defaultContext, Next).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }

        [Fact]
        public void ContentLengthTooLargeTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Request.Body = new MemoryStream();
            defaultContext.Request.Body.Write(new byte[30 * 1024 + 1],0, 30 * 1024 + 1);
            defaultContext.Request.ContentLength = 30 * 1024 + 1;

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(NLogRequestPostedBodyMiddlewareConfiguration.Default);
            middlewareInstance.InvokeAsync(defaultContext, Next).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }

        [Fact]
        public void MissingContentLengthTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Request.Body = new MemoryStream();
            defaultContext.Request.Body.Write(new byte[128],0,128);
            defaultContext.Request.ContentLength = null;

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(NLogRequestPostedBodyMiddlewareConfiguration.Default);
            middlewareInstance.InvokeAsync(defaultContext, Next).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }
        /*
        [Fact]
        public void CannotReadLengthTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            var stream = Substitute.For<MemoryStream>();
            byte[] bodyBytes = Encoding.ASCII.GetBytes("This is a test request body");
            stream.Write(bodyBytes);  // Throws an InvalidProgramException: Cannot create boxed ByRef-like values
            defaultContext.Request.Body = stream;
            defaultContext.Request.ContentLength = bodyBytes.Length;

            defaultContext.Request.Body.CanRead.Returns(false);

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(NLogRequestPostedBodyMiddlewareConfiguration.Default);
            middlewareInstance.InvokeAsync(defaultContext, Next).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }

        [Fact]
        public void CannotSeekLengthTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            var stream = Substitute.For<MemoryStream>();
            byte[] bodyBytes = Encoding.ASCII.GetBytes("This is a test request body");
            stream.Write(bodyBytes);  // Throws an InvalidProgramException: Cannot create boxed ByRef-like values
            defaultContext.Request.Body = stream;
            defaultContext.Request.ContentLength = bodyBytes.Length;

            defaultContext.Request.Body.CanSeek.Returns(false);

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(NLogRequestPostedBodyMiddlewareConfiguration.Default);
            middlewareInstance.InvokeAsync(defaultContext, Next).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }
        */
//#endif
    }
}
