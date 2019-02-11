using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
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

        private static void SetBodyStream(HttpContextBase httpContext, Stream stream)
        {
#if ASP_NET_CORE
            httpContext.Request.Body.Returns(stream);
#else
            httpContext.Request.InputStream.Returns(stream);
#endif
        }

        private static MemoryStream CreateStream(string content)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(content);
            streamWriter.Flush();
            return stream;
        }
    }
}
