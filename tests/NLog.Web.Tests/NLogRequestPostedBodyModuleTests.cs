using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Hosting;
using NLog.Web.LayoutRenderers;
using NLog.Web.Tests.LayoutRenderers;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogRequestPostedBodyModuleTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void HttpRequestNoBodyTest()
        {
            // Arrange
            var httpContext = SetUpFakeHttpContext();

            // Act
            var httpModule = new NLogRequestPostedBodyModule();
            httpModule.OnBeginRequest(httpContext);

            // Assert
            Assert.NotNull(httpContext.Items);
            Assert.Empty(httpContext.Items);
        }

        [Fact]
        public void HttpRequestBodyTest()
        {
            // Arrange
            var expectedMessage = "Expected message";
            MyWorkerRequest myRequest = new MyWorkerRequest(expectedMessage);
            HttpContext httpContext = new HttpContext(myRequest);

            // Act
            var httpModule = new NLogRequestPostedBodyModule();
            httpModule.OnBeginRequest(httpContext);

            // Assert
            Assert.NotNull(httpContext.Items);
            Assert.Single(httpContext.Items);
            Assert.NotNull(httpContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey]);
            Assert.True(httpContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] is string);
            Assert.Equal(expectedMessage, httpContext.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] as string);
        }

        public class MyWorkerRequest : SimpleWorkerRequest
        {
            private readonly MemoryStream _entityBody;

            public MyWorkerRequest(string entityBody)
                :base("/", "/", "/", "", new StringWriter(CultureInfo.InvariantCulture))
            {
                _entityBody = new MemoryStream();
                StreamWriter sw = new StreamWriter(_entityBody);
                sw.Write(entityBody);
                sw.Flush();
                _entityBody.Position = 0;
            }

            public override bool IsEntireEntityBodyIsPreloaded() => true;
            public override byte[] GetPreloadedEntityBody() => _entityBody.ToArray();
        }
    }
}
