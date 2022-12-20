using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Web.Internal;
using NLog.Web.Targets.Wrappers;
using NLog.Web.Tests.LayoutRenderers;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogHttpModuleTests : TestInvolvingAspNetHttpContext
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
        public void TestSingleDebugTarget()
        {
            var logFactory = RegisterSingleDebugTarget();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only"));

            var context = SetUpFakeHttpContext();

            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            target.HttpContextAccessor = new FakeHttpContextAccessor(new HttpContextWrapper(context));

            // Act
            var httpModule = new NLogHttpModule();
            httpModule.OnBeginRequest(context);

            ILogger logger = logFactory.GetCurrentClassLogger();

            for (int i = 0; i < 10; i++)
            {
                logger.Debug(i.ToString());
            }

            Assert.NotNull(context.Items);
            Assert.NotEmpty(context.Items);
            Assert.Equal(1, context.Items.Count);

            foreach (var itemValue in context.Items.Values)
            {
                var bufferDictionary =
                    itemValue as Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer>;

                Assert.NotNull(bufferDictionary);

                Assert.Single(bufferDictionary);

                foreach (var bufferValue in bufferDictionary.Values)
                {
                    Assert.NotNull(bufferValue);

                    Assert.Equal(9, bufferValue.Count);
                }
            }

            httpModule.OnEndRequest(context);

            var wrappedTarget = target.WrappedTarget;
            var debugTarget = wrappedTarget as DebugTarget;

            Assert.NotNull(debugTarget);

            // Buffer limit is set to 9 above
            Assert.Equal(9, debugTarget.Counter);

            Assert.Equal("9", debugTarget.LastMessage);

            LogManager.Shutdown();
        }

        [Fact]
        public void TestSingleMemoryTarget()
        {
            var logFactory = RegisterSingleMemoryTarget();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only"));

            var context = SetUpFakeHttpContext();

            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            target.HttpContextAccessor = new FakeHttpContextAccessor(new HttpContextWrapper(context));

            // Act
            var httpModule = new NLogHttpModule();
            httpModule.OnBeginRequest(context);

            ILogger logger = logFactory.GetCurrentClassLogger();

            for (int i = 0; i < 10; i++)
            {
                logger.Debug(i.ToString());
            }

            Assert.NotNull(context.Items);
            Assert.NotEmpty(context.Items);
            Assert.Equal(1, context.Items.Count);

            foreach (var itemValue in context.Items.Values)
            {
                var bufferDictionary =
                    itemValue as Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer>;

                Assert.NotNull(bufferDictionary);

                Assert.Single(bufferDictionary);

                foreach (var bufferValue in bufferDictionary.Values)
                {
                    Assert.NotNull(bufferValue);

                    Assert.Equal(9, bufferValue.Count);
                }
            }

            httpModule.OnEndRequest(context);

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
        public void TestSingleDebugTargetWithNullContext()
        {
            var logFactory = RegisterSingleDebugTarget();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only"));

            HttpContext context = null;

            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            target.HttpContextAccessor = new FakeHttpContextAccessor(null);

            // Act
            var httpModule = new NLogHttpModule();
            httpModule.OnBeginRequest(context);

            ILogger logger = logFactory.GetCurrentClassLogger();

            for (int i = 0; i < 10; i++)
            {
                logger.Debug(i.ToString());
            }

            httpModule.OnEndRequest(context);

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
        public void TestSingleMemoryTargetWithNullContext()
        {
            var logFactory = RegisterSingleMemoryTarget();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only"));

            HttpContext context = null;

            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("only");
            target.HttpContextAccessor = new FakeHttpContextAccessor(null);

            // Act
            var httpModule = new NLogHttpModule();
            httpModule.OnBeginRequest(context);

            ILogger logger = logFactory.GetCurrentClassLogger();

            for (int i = 0; i < 10; i++)
            {
                logger.Debug(i.ToString());
            }

            httpModule.OnEndRequest(context);

            var wrappedTarget = target.WrappedTarget;
            var memoryTarget = wrappedTarget as MemoryTarget;

            Assert.NotNull(memoryTarget);

            Assert.NotNull(memoryTarget.Logs);

            Assert.NotEmpty(memoryTarget.Logs);

            Assert.Equal(10, memoryTarget.Logs.Count);

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
        public void TestMutipleMemoryTargetsWithNullContext()
        {
            var logFactory = RegisterMultipleMemoryTargets();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first"));
            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second"));
            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third"));

            HttpContext context = null;

            var firstTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first");
            firstTarget.HttpContextAccessor = new FakeHttpContextAccessor(null);

            var secondTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second");
            secondTarget.HttpContextAccessor = new FakeHttpContextAccessor(null);

            var thirdTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third");
            thirdTarget.HttpContextAccessor = new FakeHttpContextAccessor(null);

            // Act
            var httpModule = new NLogHttpModule();
            httpModule.OnBeginRequest(context);

            ILogger logger = logFactory.GetCurrentClassLogger();

            for (int i = 0; i < 10; i++)
            {
                logger.Debug(i.ToString());
            }

            httpModule.OnEndRequest(context);

            TestMemoryTargetForMultipleCaseNullContext(firstTarget);
            TestMemoryTargetForMultipleCaseNullContext(secondTarget);
            TestMemoryTargetForMultipleCaseNullContext(thirdTarget);

            LogManager.Shutdown();
        }

        [Fact]
        public void TestMutipleMemoryTargets()
        {
            var logFactory = RegisterMultipleMemoryTargets();

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first"));
            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second"));
            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third"));

            var context = SetUpFakeHttpContext();

            var firstTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first");
            firstTarget.HttpContextAccessor = new FakeHttpContextAccessor(new HttpContextWrapper(context));

            var secondTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second");
            secondTarget.HttpContextAccessor = new FakeHttpContextAccessor(new HttpContextWrapper(context));

            var thirdTarget = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third");
            thirdTarget.HttpContextAccessor = new FakeHttpContextAccessor(new HttpContextWrapper(context));

            // Act
            var httpModule = new NLogHttpModule();
            httpModule.OnBeginRequest(context);

            ILogger logger = logFactory.GetCurrentClassLogger();

            for (int i = 0; i < 10; i++)
            {
                logger.Debug(i.ToString());
            }

            Assert.NotNull(context.Items);
            Assert.NotEmpty(context.Items);
            Assert.Equal(1, context.Items.Count);

            foreach (var itemValue in context.Items.Values)
            {
                var bufferDictionary =
                    itemValue as Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer>;

                Assert.NotNull(bufferDictionary);

                Assert.Equal(3,bufferDictionary.Count);

                foreach (var bufferValue in bufferDictionary.Values)
                {
                    Assert.NotNull(bufferValue);

                    Assert.Equal(9, bufferValue.Count);
                }
            }

            httpModule.OnEndRequest(context);

            TestMemoryTargetForMultipleCase(firstTarget);
            TestMemoryTargetForMultipleCase(secondTarget);
            TestMemoryTargetForMultipleCase(thirdTarget);

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
    }
}
