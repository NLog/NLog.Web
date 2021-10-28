using System;
using System.Collections.Generic;
using System.Text;
using NLog.Config;
using NLog.Layouts;

namespace NLog.Web.Layouts
{
    /// <summary>
    /// A specialized layout that renders W3C Extended Log Format (ELF).
    /// </summary>
    [Layout("W3CExtendedLogLayout")]
    [ThreadAgnostic]
    [AppDomainFixedOutput]
    public class W3CExtendedLogLayout : LayoutWithHeaderAndFooter
    {
        /// <summary>
        /// Gets the array of extended fields to include
        /// </summary>
        /// <docgen category='W3C Options' order='10' />
        [ArrayParameter(typeof(W3CExtendedLogField), "field")]
        public IList<W3CExtendedLogField> Fields { get; } = new List<W3CExtendedLogField>();

        /// <summary>
        /// Gets the array of directive headers to include
        /// </summary>
        /// <docgen category='W3C Options' order='10' />
        [ArrayParameter(typeof(W3CExtendedLogField), "directive")]
        public IList<W3CExtendedLogField> Directives { get; } = new List<W3CExtendedLogField>();

        /// <summary>
        /// Newline to append after each directive header
        /// </summary>
        public NLog.Targets.LineEndingMode LineEnding { get; set; } = NLog.Targets.LineEndingMode.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="W3CExtendedLogLayout"/> class.
        /// </summary>
        public W3CExtendedLogLayout()
        {
            Layout = this;
            Header = new W3CExtendedHeaderLayout(this);
            Footer = null;
        }

        /// <inheritdoc />
        protected override void InitializeLayout()
        {
            if (Fields.Count == 0)
            {
                Fields.Add(new W3CExtendedLogField() { Name = "date",           Layout = "${date:universalTime=true:format=yyyy-MM-dd}" });
                Fields.Add(new W3CExtendedLogField() { Name = "time",           Layout = @"${date:universalTime=true:format=HH\:mm\:ss}" });
                Fields.Add(new W3CExtendedLogField() { Name = "c-ip",           Layout = "${aspnet-request-ip}" });
                Fields.Add(new W3CExtendedLogField() { Name = "cs-username",    Layout = "${aspnet-user-identity}" });
                Fields.Add(new W3CExtendedLogField() { Name = "s-computername", Layout = "${machinename}" });
                Fields.Add(new W3CExtendedLogField() { Name = "cs-method",      Layout = "${aspnet-request-method}" });
                Fields.Add(new W3CExtendedLogField() { Name = "cs-uri-stem",    Layout = "${aspnet-request-url:includeScheme=false:includeHost=false}" });
                Fields.Add(new W3CExtendedLogField() { Name = "cs-uri-query",   Layout = "${aspnet-request-url:includeScheme=false:includeHost=false:includePath=false:includeQueryString=true}" });
                Fields.Add(new W3CExtendedLogField() { Name = "sc-status",      Layout = "${aspnet-response-statuscode}" });
                Fields.Add(new W3CExtendedLogField() { Name = "sc-bytes",       Layout = "${aspnet-response-contentlength}" });
                Fields.Add(new W3CExtendedLogField() { Name = "cs-bytes",       Layout = "${aspnet-request-contentlength}" });
                Fields.Add(new W3CExtendedLogField() { Name = "time-taken",     Layout = "${aspnet-request-duration}" });
                Fields.Add(new W3CExtendedLogField() { Name = "cs-host",        Layout = "${aspnet-request-url:includeScheme=false:includeHost=true:includePath=false}" });
                Fields.Add(new W3CExtendedLogField() { Name = "cs(User-Agent)", Layout = "${aspnet-request-useragent}" });
            }

            if (Directives.Count == 0)
            {
                Directives.Add(new W3CExtendedLogField() { Name = "Software",   Layout = "Microsoft Internet Information Server" });
                Directives.Add(new W3CExtendedLogField() { Name = "Version",    Layout = "1.0" });
                Directives.Add(new W3CExtendedLogField() { Name = "Start-Date",  Layout = @"${date:universalTime=true:format=yyyy-MM-dd HH\:mm\:ss}" });
            }

            base.InitializeLayout();
        }

        private void RenderHeader(LogEventInfo logEvent, StringBuilder sb)
        {
            for (int i = 0; i < Directives.Count; ++i)
            {
                var directive = Directives[i];
                var directiveValue = directive.Layout?.Render(logEvent);
                if (!string.IsNullOrEmpty(directiveValue))
                {
                    sb.Append('#');
                    sb.Append(directive.Name.Replace(' ', '-'));
                    sb.Append(": ");
                    sb.Append(directiveValue);
                    sb.Append(LineEnding.NewLineCharacters);
                }
            }

            sb.Append("#Fields: ");
            string fieldSeparator = string.Empty;
            for (int i = 0; i < Fields.Count; i++)
            {
                var field = Fields[i];
                sb.Append(fieldSeparator);
                sb.Append(field.Name.Replace(' ', '-'));
                fieldSeparator = " ";
            }
        }

        /// <inheritdoc />
        protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
        {
            for (int i = 0; i < Fields.Count; ++i)
            {
                if (i > 0)
                    target.Append(' ');

                var orgLength = target.Length;
                Fields[i].Layout.Render(logEvent, target);
                if (target.Length == orgLength)
                {
                    target.Append('-');
                    continue;
                }

                for (int j = orgLength; j < target.Length; ++j)
                {
                    if (target[j] == ' ')
                        target[j] = '-';
                }
            }
        }

        [ThreadAgnostic]
        [AppDomainFixedOutput]
        private class W3CExtendedHeaderLayout : Layout
        {
            private readonly W3CExtendedLogLayout _parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="W3CExtendedHeaderLayout"/> class.
            /// </summary>
            public W3CExtendedHeaderLayout(W3CExtendedLogLayout parent)
            {
                _parent = parent;
            }

            /// <inheritdoc />
            public override void Precalculate(LogEventInfo logEvent)
            {
                // Precalculation and caching is not needed
            }

            /// <inheritdoc />
            protected override string GetFormattedMessage(LogEventInfo logEvent)
            {
                var stringBuilder = new StringBuilder();
                _parent.RenderHeader(logEvent, stringBuilder);
                return stringBuilder.ToString();
            }

            /// <inheritdoc />
            protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
            {
                _parent.RenderHeader(logEvent, target);
            }
        }
    }
}
