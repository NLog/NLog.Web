using System;
using NLog.Web.Tests.LayoutRenderers;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogRequestLoggingModuleTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void HttpRequestCompletedTest()
        {
            var testTarget = new NLog.Targets.MemoryTarget() { Layout = "${level}|${message}${exception}" };
            var logFactory = new LogFactory().Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteTo(testTarget);
            }).LogFactory;
            var httpContext = SetUpFakeHttpContext();
            var httpModule = new NLogRequestLoggingModule(logFactory.GetCurrentClassLogger()) { DurationThresholdMs = 5000 };
            httpModule.OnEndRequest(httpContext);

            Assert.Single(testTarget.Logs);
            Assert.Equal("Info|HttpRequest Completed", testTarget.Logs[0]);
        }

        [Fact]
        public void HttpRequestSlowTest()
        {
            var testTarget = new NLog.Targets.MemoryTarget() { Layout = "${level}|${message}${exception}" };
            var logFactory = new LogFactory().Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteTo(testTarget);
            }).LogFactory;
            var httpContext = SetUpFakeHttpContext();
            var httpModule = new NLogRequestLoggingModule(logFactory.GetCurrentClassLogger()) { DurationThresholdMs = 50 };
            System.Threading.Thread.Sleep(50);
            httpModule.OnEndRequest(httpContext);

            Assert.Single(testTarget.Logs);
            Assert.Equal("Warn|HttpRequest Slow", testTarget.Logs[0]);
        }

        [Fact]
        public void HttpRequestFailedTest()
        {
            var testTarget = new NLog.Targets.MemoryTarget() { Layout = "${level}|${message}${exception}" };
            var logFactory = new LogFactory().Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteTo(testTarget);
            }).LogFactory;
            var httpContext = SetUpFakeHttpContext();
            httpContext.Response.StatusCode = 503;
            var httpModule = new NLogRequestLoggingModule(logFactory.GetCurrentClassLogger());
            httpModule.OnEndRequest(httpContext);

            Assert.Single(testTarget.Logs);
            Assert.Equal("Warn|HttpRequest Failure", testTarget.Logs[0]);
        }

        [Fact]
        public void HttpRequestExcludedTest()
        {
            var testTarget = new NLog.Targets.MemoryTarget() { Layout = "${level}|${message}${exception}" };
            var logFactory = new LogFactory().Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().WriteTo(testTarget);
            }).LogFactory;
            var httpContext = SetUpFakeHttpContext();
            var httpModule = new NLogRequestLoggingModule(logFactory.GetCurrentClassLogger());
            httpModule.ExcludeRequestPaths.Add("/documentation/");
            httpModule.OnEndRequest(httpContext);

            Assert.Single(testTarget.Logs);
            Assert.Equal("Debug|HttpRequest Completed", testTarget.Logs[0]);
        }
    }
}
