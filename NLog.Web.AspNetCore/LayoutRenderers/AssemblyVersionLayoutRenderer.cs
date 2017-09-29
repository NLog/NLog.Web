using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;

#if ASP_NET_CORE
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
#endif

#if ASP_NET_CORE && !NETSTANDARD2_0
using Microsoft.Extensions.PlatformAbstractions;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Overwrite the NLog.LayoutRenderers.AssemblyVersionLayoutRenderer
    /// </summary>
    [LayoutRenderer("assembly-version")]
    public class AssemblyVersionLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// The (full) name of the assembly. If <c>null</c>, using:
        /// 1) for .NET Standard - the runtime framework (for example, for .NET Core 1.1 this layout renderer returned value "1.1"),
        /// 2) for .NET Full - the entry assembly.
        /// </summary>
        [DefaultParameter]
        public string Name { get; set; }

        /// <summary>
        /// Implemented by subclasses to render request information and append it to the specified <see cref="StringBuilder" />.
        /// 
        /// Won't be called if <see cref="AspNetLayoutRendererBase.HttpContextAccessor"/> of <see cref="IHttpContextAccessor.HttpContext"/> is <c>null</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            InternalLogger.Trace("Using ${assembly-version} of NLog.Web");

            var nameNotEmpty = !string.IsNullOrEmpty(Name);
            if (nameNotEmpty)
            {
                var assembly = Assembly.Load(new AssemblyName(Name));
                if (assembly == null)
                {
                    builder.Append("Could not find assembly " + Name);
                }
                else
                {
                    builder.Append(assembly.GetName().Version.ToString());
                }
            }
            else
            {
                //try entry assembly

#if ASP_NET_CORE && !NETSTANDARD2_0
                string assemblyVersion = PlatformServices.Default.Application.RuntimeFramework.Version.ToString();

                builder.Append(assemblyVersion);

#elif NETSTANDARD2_0
                var assemblyVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                builder.Append(assemblyVersion);
#else

                var assembly = Assembly.GetEntryAssembly();

                if (assembly == null)
                {
                    assembly = GetAspNetEntryAssembly();
                }
                if (assembly == null)
                {
                    builder.Append("Could not entry assembly");
                }
                else
                {
                    builder.Append(assembly.GetName().Version.ToString());
                }
#endif

            }

        }

#if !ASP_NET_CORE
        private static Assembly GetAspNetEntryAssembly()
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
