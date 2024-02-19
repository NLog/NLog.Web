using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using NLog.Extensions.Logging;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogRequestLoggingMiddlewareTests
    {
        [Fact]
        public void HttpRequestCompletedTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Response.Body = new MemoryStream();
            defaultContext.Request.Path = "/";

            var testTarget = new NLog.Targets.MemoryTarget() { Layout = "${level}|${message}" };
            var nlogFactory = new LogFactory().Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteTo(testTarget);
            }).LogFactory;
            var loggerFactory = new NLogLoggerFactory(new NLogLoggerProvider(new NLogProviderOptions(), nlogFactory));

            var middlewareInstance = new NLogRequestLoggingMiddleware(next: (innerHttpContext) =>
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }, loggerFactory: loggerFactory);

            // Act
            middlewareInstance.Invoke(defaultContext).Wait(5000);

            // Assert
            Assert.Single(testTarget.Logs);
            Assert.Equal("Info|HttpRequest Completed", testTarget.Logs[0]);
        }

        [Fact]
        public void HttpRequestExcludedTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Response.Body = new MemoryStream();
            defaultContext.Request.Path = "/documentation/";

            var testTarget = new NLog.Targets.MemoryTarget() { Layout = "${level}|${message}" };
            var nlogFactory = new LogFactory().Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteTo(testTarget);
            }).LogFactory;
            var loggerFactory = new NLogLoggerFactory(new NLogLoggerProvider(new NLogProviderOptions(), nlogFactory));

            var options = new NLogRequestLoggingOptions();
            options.ExcludeRequestPaths.Add("/documentation/");
            var middlewareInstance = new NLogRequestLoggingMiddleware(next: (innerHttpContext) =>
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }, loggerFactory: loggerFactory, options: options);

            // Act
            middlewareInstance.Invoke(defaultContext).Wait(5000);

            // Assert
            Assert.Single(testTarget.Logs);
            Assert.Equal("Debug|HttpRequest Completed", testTarget.Logs[0]);
        }

        [Fact]
        public void HttpRequestFailedTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Response.Body = new MemoryStream();
            defaultContext.Request.Path = "/";

            var testTarget = new NLog.Targets.MemoryTarget() { Layout = "${level}|${message}" };
            var nlogFactory = new LogFactory().Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteTo(testTarget);
            }).LogFactory;
            var loggerFactory = new NLogLoggerFactory(new NLogLoggerProvider(new NLogProviderOptions(), nlogFactory));

            var middlewareInstance = new NLogRequestLoggingMiddleware(next: (innerHttpContext) =>
            {
                innerHttpContext.Response.StatusCode = 503;
                return System.Threading.Tasks.Task.CompletedTask;
            }, loggerFactory: loggerFactory);

            // Act
            middlewareInstance.Invoke(defaultContext).Wait(5000);

            // Assert
            Assert.Single(testTarget.Logs);
            Assert.Equal("Warn|HttpRequest Failure", testTarget.Logs[0]);
        }

        [Fact]
        public void HttpRequestExceptionFilterTest()
        {
            // Arrange
            DefaultHttpContext defaultContext = new DefaultHttpContext();
            defaultContext.Response.Body = new MemoryStream();
            defaultContext.Request.Path = "/";

            var nlogFactory = new LogFactory().Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteTo(new NLog.Targets.MemoryTarget() { Name = "TestTarget", Layout = "${scopeproperty:RequestId} ${exception:format=message}" });
            }).LogFactory;
            var loggerFactory = new NLogLoggerFactory(new NLogLoggerProvider(new NLogProviderOptions(), nlogFactory));

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
    }
}
