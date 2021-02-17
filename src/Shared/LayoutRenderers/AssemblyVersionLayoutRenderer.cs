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
        /// Support capture of Assembly-Version from active HttpContext ApplicationInstance
        /// </summary>
        public LayoutRenderer FixThreadAgnostic => string.IsNullOrEmpty(Name) ? _fixThreadAgnostic : null;
        private readonly LayoutRenderer _fixThreadAgnostic = new ThreadIdLayoutRenderer();
#endif

        /// <inheritdoc />
        protected override void InitializeLayoutRenderer()
        {
            InternalLogger.Debug("Extending ${assembly-version} " + nameof(NLog.LayoutRenderers.AssemblyVersionLayoutRenderer) + " with NLog.Web implementation");
            base.InitializeLayoutRenderer();
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