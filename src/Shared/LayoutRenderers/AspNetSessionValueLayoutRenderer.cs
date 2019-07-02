using System;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if !ASP_NET_CORE
using System.Web;
#endif

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
    [ThreadSafe]
    public class AspNetSessionValueLayoutRenderer : AspNetLayoutRendererBase
    {
#if ASP_NET_CORE
        private const string NLogRetrievingSessionValue = "NLogRetrievingSessionValue";
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetSessionValueLayoutRenderer" /> class.
        /// </summary>
        public AspNetSessionValueLayoutRenderer()
        {
            Culture = CultureInfo.CurrentUICulture;
        }

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
        /// Gets or sets the culture used for rendering.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Renders the specified ASP.NET Session value and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            if (Variable == null)
            {
                return;
            }

            var context = HttpContextAccessor.HttpContext;
            var contextSession = context.TryGetSession();
            if (contextSession == null)
                return;

#if !ASP_NET_CORE
            var value = PropertyReader.GetValue(Variable, contextSession, (session,key) => session.Count > 0 ? session[key] : null, EvaluateAsNestedProperties);
#else
            //because session.get / session.getstring also creating log messages in some cases, this could lead to stackoverflow issues. 
            //We remember on the context.Items that we are looking up a session value so we prevent stackoverflows
            if (context.Items == null || (context.Items.Count > 0 && context.Items.ContainsKey(NLogRetrievingSessionValue)))
            {
                return;
            }

            context.Items[NLogRetrievingSessionValue] = true;

            object value;
            try
            {
                value = PropertyReader.GetValue(Variable, contextSession, (session, key) => session.GetString(key), EvaluateAsNestedProperties);
            }
            finally
            {
                context.Items.Remove(NLogRetrievingSessionValue);
            }
#endif
            if (value != null)
            {
                var formatProvider = GetFormatProvider(logEvent, Culture);
                builder.Append(Convert.ToString(value, formatProvider));
            }
        }
    }
}