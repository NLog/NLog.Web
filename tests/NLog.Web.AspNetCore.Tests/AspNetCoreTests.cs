#if ASP_NET_CORE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace NLog.Web.Tests
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

            LogManager.Setup().RegisterNLogWeb(serviceProvider: webhost.Services).LoadConfiguration(configuration);

            var logger = loggerFact.CreateLogger("logger1");

            logger.LogError("error1");

            var logged = target.Logs;

            Assert.Single(logged);
            Assert.Equal("logger1|error1", logged.First());
        }

#if !ASP_NET_CORE1 && !ASP_NET_CORE2
        [Fact]
        public void LoadConfigurationFromAppSettingsShouldLogTest()
        {
            var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), nameof(AspNetCoreTests), Guid.NewGuid().ToString()).Replace("\\", "/");
            var appSettings = System.IO.Path.Combine(tempPath, "appsettings.json");

            try
            {
                // Arrange
                System.IO.Directory.CreateDirectory(tempPath);
                System.IO.File.AppendAllText(appSettings, @"{
                  ""basepath"": """ + tempPath + @""",
                  ""NLog"": {
                    ""throwConfigExceptions"": true,
                    ""targets"": {
                        ""logfile"": {
                            ""type"": ""File"",
                            ""fileName"": ""${configsetting:basepath}/hello.txt"",
                            ""layout"": ""${message}""
                        }
                    },
                    ""rules"": [
                      {
                        ""logger"": ""*"",
                        ""minLevel"": ""Debug"",
                        ""writeTo"": ""logfile""
                      }
                    ]
                  }
                }");

                // Act
                var logFactory = new LogFactory();
                var logger = logFactory.Setup().LoadConfigurationFromAppSettings(basePath: tempPath).GetCurrentClassLogger();
                logger.Info("Hello World");

                // Assert
                var fileOutput = System.IO.File.ReadAllText(System.IO.Path.Combine(tempPath, "hello.txt"));
                Assert.Contains("Hello World", fileOutput);
            }
            finally
            {
                if (System.IO.Directory.Exists(tempPath))
                {
                    System.IO.Directory.Delete(tempPath, true);
                }
            }
        }

        [Fact]
        public void LoadConfigurationFromAppSettingsShouldLogTest2()
        {
            var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), nameof(AspNetCoreTests), Guid.NewGuid().ToString()).Replace("\\", "/");
            var appSettings = System.IO.Path.Combine(tempPath, "appsettings.json");

            try
            {
                // Arrange
                System.IO.Directory.CreateDirectory(tempPath);
                System.IO.File.AppendAllText(appSettings, @"{
                  ""basepath"": """ + tempPath + @"""
                }");

                System.IO.File.AppendAllText(System.IO.Path.Combine(tempPath, "nlog.config"), @"<nlog>
                    <targets>
                        <target type=""file"" name=""logfile"" layout=""${message}"" fileName=""${configsetting:basepath}/hello.txt"" />
                    </targets>
                    <rules>
                        <logger name=""*"" minLevel=""Debug"" writeTo=""logfile"" />
                    </rules>
                </nlog>");

                // Act
                var logFactory = new LogFactory();
                var logger = logFactory.Setup().LoadConfigurationFromAppSettings(basePath: tempPath).GetCurrentClassLogger();
                logger.Info("Hello World");

                // Assert
                var fileOutput = System.IO.File.ReadAllText(System.IO.Path.Combine(tempPath, "hello.txt"));
                Assert.Contains("Hello World", fileOutput);
            }
            finally
            {
                if (System.IO.Directory.Exists(tempPath))
                {
                    System.IO.Directory.Delete(tempPath, true);
                }
            }
        }
#endif

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
                //clear so next time it's rebuild
                ConfigurationItemFactory.Default = null;

                var webhost = CreateWebHost();

                var configuration = CreateConfigWithMemoryTarget(out var target, "${logger}|${message}|${aspnet-item:key1}");

                LogManager.Setup().RegisterNLogWeb(serviceProvider: webhost.Services).LoadConfiguration(configuration);

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

#if !ASP_NET_CORE1
        [Fact]
        public void SkipRegisterHttpContext()
        {
            var webhost = CreateWebHost(new NLogAspNetCoreOptions { RegisterHttpContextAccessor = false });
            Assert.Null(webhost.Services.GetService<IHttpContextAccessor>());
        }
#endif

        /// <summary>
        /// Create webhost with UseNlog
        /// </summary>
        /// <returns></returns>
        private static IWebHost CreateWebHost(NLogAspNetCoreOptions options = null)
        {
#if !ASP_NET_CORE1
            var webhost =
                Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
                    .Configure(c => c.New()) //.New needed, otherwise:
                                             // Unhandled Exception: System.ArgumentException: A valid non-empty application name must be provided.
                                             // Parameter name: applicationName
                    .UseNLog(options) //use NLog for ILoggers and pass httpcontext
                    .Build();
            return webhost;
#else
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();
            return host;
#endif
        }

        private static ILoggerFactory GetLoggerFactory(IWebHost webhost)
        {
            return webhost.Services.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
        }

#if !ASP_NET_CORE2 && !ASP_NET_CORE3
        public class Startup
        {
            public Startup()
            {
            }

            public void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                app.AddNLogWeb();
                loggerFactory.AddNLog();
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            }
        }
#endif
    }
}

#endif