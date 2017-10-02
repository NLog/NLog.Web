#if NETCOREAPP2_0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Xunit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Web.Tests.LayoutRenderers;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;


namespace NLog.Web.AspNetCore.Tests
{
    public class AspNetCoreTests : TestBase, IDisposable
    {
        #region IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            LogManager.Configuration = null;
        }

        #endregion

        [Fact]
        public void UseNLogShouldLogTest()
        {
            var webhost = CreateWebHost();

            var loggerFact = GetLoggerFactory(webhost);

            Assert.NotNull(loggerFact);

            var configuration = CreateConfigWithMemoryTarget(out var target, "${logger}|${message}");

            LogManager.Configuration = configuration;

            var logger = loggerFact.CreateLogger("logger1");

            logger.LogError("error1");

            var logged = target.Logs;

            Assert.Single(logged);
            Assert.Equal("logger1|error1", logged.First());

        }

        private static LoggingConfiguration CreateConfigWithMemoryTarget(out MemoryTarget target, Layout layout)
        {
            var configuration = new LoggingConfiguration();
            target = new MemoryTarget("target1") { Layout = layout };

            configuration.AddRuleForAllLevels(target);
            return configuration;
        }


        [Fact]
        public void UseAspNetWithoutRegister()
        {
            try
            {


                //clear everything besides NLog
                ConfigurationItemFactory.Default.Clear();
                ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(Logger).Assembly);

                var webhost = CreateWebHost();

                var configuration = CreateConfigWithMemoryTarget(out var target, "${logger}|${message}|${aspnet-item:key1}");

                LogManager.Configuration = configuration;

                var httpContext = webhost.Services.GetService<IHttpContextAccessor>().HttpContext = new DefaultHttpContext();
                httpContext.Items["key1"] = "value1";

                var loggerFact = GetLoggerFactory(webhost);

                var logger = loggerFact.CreateLogger("logger1");

                logger.LogError("error1");

                var logged = target.Logs;

                Assert.Single(logged);
                Assert.Equal("logger1|error1|value1", logged.First());
            }
            finally
            {
                //clear so next time it's rebuild
                ConfigurationItemFactory.Default = null;
            }

        }




        [Fact]
        public void RegisterHttpContext()
        {
            var webhost = CreateWebHost();
            Assert.NotNull(webhost.Services.GetService<IHttpContextAccessor>());

        }
        [Fact]
        public void SkipRegisterHttpContext()
        {
            var webhost = CreateWebHost(new NLogAspNetCoreOptions { RegisterHttpContextAccessor = false });
            Assert.Null(webhost.Services.GetService<IHttpContextAccessor>());

        }

        /// <summary>
        /// Create webhost with UseNlog
        /// </summary>
        /// <returns></returns>
        private static IWebHost CreateWebHost(NLogAspNetCoreOptions options = null)
        {
            var webhost =
                Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
                    .Configure(c => c.New()) //.New needed, otherwise:
                                             // Unhandled Exception: System.ArgumentException: A valid non-empty application name must be provided.
                                             // Parameter name: applicationName
                    .UseNLog(options) //use NLog for ILoggers and pass httpcontext
                    .Build();
            return webhost;
        }

        private static ILoggerFactory GetLoggerFactory(IWebHost webhost)
        {
            return webhost.Services.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
        }

    }
}

#endif