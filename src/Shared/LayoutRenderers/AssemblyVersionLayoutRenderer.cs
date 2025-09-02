using System.Reflection;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Extend NLog.LayoutRenderers.AssemblyVersionLayoutRenderer with ASP.NET Full and Core support
    /// </summary>
    /// <remarks>
    /// <code>${assembly-version}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AssemblyVersion-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("assembly-version")]
    [ThreadAgnostic]
    public class AssemblyVersionLayoutRenderer : NLog.LayoutRenderers.AssemblyVersionLayoutRenderer
    {
#if !ASP_NET_CORE
        /// <summary>
        /// Support capture of Assembly-Version from active HttpContext ApplicationInstance
        /// </summary>
        public LayoutRenderer? FixThreadAgnostic => string.IsNullOrEmpty(Name) ? _fixThreadAgnostic : null;
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
            if (assembly is null)
            {
                assembly = GetAspNetEntryAssembly();
            }
#endif
#pragma warning disable CS8603 // Possible null reference return.
            return assembly;
#pragma warning restore CS8603 // Possible null reference return.
        }

#if !ASP_NET_CORE
        private static Assembly? GetAspNetEntryAssembly()
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