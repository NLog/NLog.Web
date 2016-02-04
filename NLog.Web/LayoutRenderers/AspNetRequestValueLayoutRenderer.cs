using System.Text;
using System.Web;
using NLog.Config;
using NLog.LayoutRenderers;

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

        /// <summary>
        /// Gets or sets the ServerVariables item to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string ServerVariable { get; set; }

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
            HttpContextBase context = HttpContextAccessor.HttpContext;
            var httpRequest = context.Request;
            if (httpRequest == null)
            {
                return;
            }

            if (this.QueryString != null)
            {
                builder.Append(httpRequest.QueryString[this.QueryString]);
            }
            else if (this.Form != null)
            {
                builder.Append(httpRequest.Form[this.Form]);
            }
            else if (this.Cookie != null)
            {
                HttpCookie cookie = httpRequest.Cookies[this.Cookie];

                if (cookie != null)
                {
                    builder.Append(cookie.Value);
                }
            }
            else if (this.ServerVariable != null)
            {
                builder.Append(httpRequest.ServerVariables[this.ServerVariable]);
            }
            else if (this.Header != null)
            {
                string header = httpRequest.Headers[this.Header];

                if (header != null)
                {
                    builder.Append(header);
                }
            }
            else if (this.Item != null)
            {
                builder.Append(httpRequest[this.Item]);
            }
        }
    }
}