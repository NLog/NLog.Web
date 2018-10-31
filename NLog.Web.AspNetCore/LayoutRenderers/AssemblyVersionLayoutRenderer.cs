using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Extend NLog.LayoutRenderers.AssemblyVersionLayoutRenderer with ASP.NET Full and Core support
    /// </summary>
    [LayoutRenderer("assembly-version")]
    [ThreadAgnostic]
    [ThreadSafe]
    public class AssemblyVersionLayoutRenderer : NLog.LayoutRenderers.AssemblyVersionLayoutRenderer
    {
        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            InternalLogger.Trace("Extending ${assembly-version} " + nameof(NLog.LayoutRenderers.AssemblyVersionLayoutRenderer) + " with NLog.Web implementation");

            base.Append(builder, logEvent);
        }

        /// <inheritdoc />
        protected override System.Reflection.Assembly GetAssembly()
        {
            var assembly = base.GetAssembly();

#if !ASP_NET_CORE
            if (assembly == null)
            {
                assembly = GetAspNetEntryAssembly();
            }
#endif

            return assembly;
        }

#if !ASP_NET_CORE

        private static System.Reflection.Assembly GetAspNetEntryAssembly()
        {
            if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.ApplicationInstance == null)
            {
                return null;
            }

            var type = System.Web.HttpContext.Current.ApplicationInstance.GetType();
            while (type != null && type.Namespace == "ASP")
            {
                type = type.BaseType;
            }
            return type != null ? type.Assembly : null;
        }

#endif
    }
}
