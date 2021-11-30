using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Config;

namespace NLog.Web
{
    /// <summary>
    /// Extension methods to setup LogFactory options
    /// </summary>
    public static class SetupBuilderExtensions
    {
        /// <summary>
        /// Register the NLog.Web LayoutRenderers before loading NLog config
        /// </summary>
        public static ISetupBuilder RegisterNLogWeb(this ISetupBuilder setupBuilder)
        {
            setupBuilder.SetupExtensions(e => e.RegisterNLogWeb());
            return setupBuilder;
        }
    }
}
