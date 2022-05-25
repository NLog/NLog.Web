using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogRequestPostedBodyHttpModuleTests
    {
#if !ASP_NET_CORE

        [Fact]
        public void SuccessTest()
        {
            // Arrange
            var stream = new MemoryStream();
            byte[] bodyBytes = Encoding.UTF8.GetBytes("This is a test request body");
            stream.Write(bodyBytes,0,bodyBytes.Length);
            var items = new Dictionary<object,object>();

            // Act
            long streamBeforePosition = stream.Position;

            var httpModule = new NLogRequestPostedBodyHttpModule();

            httpModule.Init(new HttpApplication());

            httpModule.CaptureRequestPostedBody(stream, items, true);

            long streamAfterPosition = stream.Position;

            // Assert
            Assert.NotNull(items);
            Assert.Single(items);
            Assert.NotNull(items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey]);
            Assert.True(items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] is string);
            Assert.Equal("This is a test request body", items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] as string);
            Assert.Equal(streamBeforePosition, streamAfterPosition);
            Assert.Equal("NLog Request Posted Body Module", httpModule.ModuleName);

            httpModule.Dispose();
        }

        [Fact]
        public void EmptyBodyTest()
        {
            // Arrange
            var stream = new MemoryStream();
            var items = new Dictionary<object, object>();

            // Act
            long streamBeforePosition = stream.Position;

            var httpModule = new NLogRequestPostedBodyHttpModule();
            httpModule.CaptureRequestPostedBody(stream, items, true);

            long streamAfterPosition = stream.Position;

            // Assert
            Assert.NotNull(items);
            Assert.Single(items);
            Assert.NotNull(items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey]);
            Assert.True(items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] is string);
            Assert.Equal(string.Empty, items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] as string);
            Assert.Equal(streamBeforePosition, streamAfterPosition);
        }

        [Fact]
        public void NullBodyTest()
        {
            // Arrange
            var items = new Dictionary<object, object>();

            // Act
            var httpModule = new NLogRequestPostedBodyHttpModule();
            httpModule.CaptureRequestPostedBody(null, items, true);

            // Assert
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        [Fact]
        public void ContentLengthTooLargeTest()
        {
            // Arrange
            var stream = new MemoryStream();
            stream.Write(new byte[30*1024+1], 0, 30 * 1024 + 1);
            var items = new Dictionary<object, object>();

            // Act
            var httpModule = new NLogRequestPostedBodyHttpModule();
            httpModule.CaptureRequestPostedBody(stream, items, false);

            // Assert
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        [Fact]
        public void MissingContentLengthTest()
        {
            // Arrange
            var stream = new MemoryStream();
            byte[] bodyBytes = Encoding.UTF8.GetBytes("This is a test request body");
            stream.Write(bodyBytes, 0, bodyBytes.Length);
            var items = new Dictionary<object, object>();

            // Act
            var httpModule = new NLogRequestPostedBodyHttpModule();
            httpModule.CaptureRequestPostedBody(stream, items, false);

            // Assert
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        [Fact]
        public void CannotReadLengthTest()
        {
            // Arrange
            var stream = Substitute.For<MemoryStream>();
            byte[] bodyBytes = Encoding.UTF8.GetBytes("This is a test request body");
            stream.Write(bodyBytes, 0, bodyBytes.Length);
            var items = new Dictionary<object, object>();

            stream.CanRead.Returns(false);
            stream.CanSeek.Returns(true);

            // Act
            var httpModule = new NLogRequestPostedBodyHttpModule();
            httpModule.CaptureRequestPostedBody(stream, items, true);

            // Assert
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        [Fact]
        public void CannotSeekLengthTest()
        {
            // Arrange
            var stream = Substitute.For<MemoryStream>();
            byte[] bodyBytes = Encoding.UTF8.GetBytes("This is a test request body");
            stream.Write(bodyBytes, 0, bodyBytes.Length);
            var items = new Dictionary<object, object>();

            stream.CanRead.Returns(true);
            stream.CanSeek.Returns(false);

            // Act
            var httpModule = new NLogRequestPostedBodyHttpModule();
            httpModule.CaptureRequestPostedBody(stream, items, true);

            // Assert
            Assert.NotNull(items);
            Assert.Empty(items);
        }
#endif
    }
}
