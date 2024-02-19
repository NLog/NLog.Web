using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogRequestPostedBodyMiddlewareTests
    {
        /// <summary>
        /// This acts as a parameter for the RequestDelegate parameter for the middleware InvokeAsync method
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Task Next(HttpContext context)
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
            defaultContext.Request.ContentType = "text/plain";

            // Act
            long streamBeforePosition = defaultContext.Request.Body.Position;

            var middlewareInstance = new NLogRequestPostedBodyMiddleware(Next, NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).Wait(5000);

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
            defaultContext.Request.ContentType = "text/plain";

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(Next, NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).Wait(5000);

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }

        [Fact]
        public void ExcludContentTypeTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Request.Body = new MemoryStream();
            byte[] bodyBytes = Encoding.UTF8.GetBytes("This is a test request body");
            defaultContext.Request.Body.Write(bodyBytes, 0, bodyBytes.Length);
            defaultContext.Request.ContentLength = bodyBytes.Length;
            defaultContext.Request.ContentType = "application/octet";

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(Next, NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).Wait(5000);

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }

        [Fact]
        public void NullContextTest()
        {
            // Arrange
            HttpContext defaultContext = null;

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(Next, NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).Wait(5000);

            // Assert
            // Assert that we got to this point without NullReferenceException
            Assert.True(true);
        }

        [Fact]
        public void NullRequestTest()
        {
            // Arrange
            HttpContext defaultContext = Substitute.For<HttpContext>();
            defaultContext.Request.ReturnsNull();

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(Next, NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).Wait(5000);

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }

        [Fact]
        public void NullBodyTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Request.Body = null;

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(Next,NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).Wait(5000);

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
            defaultContext.Request.ContentType = "text/plain";

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(Next,NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).ConfigureAwait(false).GetAwaiter().GetResult();

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
            defaultContext.Request.ContentType = "text/plain";

            // Act
            var middlewareInstance = new NLogRequestPostedBodyMiddleware(Next,NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).Wait(5000);

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }

        [Fact]
        public void CannotReadLengthTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();

            defaultContext.Request.Body = Substitute.For<Stream>();
            defaultContext.Request.ContentLength = 1;
            defaultContext.Request.ContentType = "text/plain";

            defaultContext.Request.Body.CanRead.Returns(false);
            defaultContext.Request.Body.CanSeek.Returns(true);

            // Act
            var middlewareInstance =
                new NLogRequestPostedBodyMiddleware(Next,NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).Wait(5000);

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Empty(defaultContext.Items);
        }

        [Fact]
        public void CannotSeekLengthTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();

            defaultContext.Request.Body = new NetworkStream() { _length = 2 };
            defaultContext.Request.ContentLength = 2;
            defaultContext.Request.ContentType = "text/plain";

            // Act
            var middlewareInstance =
                new NLogRequestPostedBodyMiddleware(Next,NLogRequestPostedBodyMiddlewareOptions.Default);
            middlewareInstance.Invoke(defaultContext).Wait(5000);

            // Assert
            Assert.NotNull(defaultContext.Items);
            Assert.Single(defaultContext.Items);
            Assert.NotNull(defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey]);
            Assert.True(defaultContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] is string);
        }

        private sealed class NetworkStream : Stream
        {
            internal long _length;

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => _length;

#if NET6_0_OR_GREATER
            public override long Position { get => throw new System.NotSupportedException(); set => throw new System.NotSupportedException(); }  // Microsoft.AspNetCore.Server.IIS.Core.ReadOnlyStream
#else
            public override long Position { get; set; }
#endif

            public override int Read(byte[] buffer, int offset, int count)
            {
                int delta = Math.Min((int)(Length - offset), count);
                if (delta > 0)
                {
                    _length -= delta;
                    return delta;
                }
                return 0;
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new System.NotSupportedException();  // Microsoft.AspNetCore.Server.IIS.Core.ReadOnlyStream
            public override void SetLength(long value) => throw new System.NotSupportedException(); // Microsoft.AspNetCore.Server.IIS.Core.ReadOnlyStream
            public override void Write(byte[] buffer, int offset, int count) => throw new System.NotSupportedException();   // Microsoft.AspNetCore.Server.IIS.Core.ReadOnlyStream
            public override void Flush() => throw new System.NotSupportedException();   // Microsoft.AspNetCore.Server.IIS.Core.ReadOnlyStream
        }
    }
}
