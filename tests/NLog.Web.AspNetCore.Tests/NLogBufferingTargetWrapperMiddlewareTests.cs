using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Fluent;
using NLog.Web.Internal;
using NLog.Web.LayoutRenderers;
using NLog.Web.Targets.Wrappers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace NLog.Web.Tests
{
    /// <summary>
    /// The unit tests must be executed serially
    /// </summary>
    public class NLogBufferingTargetWrapperMiddlewareTests
    {
        private static LogFactory RegisterAspNetCoreBufferingTargetWrapper(string targetName)
        {
            LogManager.LogFactory.Setup().RegisterNLogWeb().LoadConfigurationFromXml(
                $@"<nlog throwConfigExceptions='true'>
                    <targets>
                        <target type='AspNetBufferingWrapper' name='{targetName}'>
                            <target type='Memory' name='unitTestMemoryTarget' />
                        </target>
                    </targets>
                    <rules>
                        <logger name='*' minlevel='Debug' writeTo='{targetName}' />
                    </rules>
                </nlog>");
            return LogManager.LogFactory;
        }

        [Fact]
        public async Task BufferingMiddlewareInvokeTest()
        {
            var logFactory = RegisterAspNetCoreBufferingTargetWrapper("first");

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first"));

            DefaultHttpContext context = new DefaultHttpContext();

            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first");
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            RequestDelegate next = (HttpContext hc) =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for(int i = 0; i < 10; i++)
                {
                    logger.Debug("This is a unit test logging.");
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

            var nlogEventBuffer = eventBuffer as Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer>;

            Assert.NotNull(nlogEventBuffer);

            var secondFactory = RegisterAspNetCoreBufferingTargetWrapper("second");

            Assert.NotNull(secondFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second"));

            target = secondFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second");
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            await middleware.Invoke(context).ConfigureAwait(false);

            Assert.NotEmpty(context.Items);

            var thirdFactory = RegisterAspNetCoreBufferingTargetWrapper("third");

            Assert.NotNull(thirdFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third"));

            target = thirdFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third");
            target.HttpContextAccessor = new FakeHttpContextAccessor(context);

            await middleware.Invoke(context).ConfigureAwait(false);

            Assert.NotEmpty(context.Items);

            LogManager.Shutdown();
        }

        [Fact]
        public async Task BufferingMiddlewareInvokeNullContextTest()
        {
            var logFactory = RegisterAspNetCoreBufferingTargetWrapper("first");

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first"));
            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first");
            target.HttpContextAccessor = new FakeHttpContextAccessor(null);

            // This should not cause exception even if null
            DefaultHttpContext context = null;

            RequestDelegate next = (HttpContext hc) =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for (int i = 0; i < 10; i++)
                {
                    logger.Debug("This is a unit test logging.");
                }

                return Task.CompletedTask;
            };

            NLogBufferingTargetWrapperMiddleware middleware = new NLogBufferingTargetWrapperMiddleware(next);

            await middleware.Invoke(context).ConfigureAwait(false);

            var secondFactory = RegisterAspNetCoreBufferingTargetWrapper("second");

            Assert.NotNull(secondFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second"));

            target = secondFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("second");
            target.HttpContextAccessor = new FakeHttpContextAccessor(null);

            await middleware.Invoke(context).ConfigureAwait(false);

            var thirdFactory = RegisterAspNetCoreBufferingTargetWrapper("third");

            Assert.NotNull(thirdFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third"));

            target = thirdFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("third");
            target.HttpContextAccessor = new FakeHttpContextAccessor(null);

            await middleware.Invoke(context).ConfigureAwait(false);

            LogManager.Shutdown();
        }
    }
}
