using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Web.Internal;
using NLog.Web.Targets.Wrappers;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogBufferingTargetWrapperMiddlewareTests
    {
        private static LogFactory RegisterSingleDebugTarget()
        {
            LogManager.LogFactory.Setup().RegisterNLogWeb().LoadConfigurationFromXml(
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
                </nlog>");
            return LogManager.LogFactory;
        }

        private static LogFactory RegisterSingleMemoryTarget()
        {
            LogManager.LogFactory.Setup().RegisterNLogWeb().LoadConfigurationFromXml(
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
                </nlog>");
            return LogManager.LogFactory;
        }

        private static LogFactory RegisterMultipleMemoryTargets()
        {
            LogManager.LogFactory.Setup().RegisterNLogWeb().LoadConfigurationFromXml(
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
                </nlog>");
            return LogManager.LogFactory;
        }

        [Fact]
        public async Task TestSingleDebugTarget()
        {
            var logFactory = RegisterSingleDebugTarget();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only"));

            DefaultHttpContext context = new DefaultHttpContext();

            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            RequestDelegate next = (HttpContext hc) =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }

                return Task.CompletedTask;
            };

            NLogBufferingTargetWrapperMiddleware middleware = new NLogBufferingTargetWrapperMiddleware(next);

            await middleware.Invoke(context).ConfigureAwait(false);

            Assert.NotEmpty(context.Items);

            var eventBufferKeyPair = context.Items.First();

            Assert.NotNull(eventBufferKeyPair.Key);

            var eventBuffer = eventBufferKeyPair.Value;

            Assert.NotNull(eventBuffer);

            var nlogEventBuffer = eventBuffer as Dictionary<AspNetBufferingTargetWrapper, LogEventInfoBuffer>;

            Assert.NotNull(nlogEventBuffer);

            var wrappedTarget = target.WrappedTarget;
            var debugTarget = wrappedTarget as DebugTarget;

            Assert.NotNull(debugTarget);

            // Buffer limit is set to 9 above
            Assert.Equal(9, debugTarget.Counter);

            Assert.Equal("9", debugTarget.LastMessage);

            LogManager.Shutdown();
        }

        [Fact]
        public async Task TestSingleMemoryTarget()
        {
            var logFactory = RegisterSingleMemoryTarget();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only"));

            DefaultHttpContext context = new DefaultHttpContext();

            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            RequestDelegate next = (HttpContext hc) =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }

                return Task.CompletedTask;
            };

            NLogBufferingTargetWrapperMiddleware middleware = new NLogBufferingTargetWrapperMiddleware(next);

            await middleware.Invoke(context).ConfigureAwait(false);

            Assert.NotEmpty(context.Items);

            var eventBufferKeyPair = context.Items.First();

            Assert.NotNull(eventBufferKeyPair.Key);

            var eventBuffer = eventBufferKeyPair.Value;

            Assert.NotNull(eventBuffer);

            var nlogEventBuffer = eventBuffer as Dictionary<AspNetBufferingTargetWrapper, LogEventInfoBuffer>;

            Assert.NotNull(nlogEventBuffer);

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

            LogManager.Shutdown();
        }

        [Fact]
        public async Task TestSingleDebugTargetWithNullContext()
        {
            var logFactory = RegisterSingleDebugTarget();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only"));

            DefaultHttpContext context = null;

            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            RequestDelegate next = (HttpContext hc) =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }

                return Task.CompletedTask;
            };

            NLogBufferingTargetWrapperMiddleware middleware = new NLogBufferingTargetWrapperMiddleware(next);

            await middleware.Invoke(context).ConfigureAwait(false);

            var wrappedTarget = target.WrappedTarget;
            var debugTarget = wrappedTarget as DebugTarget;

            Assert.NotNull(debugTarget);

            // There should be 10 messages even if the buffer limit was 9.
            // Due to null http context we skipped the buffering target
            // so we should have all 10 messages.
            Assert.Equal(10, debugTarget.Counter);

            Assert.Equal("9", debugTarget.LastMessage);

            LogManager.Shutdown();
        }

        [Fact]
        public async Task TestSingleMemoryTargetWithNullContext()
        {
            var logFactory = RegisterSingleMemoryTarget();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only"));

            DefaultHttpContext context = null;

            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            RequestDelegate next = (HttpContext hc) =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }

                return Task.CompletedTask;
            };

            NLogBufferingTargetWrapperMiddleware middleware = new NLogBufferingTargetWrapperMiddleware(next);

            await middleware.Invoke(context).ConfigureAwait(false);

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

            LogManager.Shutdown();
        }

        [Fact]
        public async Task TestMultipleMemoryTargets()
        {
            var logFactory = RegisterMultipleMemoryTargets();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first"));
            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second"));
            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third"));

            DefaultHttpContext context = new DefaultHttpContext();

            var firstTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first");
            firstTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            var secondTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second");
            secondTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            var thirdTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third");
            thirdTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            RequestDelegate next = (HttpContext hc) =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }

                return Task.CompletedTask;
            };

            NLogBufferingTargetWrapperMiddleware middleware = new NLogBufferingTargetWrapperMiddleware(next);

            await middleware.Invoke(context).ConfigureAwait(false);

            Assert.NotEmpty(context.Items);

            var eventBufferKeyPair = context.Items.First();

            Assert.NotNull(eventBufferKeyPair.Key);

            var eventBuffer = eventBufferKeyPair.Value;

            Assert.NotNull(eventBuffer);

            var nlogEventBuffer = eventBuffer as Dictionary<AspNetBufferingTargetWrapper, LogEventInfoBuffer>;

            Assert.NotNull(nlogEventBuffer);

            TestMemoryTargetForMultipleCase(firstTarget);
            TestMemoryTargetForMultipleCase(secondTarget);
            TestMemoryTargetForMultipleCase(thirdTarget);

            LogManager.Shutdown();
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
        public async Task TestMultipleMemoryTargetsWithNullContext()
        {
            var logFactory = RegisterMultipleMemoryTargets();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first"));
            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second"));
            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third"));

            DefaultHttpContext context = null;

            var firstTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first");
            firstTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            var secondTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second");
            secondTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            var thirdTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third");
            thirdTarget.HttpContextAccessor = new FakeHttpContextAccessor(context);

            RequestDelegate next = (HttpContext hc) =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug(i.ToString);
                }

                return Task.CompletedTask;
            };

            NLogBufferingTargetWrapperMiddleware middleware = new NLogBufferingTargetWrapperMiddleware(next);

            await middleware.Invoke(context).ConfigureAwait(false);

            TestMemoryTargetForMultipleCaseNullContext(firstTarget);
            TestMemoryTargetForMultipleCaseNullContext(secondTarget);
            TestMemoryTargetForMultipleCaseNullContext(thirdTarget);

            LogManager.Shutdown();
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
    }
}
