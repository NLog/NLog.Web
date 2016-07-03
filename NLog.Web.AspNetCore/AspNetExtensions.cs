using Microsoft.AspNetCore.Builder;
using NLog.Web.Internal;

namespace NLog.Web
{
    /// <summary>
    /// Helpers for ASP.NET
    /// </summary>
    public static class AspNetExtensions
    {
        /// <summary>
        /// Enable NLog Web for ASP.NET 5.
        /// </summary>
        /// <param name="app"></param>

        public static void AddNLogWeb(this IApplicationBuilder app)
        {
            ServiceLocator.ServiceProvider = app.ApplicationServices;            
        }
    }
}
