using System.Collections.Generic;
using System.Linq; 
using System.Web;
using NLog.Web.Targets.Wrappers;
using NLog.Web.Tests.LayoutRenderers;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogBufferingTargetWrapperModuleTests : TestInvolvingAspNetHttpContext
    {
        private static LogFactory RegisterAspNetBufferingTargetWrapper(string targetName)
        {
            LogManager.LogFactory.Setup().RegisterNLogWeb().LoadConfigurationFromXml(
                $@"<nlog throwConfigExceptions='true'>
                    <targets>
                        <target 
                            type='AspNetBufferingWrapper' 
                            name='{targetName}' 
                            bufferGrowLimit='9' 
                            bufferSize='7' 
                            growBufferAsNeeded='true'>
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
        public void SuccessTest()
        {
            // Arrange
            var httpContext = SetUpFakeHttpContext();

            var logFactory = RegisterAspNetBufferingTargetWrapper("first");

            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first"));

            var target = logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("first");
            target.HttpContextAccessor = new FakeHttpContextAccessor(new HttpContextWrapper(httpContext));

            // Act
            var httpModule = new NLogBufferingTargetWrapperModule();
            httpModule.Initialize(httpContext);

            ILogger logger = logFactory.GetCurrentClassLogger();

            for (int i = 0; i < 10; i++)
            {
                logger.Debug("This is a unit test logging.");
            }

            // Assert
            Assert.NotNull(httpContext.Items);
            Assert.NotEmpty(httpContext.Items);
            Assert.Equal(1, httpContext.Items.Count);

            foreach (var itemValue in httpContext.Items.Values)
            {
                var bufferDictionary =
                    itemValue as Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer>;
                Assert.NotNull(bufferDictionary);
                Assert.Single(bufferDictionary);

                foreach (var bufferValue in bufferDictionary.Values)
                {
                    Assert.NotNull(bufferValue);
                    Assert.Equal(9,bufferValue.Count);
                }
            }

            httpModule.Flush(httpContext);
        }
    }
}
