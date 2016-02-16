using System;
using System.Collections.Generic;
using System.Linq;

namespace NLog.Web.Internal
{
    /// <summary>
    /// Service provider
    /// </summary>
    internal static class ServiceLocator
    {
        /// <summary>
        /// The current service provider for reading ASP.NET 5 session, request etc.
        /// </summary>
        /// <remarks>This is a anti-pattern, but for now there is not other solution to fix this.</remarks>
        public static IServiceProvider ServiceProvider { get; set; }
    }
}
