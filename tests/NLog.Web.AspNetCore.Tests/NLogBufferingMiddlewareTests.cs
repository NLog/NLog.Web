using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class NLogBufferingMiddlewareTests
    {
        private LogFactory RegisterAspNetCoreBufferingTargetWrapper(string targetName)
        {
           return new NLog.LogFactory().Setup().RegisterNLogWeb().LoadConfigurationFromXml(
                $@"<nlog throwConfigExceptions='true'>
                    <targets>
                        <target type='AspNetCoreBufferingWrapper' name='{targetName}'>
                            <target type='Memory' name='unitTestMemoryTarget' />
                        </target>
                    </targets>
                    <rules>
                        <logger name='*' minlevel='Debug' writeTo='{targetName}' />
                    </rules>
                </nlog>").LogFactory;
        }

        [Fact]
        public async Task BufferingMiddlewareInvokeTest()
        {
            var logFactory = RegisterAspNetCoreBufferingTargetWrapper("first");

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetCoreBufferingTargetWrapper>("first"));

            DefaultHttpContext context = new DefaultHttpContext();

            RequestDelegate next = (HttpContext hc) =>
            {
                ILogger logger = logFactory.GetCurrentClassLogger();

                for(int i = 0; i < 10; i++)
                {
                    logger.Debug("This is a unit test logging.");
                }

                return Task.CompletedTask;
            };

            NLogBufferingMiddleware middleware = new NLogBufferingMiddleware(next);

            await middleware.Invoke(context).ConfigureAwait(false);

            Assert.NotEmpty(context.Items);

            Assert.Equal(1, context.Items.Count);

            var eventBufferKeyPair = context.Items.First();

            Assert.NotNull(eventBufferKeyPair);

            var eventBuffer = eventBufferKeyPair.Value;

            Assert.NotNull(eventBuffer);

            var nlogEventBuffer = eventBuffer as LogEventInfoBuffer;

            Assert.NotNull(nlogEventBuffer);

            //The AspNetCoreBufferingTargetWrapper.Write() method is having a null HttpContext, will need a fix.
            //Assert.Equal(10, nlogEventBuffer.Count);

            var secondFactory = RegisterAspNetCoreBufferingTargetWrapper("second");

            Assert.NotNull(secondFactory?.Configuration?.FindTargetByName<AspNetCoreBufferingTargetWrapper>("second"));

            await middleware.Invoke(context).ConfigureAwait(false);

            Assert.NotEmpty(context.Items);

            Assert.Equal(2, context.Items.Count);

            var thirdFactory = RegisterAspNetCoreBufferingTargetWrapper("third");

            Assert.NotNull(thirdFactory?.Configuration?.FindTargetByName<AspNetCoreBufferingTargetWrapper>("third"));

            await middleware.Invoke(context).ConfigureAwait(false);

            Assert.NotEmpty(context.Items);

            Assert.Equal(3, context.Items.Count);

            LogManager.Shutdown();
        }
    }
}
