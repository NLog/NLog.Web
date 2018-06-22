using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Web.Internal;
using NLog.LayoutRenderers;

#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#endif

#if ASP_NET_CORE && NETSTANDARD1_3
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
        /// Initializes a new instance of the <see cref="AssemblyVersionLayoutRenderer" /> class.
        /// </summary>
        public AssemblyVersionLayoutRenderer()
        {
            Type = AssemblyVersionType.Assembly;
        }

        /// <summary>
        /// The (full) name of the assembly. If <c>null</c>, using:
        /// 1) for .NET Standard - the runtime framework (for example, for .NET Core 1.1 this layout renderer returned value "1.1"),
        /// 2) for .NET Full - the entry assembly.
        /// </summary>
        [DefaultParameter]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of assembly version to retrieve.
        /// </summary>
        [DefaultValue(nameof(AssemblyVersionType.Assembly))]
        public AssemblyVersionType Type { get; set; }

        /// <summary>
        /// Initializes the layout renderer.
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            _assemblyVersion = null;
            base.InitializeLayoutRenderer();
        }

        /// <summary>
        /// Closes the layout renderer.
        /// </summary>
        protected override void CloseLayoutRenderer()
        {
            _assemblyVersion = null;
            base.CloseLayoutRenderer();
        }

        private string _assemblyVersion;

        /// <summary>
        /// Renders an assembly version and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            InternalLogger.Trace("Replacing AssemblyVersion Layout Renderer ${assembly-version} with NLog.Web implementation");

            var version = _assemblyVersion ?? (_assemblyVersion = GetVersion());

            if (string.IsNullOrEmpty(version))
            {
                version = $"Could not find value for {(string.IsNullOrEmpty(Name) ? "entry" : Name)} assembly and version type {Type}";
            }

            builder.Append(version);
        }

        private string GetVersion()
        {
            var assembly = GetAssembly();
            return GetVersion(assembly);
        }

        private System.Reflection.Assembly GetAssembly()
        {
            if (string.IsNullOrEmpty(Name))
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
#if !ASP_NET_CORE
                if (assembly == null)
                {
                    assembly = GetAspNetEntryAssembly();
                }
#endif
                return assembly;
            }
            else
            {
                return System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(Name));
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

        private string GetVersion(System.Reflection.Assembly assembly)
        {
            switch (Type)
            {
                case AssemblyVersionType.File:
                    return ReflectionHelpers.GetCustomAttribute<System.Reflection.AssemblyFileVersionAttribute>(assembly)?.Version;

                case AssemblyVersionType.Informational:
                    return ReflectionHelpers.GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>(assembly)?.InformationalVersion;

                default:
                    return assembly?.GetName().Version?.ToString();
            }
        }
    }
}
