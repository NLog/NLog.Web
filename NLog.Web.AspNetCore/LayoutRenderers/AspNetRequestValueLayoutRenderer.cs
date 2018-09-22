using System;
using System.Text;
#if !ASP_NET_CORE
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
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
                return;

            if (QueryString != null)
            {
                AppendQueryString(builder, httpRequest);
            }
            else if (Form != null && httpRequest.Form != null)
            {
                AppendForm(builder, httpRequest);
            }
            else if (Cookie != null && httpRequest.Cookies != null)
            {
                AppendCookie(builder, httpRequest);
            }
#if !ASP_NET_CORE
            else if (ServerVariable != null && httpRequest.ServerVariables != null)
            {
                builder.Append(httpRequest.ServerVariables[ServerVariable]);
            }
#endif
            else if (Header != null && httpRequest.Headers != null)
            {
                AppendHeader(builder, httpRequest);
            }
            else if (Item != null)
            {
                AppendItem(builder, httpRequest);
            }
        }


#if !ASP_NET_CORE

        private void AppendQueryString(StringBuilder builder, HttpRequestBase httpRequest)
        {
            var queryString = httpRequest.QueryString;
            if (queryString == null)
                return;

            var query = queryString[QueryString];
            if (query != null)
                builder.Append(query);
        }

        private void AppendForm(StringBuilder builder, HttpRequestBase httpRequest)
        {
            var formValue = httpRequest.Form[Form];
            if (formValue != null)
                builder.Append(formValue);
        }

        private void AppendCookie(StringBuilder builder, HttpRequestBase httpRequest)
        {
            var cookie = httpRequest.Cookies[Cookie];
            if (cookie != null)
                builder.Append(cookie.Value);
        }

        private void AppendHeader(StringBuilder builder, HttpRequestBase httpRequest)
        {
            var header = httpRequest.Headers[Header];
            if (header != null)
                builder.Append(header);
        }

        private void AppendItem(StringBuilder builder, HttpRequestBase httpRequest)
        {
            builder.Append(httpRequest[Item]);
        }
#else

        private void AppendQueryString(StringBuilder builder, HttpRequest httpRequest)
        {
            if (httpRequest.Query?.TryGetValue(QueryString, out var queryValue) ?? false)
            {
                builder.Append(queryValue.ToString());
            }
        }

        private void AppendForm(StringBuilder builder, HttpRequest httpRequest)
        {
            if (httpRequest.Form.TryGetValue(Form, out var formValue))
            {
                builder.Append(formValue.ToString());
            }
        }

        private void AppendCookie(StringBuilder builder, HttpRequest httpRequest)
        {
            if (httpRequest.Cookies.TryGetValue(Cookie, out var cookieValue))
            {
                builder.Append(cookieValue);
            }
        }

        private void AppendHeader(StringBuilder builder, HttpRequest httpRequest)
        {
            if (httpRequest.Headers.TryGetValue(Header, out var headerValue))
            {
                builder.Append(headerValue.ToString());
            }
        }

        private void AppendItem(StringBuilder builder, HttpRequest httpRequest)
        {
            object itemValue = null;
            if (httpRequest.HttpContext.Items?.TryGetValue(Item, out itemValue) ?? false)
            {
                builder.Append(itemValue);
            }
        }
#endif
    }
}