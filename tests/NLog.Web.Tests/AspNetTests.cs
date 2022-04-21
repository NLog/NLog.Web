using System;
using NLog.Web.Targets.Wrappers;
using Xunit;

namespace NLog.Web.Tests
{
    public sealed class AspNetTests
    {
        [Fact]
        public void RegisterNLogWebTest()
        {
            // Act
            var logFactory = new NLog.LogFactory().Setup().RegisterNLogWeb().LoadConfigurationFromXml(
                @"<nlog throwConfigExceptions='true'>
                    <targets>
                        <target type='AspNetBufferingWrapper' name='hello'>
                            <target type='Memory' name='hello_wrapped' />
                        </target>
                    </targets>
                    <rules>
                        <logger name='*' writeTo='hello' />
                    </rules>
                </nlog>").LogFactory;

            // Assert
            Assert.NotNull(logFactory?.Configuration?.FindTargetByName<AspNetBufferingTargetWrapper>("hello"));
        }
    }
}
