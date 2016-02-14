using System;
using System.Globalization;
using System.Linq;
using System.Text;
#if !DNX
using System.Web;
#else
using Microsoft.AspNet.Http;
#endif
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Session variable.
    /// </summary>
    /// <remarks>
    /// Use this layout renderer to insert the value of the specified variable stored 
    /// in the ASP.NET Session dictionary.
    /// </remarks>
    /// <example>
    /// <para>You can set the value of an ASP.NET Session variable by using the following code:</para>
    /// <code lang="C#">
    /// <![CDATA[
    /// HttpContext.Current.Session["myvariable"] = 123;
    /// HttpContext.Current.Session["stringvariable"] = "aaa BBB";
    /// HttpContext.Current.Session["anothervariable"] = DateTime.Now;
    /// ]]>
    /// </code>
    /// <para>Example usage of ${aspnet-session}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-session:variable=myvariable} - produces "123"
    /// ${aspnet-session:variable=anothervariable} - produces "01/01/2006 00:00:00"
    /// ${aspnet-session:variable=anothervariable:culture=pl-PL} - produces "2006-01-01 00:00:00"
    /// ${aspnet-session:variable=myvariable:padding=5} - produces "  123"
    /// ${aspnet-session:variable=myvariable:padding=-5} - produces "123  "
    /// ${aspnet-session:variable=stringvariable:upperCase=true} - produces "AAA BBB"
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-session")]
    public class AspNetSessionValueLayoutRenderer : AspNetLayoutRendererBase
    {
#if DNX
        /// <summary>
        /// Initializes the <see cref="AspNetSessionValueLayoutRenderer"/> with the <see cref="IHttpContextAccessor"/>.
        /// </summary>
        public AspNetSessionValueLayoutRenderer(IHttpContextAccessor accessor) : base(accessor)
        {
        }
#endif
        /// <summary>
        /// Gets or sets the session variable name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultParameter]
        public string Variable { get; set; }

        /// <summary>
        /// Gets or sets whether variables with a dot are evaluated as properties or not
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public bool EvaluateAsNestedProperties { get; set; }

        /// <summary>
        /// Renders the specified ASP.NET Session value and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            if (this.Variable == null)
            {
                return;
            }

            var context = HttpContextAccessor.HttpContext;

            if (context.Session == null)
            {
                return;
            }
#if !DNX
            var value = PropertyReader.GetValue(Variable, k => context.Session[k], EvaluateAsNestedProperties);
#else
            var value = PropertyReader.GetValue(Variable, k => context.Session.GetString(k), EvaluateAsNestedProperties);
#endif

            builder.Append(Convert.ToString(value, CultureInfo.CurrentUICulture));
        }
    }
}