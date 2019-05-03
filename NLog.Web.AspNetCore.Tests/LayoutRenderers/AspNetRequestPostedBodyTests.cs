using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#else
using System.Web;
#endif
using NLog.Web.LayoutRenderers;
using NLog.Web.Tests.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.AspNetCore.Tests.LayoutRenderers
{
    public class AspNetRequestPostedBodyTests : LayoutRenderersTestBase<AspNetRequestPostedBody>
    {
        [Fact]
        public void NullStreamRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            SetBodyStream(httpContext, null);

            var logEventInfo = new LogEventInfo();

            // Act
            string result = renderer.Render(logEventInfo);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        public void CorrectStreamRendersFullStreamAndRestorePosition(long position)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var json = "{user: 'foo', password: '123'}";
            var stream = CreateStream(json);
            stream.Position = position;
            SetBodyStream(httpContext, stream);

            var logEventInfo = new LogEventInfo();

            // Act
            string result = renderer.Render(logEventInfo);

            // Assert
            Assert.Equal(json, result);
            Assert.Equal(position, stream.Position);
        }

        [Theory]
        [InlineData("", null, "")]
        [InlineData("", 0, "")]
        [InlineData("ABCDEFGHIJK", null, "ABCDEFGHIJK")]
        [InlineData("ABCDEFGHIJK", 0, "ABCDEFGHIJK")]
        [InlineData("ABCDEFGHIJK", 50, "ABCDEFGHIJK")]
        [InlineData("ABCDEFGHIJK", 10, "")]
        public void MaxContentLengthProtectsAgainstLargeBodyStreamTest(string body, int? maxContentLength, string expectedResult)
        {
            MaxContentLengthProtectsAgainstLargeBodyStream(body, maxContentLength, expectedResult, false);
#if ASP_NET_CORE
            MaxContentLengthProtectsAgainstLargeBodyStream(body, maxContentLength, expectedResult, true);
#endif
        }

        private static void MaxContentLengthProtectsAgainstLargeBodyStream(string body, int? maxContentLength, string expectedResult, bool canReadOnce)
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            if (maxContentLength.HasValue)
                renderer.MaxContentLength = maxContentLength.Value;

            var stream = CreateStream(body, canReadOnce);
            SetBodyStream(httpContext, stream);

            var logEventInfo = new LogEventInfo();

            // Act
            string result = renderer.Render(logEventInfo);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        private static void SetBodyStream(HttpContextBase httpContext, Stream stream)
        {
#if ASP_NET_CORE
            httpContext.Request.Body.Returns(stream);
#else
            httpContext.Request.InputStream.Returns(stream);
#endif
            httpContext.Request.ContentLength.Returns((int)(stream?.Length ?? 0));
        }

        private static MemoryStream CreateStream(string content, bool readOnce = false)
        {
            var stream = readOnce ? new ReadOnceStream() : new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(content);
            streamWriter.Flush();
            if (readOnce)
                stream.Position = 0;
            return stream;
        }

        class ReadOnceStream : MemoryStream
        {
            public override bool CanSeek => false;
        }
    }
}
