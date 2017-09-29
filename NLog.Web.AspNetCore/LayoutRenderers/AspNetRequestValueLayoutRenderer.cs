using System;
using System.Text;
#if !ASP_NET_CORE
using NLog.Common;
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request variable.
    /// </summary>
    /// <remarks>
    /// Use this layout renderer to insert the value of the specified parameter of the
    /// ASP.NET Request object.
    /// </remarks>
    /// <example>
    /// <para>Example usage of ${aspnet-request}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request:item=v}
    /// ${aspnet-request:querystring=v}
    /// ${aspnet-request:form=v}
    /// ${aspnet-request:cookie=v}
    /// ${aspnet-request:header=h}
    /// ${aspnet-request:serverVariable=v}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request")]
    public class AspNetRequestValueLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Gets or sets the item name. The QueryString, Form, Cookies, or ServerVariables collection variables having the specified name are rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultParameter]
        public string Item { get; set; }

        /// <summary>
        /// Gets or sets the QueryString variable to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the form variable to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Form { get; set; }

        /// <summary>
        /// Gets or sets the cookie to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Cookie { get; set; }

#if !ASP_NET_CORE

        //missing in .NET Core (RC2)

        /// <summary>
        /// Gets or sets the ServerVariables item to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string ServerVariable { get; set; }

#endif

        /// <summary>
        /// Gets or sets the Headers item to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Header { get; set; }

        /// <summary>
        /// Renders the specified ASP.NET Request variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext?.TryGetRequest();
            if (httpRequest == null)
            {
                return;
            }

            if (this.QueryString != null)
            {
#if !ASP_NET_CORE
                if (httpRequest.QueryString != null)
                {
                    builder.Append(httpRequest.QueryString[this.QueryString]);
                }
#else
                if (httpRequest.Query != null)
                {
                    builder.Append(httpRequest.Query[this.QueryString]);
                }
#endif
            }
            else if (this.Form != null && httpRequest.Form != null)
            {
                builder.Append(httpRequest.Form[this.Form]);
            }
            else if (this.Cookie != null && httpRequest.Cookies != null)
            {
#if !ASP_NET_CORE
                var cookie = httpRequest.Cookies[this.Cookie];

                if (cookie != null)
                {
                    builder.Append(cookie.Value);
                }
#else
                var cookie = httpRequest.Cookies[this.Cookie];
                builder.Append(cookie);
#endif

            }
#if !ASP_NET_CORE
            else if (this.ServerVariable != null && httpRequest.ServerVariables != null)
            {
                builder.Append(httpRequest.ServerVariables[this.ServerVariable]);
            }
#endif
            else if (this.Header != null && httpRequest.Headers != null)
            {
                string header = httpRequest.Headers[this.Header];

                if (header != null)
                {
                    builder.Append(header);
                }
            }
            else if (this.Item != null)
            {
#if !ASP_NET_CORE
                builder.Append(httpRequest[this.Item]);
#else
                builder.Append(httpRequest.HttpContext.Items[this.Item]);
#endif
            }
        }
    }
}