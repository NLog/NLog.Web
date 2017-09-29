using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace NLog.Web.Internal
{
    /// <summary>
    /// Service provider
    /// </summary>
    /// <remarks>
    /// This is a anti-pattern, but it works well with NLog
    /// </remarks>
    internal static class ServiceLocator
    {
        /// <summary>
        /// The current service provider for reading ASP.NET Core session, request etc.
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }

   
    }
}
