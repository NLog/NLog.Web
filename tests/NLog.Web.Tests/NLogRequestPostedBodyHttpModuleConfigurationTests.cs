using System;
using System.Web;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests
{
    public class NLogRequestPostedBodyHttpModuleConfigurationTests
    {
        [Fact]
        public void SetMaximumRequestSizeTest()
        {
            var config = new NLogRequestPostedBodyHttpModuleConfiguration();
            var size = new Random().Next();
            config.MaximumRequestSize = size;

            Assert.Equal(size, config.MaximumRequestSize);
        }

        [Fact]
        public void SetByteOrderMarkTest()
        {
            var config = new NLogRequestPostedBodyHttpModuleConfiguration();
            var bom = true;
            config.DetectEncodingFromByteOrderMark = bom;

            Assert.Equal(bom, config.DetectEncodingFromByteOrderMark);

            bom = false;
            config.DetectEncodingFromByteOrderMark = bom;

            Assert.Equal(bom, config.DetectEncodingFromByteOrderMark);
        }

        [Fact]
        public void GetDefault()
        {
            var config = NLogRequestPostedBodyHttpModuleConfiguration.Default;

            Assert.NotNull(config);
        }
    }
}
