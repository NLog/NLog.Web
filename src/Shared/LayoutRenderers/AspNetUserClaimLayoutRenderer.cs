#if ASP_NET_CORE || NET46_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Web.Enums;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET User ClaimType Value Lookup.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-user-claim:ClaimType=Name}</code> to render a single specific claim type
    /// <code>${aspnet-user-claim}</code> to render all claim types
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-User-Claim-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-user-claim")]
    public class AspNetUserClaimLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Key to lookup using <see cref="ClaimsIdentity.FindFirst(string)"/> with fallback to <see cref="ClaimsPrincipal.FindFirst(string)"/>
        /// </summary>
        /// <remarks>
        /// When value is prefixed with "ClaimTypes." (Remember dot) then ít will lookup in well-known claim types from <see cref="ClaimTypes"/>. Ex. ClaimsTypes.Name
        /// If this is null or empty then the Type and Value properties of all claim types are rendered
        /// </remarks>
        [DefaultParameter]
        public string ClaimType { get; set; }

        /// <summary>
        /// If this is true, then all string properties of the <see cref="Claim"/> are rendered as well the values in its Properties property.
        /// </summary>
        public bool Verbose { get; set; }

        /// <inheritdoc />
        protected override void InitializeLayoutRenderer()
        {
            if (ClaimType?.Trim().StartsWith("ClaimTypes.", StringComparison.OrdinalIgnoreCase) == true || 
                ClaimType?.Trim().StartsWith("ClaimType.",  StringComparison.OrdinalIgnoreCase) == true)
            {
                var fieldName = ClaimType.Substring(ClaimType.IndexOf('.') + 1).Trim();
                var claimTypesField = typeof(ClaimTypes).GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
                if (claimTypesField != null)
                {
                    ClaimType = claimTypesField.GetValue(null) as string ?? ClaimType.Trim();
                }
            }

            base.InitializeLayoutRenderer();
        }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            try
            {
                var claimsPrincipal = HttpContextAccessor.HttpContext.User;
                if (claimsPrincipal == null)
                {
                    InternalLogger.Debug("aspnet-user-claim - HttpContext User is null");
                    return;
                }

                if (string.IsNullOrEmpty(ClaimType))
                {
                    if (Verbose)
                    {
#if NET46
                        SerializeVerbose((claimsPrincipal as ClaimsPrincipal)?.Claims, builder, logEvent);
#else
                        SerializeVerbose(claimsPrincipal.Claims, builder, logEvent);
#endif
                    }
                    else
                    {
                        var allClaims = GetAllClaims(claimsPrincipal);
                        SerializePairs(allClaims, builder, logEvent);
                    }
                }
                else
                {
                    var claim = GetClaim(claimsPrincipal, ClaimType);
                    if (claim != null)
                    {
                        if (Verbose)
                        {
                            SerializeVerbose(new List<Claim> {claim}, builder, logEvent);
                        }
                        else
                        {
                            builder.Append(claim.Value);

                        }
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "aspnet-user-claim - HttpContext has been disposed");
            }
        }

        private void SerializeVerbose(IEnumerable<Claim> claims, StringBuilder builder, LogEventInfo logEvent)
        {
            if (claims == null)
            {
                return;
            }

            switch (OutputFormat)
            {
                case AspNetRequestLayoutOutputFormat.Flat:
                    SerializeVerboseFlat(claims, builder, logEvent);
                    break;
                case AspNetRequestLayoutOutputFormat.JsonArray:
                case AspNetRequestLayoutOutputFormat.JsonDictionary:
                    SerializeVerboseJson(claims, builder, logEvent);
                    break;
            }
        }

        private void SerializeVerboseJson(IEnumerable<Claim> claims, StringBuilder builder, LogEventInfo logEvent)
        {
            var firstItem = true;
            var includeSeparator = false;

            foreach (var claim in claims)
            {
                if (firstItem)
                {
                    if (OutputFormat == AspNetRequestLayoutOutputFormat.JsonDictionary)
                    {
                        builder.Append('{');
                    }
                    else
                    {
                        builder.Append('[');
                    }
                }
                else
                {
                    builder.Append(',');
                }

                builder.Append('{');

                includeSeparator |= AppendJsonProperty(builder, nameof(claim.Type), claim.Type, false);
                includeSeparator |= AppendJsonProperty(builder, nameof(claim.Value), claim.Value, includeSeparator);
                includeSeparator |= AppendJsonProperty(builder, nameof(claim.ValueType), claim.ValueType, includeSeparator);

                includeSeparator |= AppendJsonProperty(builder, nameof(claim.Issuer), claim.Issuer, includeSeparator);
                includeSeparator |= AppendJsonProperty(builder, nameof(claim.OriginalIssuer), claim.OriginalIssuer, includeSeparator);

                if (claim.Properties != null && claim.Properties.Count > 0)
                {
                    builder.Append(",\"");
                    builder.Append(nameof(claim.Properties));
                    builder.Append("\":");
                    SerializePairs(claim.Properties.OrderBy(entry => entry.Key).ToList(), builder, logEvent);
                }

                builder.Append('}');

                firstItem = false;
            }

            if (!firstItem)
            {
                if (OutputFormat == AspNetRequestLayoutOutputFormat.JsonDictionary)
                {
                    builder.Append('}');
                }
                else
                {
                    builder.Append(']');
                }
            }
        }

        private void SerializeVerboseFlat(IEnumerable<Claim> claims, StringBuilder builder, LogEventInfo logEvent)
        {
            var propertySeparator = GetRenderedItemSeparator(logEvent);
            var valueSeparator = GetRenderedValueSeparator(logEvent);
            var objectSeparator = GetRenderedObjectSeparator(logEvent);

            var firstObject = true;
            var includeSeparator = false;

            foreach (var claim in claims)
            {
                if (!firstObject)
                {
                    builder.Append(objectSeparator);
                }

                firstObject = false;

                includeSeparator |= AppendFlatProperty(builder, nameof(claim.Type), claim.Type, valueSeparator, "");
                includeSeparator |= AppendFlatProperty(builder, nameof(claim.Value), claim.Value, valueSeparator, includeSeparator ? propertySeparator : "");
                includeSeparator |= AppendFlatProperty(builder, nameof(claim.ValueType), claim.ValueType, valueSeparator, includeSeparator ? propertySeparator : "");

                includeSeparator |= AppendFlatProperty(builder, nameof(claim.Issuer), claim.Issuer, valueSeparator, includeSeparator ? propertySeparator : "");
                includeSeparator |= AppendFlatProperty(builder, nameof(claim.OriginalIssuer), claim.OriginalIssuer, valueSeparator, includeSeparator ? propertySeparator : "");

                if (claim.Properties != null && claim.Properties.Count > 0)
                {
                    builder.Append(propertySeparator);
                    builder.Append("Properties[");
                    SerializePairs(claim.Properties.OrderBy(entry => entry.Key).ToList(), builder, logEvent);
                    builder.Append(']');
                }
            }
        }

        /// <summary>
        /// Separator between objects, like cookies. Only used for <see cref="AspNetRequestLayoutOutputFormat.Flat" />
        /// </summary>
        /// <remarks>Render with <see cref="GetRenderedObjectSeparator" /></remarks>
        public string ObjectSeparator { get => _objectSeparatorLayout?.OriginalText; set => _objectSeparatorLayout = new SimpleLayout(value ?? ""); }
        private SimpleLayout _objectSeparatorLayout = new SimpleLayout(";");

        /// <summary>
        /// Get the rendered <see cref="ObjectSeparator" />
        /// </summary>
        private string GetRenderedObjectSeparator(LogEventInfo logEvent)
        {
            return logEvent != null ? _objectSeparatorLayout.Render(logEvent) : ObjectSeparator;
        }

        /// <summary>
        /// Append the quoted name and value separated by a colon
        /// </summary>
        private static bool AppendJsonProperty(StringBuilder builder, string name, string value, bool includePropertySeparator)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (includePropertySeparator)
                {
                    builder.Append(',');
                }
                AppendQuoted(builder, name);
                builder.Append(':');
                AppendQuoted(builder, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Append the quoted name and value separated by a value separator
        /// and ended by item separator
        /// </summary>
        private static bool AppendFlatProperty(
            StringBuilder builder,
            string name,
            string value,
            string valueSeparator,
            string itemSeparator)
        {
            if (!string.IsNullOrEmpty(value))
            {
                builder.Append(itemSeparator);
                builder.Append(name);
                builder.Append(valueSeparator);
                builder.Append(value);
                return true;
            }
            return false;
        }

#if NET46
        private static IEnumerable<KeyValuePair<string, string>> GetAllClaims(System.Security.Principal.IPrincipal claimsPrincipal)
        {
              return GetAllClaims(claimsPrincipal as ClaimsPrincipal);
        }
#endif
        private static IEnumerable<KeyValuePair<string, string>> GetAllClaims(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.Claims?.Select(claim =>
                       new KeyValuePair<string, string>(claim.Type, claim.Value)) ??
                   System.Linq.Enumerable.Empty<KeyValuePair<string, string>>();
        }
#if NET46
        private static Claim GetClaim(System.Security.Principal.IPrincipal claimsPrincipal, string claimType)
#else
        private static Claim GetClaim(ClaimsPrincipal claimsPrincipal, string claimType)
#endif
        {
            var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;    // Prioritize primary identity
            return claimsIdentity?.FindFirst(claimType)
#if ASP_NET_CORE
                        ?? claimsPrincipal.FindFirst(claimType)
#endif
                ;
        }
    }
}

#endif