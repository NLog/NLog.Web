using System.Reflection;
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
#if !ASP_NET_CORE
        /// <summary>
        /// Support capture of Assembly-Version from active HttpContext
        /// </summary>
        public LayoutRenderer ThreadAgnostic => string.IsNullOrEmpty(Name) ? _threadAgnostic : null;
        private readonly LayoutRenderer _threadAgnostic = new ThreadIdLayoutRenderer();
#endif

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            InternalLogger.Trace("Extending ${assembly-version} " + nameof(NLog.LayoutRenderers.AssemblyVersionLayoutRenderer) + " with NLog.Web implementation");

            base.Append(builder, logEvent);
        }

        /// <inheritdoc />
        protected override Assembly GetAssembly()
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
        private static Assembly GetAspNetEntryAssembly()
        {
            var applicatonType = System.Web.HttpContext.Current?.ApplicationInstance?.GetType();
            while (applicatonType != null && applicatonType.Namespace == "ASP")
            {
                applicatonType = applicatonType.BaseType;
            }
            return applicatonType?.Assembly;
        }
#endif
    }
}