using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using NLog.Extensions.Logging;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogRequestLoggingMiddlewareTests
    {
#if !ASP_NET_CORE2
        [Fact]
        public void ExceptionFilterTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Response.Body = new MemoryStream();
            defaultContext.Request.Path = "/";

            var nlogFactory = new LogFactory().Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteTo(new NLog.Targets.MemoryTarget() { Name = "TestTarget", Layout = "${scopeproperty:RequestId} ${exception:format=message}" });
            }).LogFactory;
            var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddNLog(provider => nlogFactory));

            // Act
            var middlewareInstance = new NLogRequestLoggingMiddleware(next: (innerHttpContext) =>
            {
                using (loggerFactory.CreateLogger("RequestHandler").BeginScope(new[] { new KeyValuePair<string, object>("RequestId", 42) }))
                {
                    if (innerHttpContext != null)
                        throw new ApplicationException("Not good"); // Logging Exception before unwinding stack
                }
                return System.Threading.Tasks.Task.CompletedTask;
            }, loggerFactory: loggerFactory);

            // Assert
            Assert.Throws<ApplicationException>(() =>
            {
                try
                {
                    middlewareInstance.Invoke(defaultContext).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch
                {
                    // Assert
                    var result = nlogFactory.Configuration.FindTargetByName<NLog.Targets.MemoryTarget>("TestTarget")?.Logs?.FirstOrDefault();
                    Assert.Equal("42 Not good", result);
                    throw;
                }
            });
        }
#endif
    }
}
