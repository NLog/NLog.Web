using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Web.Targets.Wrappers;
using NLog.Web.Tests.LayoutRenderers;
using Xunit;

namespace NLog.Web.Tests
{
    public class AspNetBufferingTargetWrapperTests : TestInvolvingAspNetHttpContext
    {
        private static LogFactory RegisterSingleDebugTarget()
        {
            return new LogFactory().Setup().RegisterNLogWeb().LoadConfigurationFromXml(
                @"<nlog throwConfigExceptions='true'>
                    <targets>
                        <target 
                            type='AspNetBufferingWrapper' 
                            name='only' 
                            bufferGrowLimit='9' 
                            bufferSize='7' 
                            growBufferAsNeeded='true'>
                            <target type='Debug' name='debugTarget' layout='${message}'/>
                        </target>
                    </targets>
                    <rules>
                        <logger name='*' minlevel='Debug' writeTo='only' />
                    </rules>
                </nlog>").LogFactory;
        }

        private static LogFactory RegisterSingleMemoryTarget()
        {
            return new LogFactory().Setup().RegisterNLogWeb().LoadConfigurationFromXml(
                @"<nlog throwConfigExceptions='true'>
                    <targets>
                        <target 
                            type='AspNetBufferingWrapper' 
                            name='only' 
                            bufferGrowLimit='9' 
                            bufferSize='7' 
                            growBufferAsNeeded='true'>
                            <target type='Memory' name='memoryTarget' layout='${message}'/>
                        </target>
                    </targets>
                    <rules>
                        <logger name='*' minlevel='Debug' writeTo='only' />
                    </rules>
                </nlog>").LogFactory;
        }

        private static LogFactory RegisterMultipleMemoryTargets()
        {
            return new LogFactory().Setup().RegisterNLogWeb().LoadConfigurationFromXml(
                @"<nlog throwConfigExceptions='true'>
                    <targets>
                        <target 
                            type='AspNetBufferingWrapper' 
                            name='first' 
                            bufferGrowLimit='9' 
                            bufferSize='7' 
                            growBufferAsNeeded='true'>
                            <target type='Memory' name='memoryTarget1' layout='${message}'/>
                        </target>
                        <target 
                            type='AspNetBufferingWrapper' 
                            name='second' 
                            bufferGrowLimit='9' 
                            bufferSize='7' 
                            growBufferAsNeeded='true'>
                            <target type='Memory' name='memoryTarget2' layout='${message}'/>
                        </target>
                        <target 
                            type='AspNetBufferingWrapper' 
                            name='third' 
                            bufferGrowLimit='9' 
                            bufferSize='7' 
                            growBufferAsNeeded='true'>
                            <target type='Memory' name='memoryTarget3' layout='${message}'/>
                        </target>
                    </targets>
                    <rules>
                        <logger name='*' minlevel='Debug' writeTo='first' />
                        <logger name='*' minlevel='Debug' writeTo='second' />
                        <logger name='*' minlevel='Debug' writeTo='third' />
                    </rules>
                </nlog>").LogFactory;
        }

        [Fact]
        public void TestSingleDebugTarget()
        {
            var context = SetUpFakeHttpContext();
            Assert.NotNull(context);

            var logFactory = RegisterSingleDebugTarget();
            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            Assert.NotNull(target);
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            ExecuteLogging(context, () =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }
            });

            var wrappedTarget = target.WrappedTarget;
            var debugTarget = wrappedTarget as DebugTarget;

            Assert.NotNull(debugTarget);

            // Buffer limit is set to 9 above
            Assert.Equal(9, debugTarget.Counter);

            Assert.Equal("9", debugTarget.LastMessage);

            logFactory.Shutdown();
        }

        [Fact]
        public void TestSingleMemoryTarget()
        {
            var context = SetUpFakeHttpContext();
            Assert.NotNull(context);

            var logFactory = RegisterSingleMemoryTarget();
            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            Assert.NotNull(target);
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            ExecuteLogging(context, () =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }
            });

            var wrappedTarget = target.WrappedTarget;
            var memoryTarget = wrappedTarget as MemoryTarget;

            Assert.NotNull(memoryTarget);

            Assert.NotNull(memoryTarget.Logs);

            Assert.NotEmpty(memoryTarget.Logs);

            Assert.Equal(9, memoryTarget.Logs.Count);

            // We went thru the buffered wrapper where the buffer limit was 9,
            // so in this case we have only 9 messages starting at 1, not
            // 10 messages starting at 0.

            int j = 1;
            foreach (var message in memoryTarget.Logs)
            {
                Assert.Equal(j.ToString(), message);
                j++;
            }

            logFactory.Shutdown();
        }

        [Fact]
        public void TestSingleDebugTargetWithNullContext()
        {
            var context = SetUpFakeHttpContext(true);
            Assert.Null(context);

            var logFactory = RegisterSingleDebugTarget();
            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            Assert.NotNull(target);
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            ExecuteLogging(context, () =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }
            });

            var wrappedTarget = target.WrappedTarget;
            var debugTarget = wrappedTarget as DebugTarget;

            Assert.NotNull(debugTarget);

            // There should be 10 messages even if the buffer limit was 9.
            // Due to null http context we skipped the buffering target
            // so we should have all 10 messages.
            Assert.Equal(10, debugTarget.Counter);

            Assert.Equal("9", debugTarget.LastMessage);

            logFactory.Shutdown();
        }

        [Fact]
        public void TestSingleMemoryTargetWithNullContext()
        {
            var context = SetUpFakeHttpContext(nullContext: true);
            Assert.Null(context);

            var logFactory = RegisterSingleMemoryTarget();
            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            Assert.NotNull(target);
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            ExecuteLogging(context, () =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }
            });

            var wrappedTarget = target.WrappedTarget;
            var memoryTarget = wrappedTarget as MemoryTarget;

            Assert.NotNull(memoryTarget);

            Assert.NotNull(memoryTarget.Logs);

            Assert.NotEmpty(memoryTarget.Logs);

            Assert.Equal(10,memoryTarget.Logs.Count);

            // because we had a null HttpContext, we did not go thru the
            // buffered wrapper where the buffer limit was 9,
            // so in this case we have all 10 messages starting at 0.

            int j = 0;
            foreach (var message in memoryTarget.Logs)
            {
                Assert.Equal(j.ToString(), message);
                j++;
            }

            logFactory.Shutdown();
        }

        [Fact]
        public void TestMultipleMemoryTargets()
        {
            var context = SetUpFakeHttpContext();
            Assert.NotNull(context);

            var logFactory = RegisterMultipleMemoryTargets();
            var firstTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first");
            Assert.NotNull(firstTarget);
            firstTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            var secondTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second");
            Assert.NotNull(secondTarget);
            secondTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            var thirdTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third");
            Assert.NotNull(thirdTarget);
            thirdTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            ExecuteLogging(context, () =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }
            });

            TestMemoryTargetForMultipleCase(firstTarget);
            TestMemoryTargetForMultipleCase(secondTarget);
            TestMemoryTargetForMultipleCase(thirdTarget);

            logFactory.Shutdown();
        }

        private void TestMemoryTargetForMultipleCase(WrapperTargetBase target)
        {
            var wrappedTarget = target.WrappedTarget;
            var memoryTarget = wrappedTarget as MemoryTarget;

            Assert.NotNull(memoryTarget);

            Assert.NotNull(memoryTarget.Logs);

            Assert.NotEmpty(memoryTarget.Logs);

            Assert.Equal(9, memoryTarget.Logs.Count);

            // We went thru the buffered wrapper where the buffer limit was 9,
            // so in this case we have only 9 messages starting at 1, not
            // 10 messages starting at 0.

            int j = 1;
            foreach (var message in memoryTarget.Logs)
            {
                Assert.Equal(j.ToString(), message);
                j++;
            }
        }

        [Fact]
        public void TestMultipleMemoryTargetsWithNullContext()
        {
            var context = SetUpFakeHttpContext(nullContext: true);

            var logFactory = RegisterMultipleMemoryTargets();
            var firstTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first");
            Assert.NotNull(firstTarget);
            firstTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            var secondTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second");
            Assert.NotNull(secondTarget);
            secondTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            var thirdTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third");
            Assert.NotNull(thirdTarget);
            thirdTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            ExecuteLogging(context, () =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }
            });

            TestMemoryTargetForMultipleCaseNullContext(firstTarget);
            TestMemoryTargetForMultipleCaseNullContext(secondTarget);
            TestMemoryTargetForMultipleCaseNullContext(thirdTarget);

            logFactory.Shutdown();
        }

        private void TestMemoryTargetForMultipleCaseNullContext(WrapperTargetBase target)
        {
            var wrappedTarget = target.WrappedTarget;
            var memoryTarget = wrappedTarget as MemoryTarget;

            Assert.NotNull(memoryTarget);

            Assert.NotNull(memoryTarget.Logs);

            Assert.NotEmpty(memoryTarget.Logs);

            Assert.Equal(10, memoryTarget.Logs.Count);

            // We went thru the buffered wrapper where the buffer limit was 9,
            // so in this case we have only 9 messages starting at 1, not
            // 10 messages starting at 0.

            int j = 0;
            foreach (var message in memoryTarget.Logs)
            {
                Assert.Equal(j.ToString(), message);
                j++;
            }
        }

#if ASP_NET_CORE
        private void ExecuteLogging(HttpContext httpContext, Action loggingInvoker)
        {
            RequestDelegate next = (HttpContext hc) =>
            {
                loggingInvoker();
                return Task.CompletedTask;
            };

            NLogBufferingTargetWrapperMiddleware middleware = new NLogBufferingTargetWrapperMiddleware(next);
            middleware.Invoke(httpContext).GetAwaiter().GetResult();
        }
#else
        private void ExecuteLogging(System.Web.HttpContext httpContext, Action loggingInvoker)
        {
            var httpModule = new NLogHttpModule();
            httpModule.OnBeginRequest(httpContext);
            loggingInvoker();
            httpModule.OnEndRequest(httpContext);
        }
#endif

    }
}
