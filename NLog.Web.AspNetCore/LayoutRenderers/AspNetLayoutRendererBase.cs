using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Enums;
#if NETSTANDARD_1plus
using NLog.Web.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Base class for ASP.NET layout renderers.
    /// </summary>
    public abstract class AspNetLayoutRendererBase : LayoutRenderer
    {
        /// <summary>
        /// Initializes the <see cref="AspNetLayoutRendererBase"/>.
        /// </summary>
        protected AspNetLayoutRendererBase()
        {
#if !NETSTANDARD_1plus
            HttpContextAccessor = new DefaultHttpContextAccessor();
#endif
        }


#if NETSTANDARD_1plus

        /// <summary>
        /// Context for DI
        /// </summary>
        private IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Provides access to the current request HttpContext.
        /// </summary>
        /// <returns>HttpContextAccessor or <c>null</c></returns>
        public IHttpContextAccessor HttpContextAccessor
        {
            get { return _httpContextAccessor ?? ServiceLocator.ServiceProvider?.GetService<IHttpContextAccessor>(); }
            set { _httpContextAccessor = value; }
        }

#else
        /// <summary>
        /// Provides access to the current request HttpContext.
        /// </summary>
        public IHttpContextAccessor HttpContextAccessor { get; set; }

#endif

        /// <summary>
        /// Validates that the HttpContext is available and delegates append to subclasses.<see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (HttpContextAccessor?.HttpContext == null)
                return;

            DoAppend(builder, logEvent);
        }

        /// <summary>
        /// Implemented by subclasses to render request information and append it to the specified <see cref="StringBuilder" />.
        /// 
        /// Won't be called if <see cref="HttpContextAccessor"/> of <see cref="IHttpContextAccessor.HttpContext"/> is <c>null</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected abstract void DoAppend(StringBuilder builder, LogEventInfo logEvent);

        /// <summary>
        /// Serialize multiple values
        /// </summary>
        /// <param name="values">The values with key and value.</param>
        /// <param name="builder">Add to this builder.</param>
        /// <param name="outputFormat">The output format - JSON or flat.</param>
        protected void SerializeValues(IEnumerable<KeyValuePair<string, string>> values, StringBuilder builder, AspNetRequestLayoutOutputFormat outputFormat)
        {
            var firstItem = true;
            switch (outputFormat)
            {
                case AspNetRequestLayoutOutputFormat.Flat:

                    foreach (var kpv in values)
                    {
                        var key = kpv.Key;
                        var value = kpv.Value;

                        if (!firstItem)
                        {
                            builder.Append(',');
                        }
                        firstItem = false;
                        builder.Append(key);
                        builder.Append('=');
                        builder.Append(value);
                    }


                    break;
                case AspNetRequestLayoutOutputFormat.Json:


                    var valueList = values.ToList();

                    if (valueList.Count > 0)
                    {
                        var addArray = valueList.Count > 0;

                        if (addArray)
                        {
                            builder.Append('[');
                        }

                        foreach (var kpv in valueList)
                        {
                            var key = kpv.Key;
                            var value = kpv.Value;
                            if (!firstItem)
                            {
                                builder.Append(',');
                            }
                            firstItem = false;

                            //quoted key
                            builder.Append('{');
                            builder.Append('"');
                            //todo escape quotes
                            builder.Append(key);
                            builder.Append('"');

                            builder.Append(':');

                            //quoted value;
                            builder.Append('"');
                            //todo escape quotes
                            builder.Append(value);
                            builder.Append('"');
                            builder.Append('}');
                        }
                        if (addArray)
                        {
                            builder.Append(']');
                        }
                    }
                    break;
            }
        }
    }
}
