// 
// Copyright (c) 2004-2021 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

namespace NLog.Web.Internal
{
    /// <summary>
    /// Provides logging interface and utility functions.
    /// </summary>
    internal static class AssemblyExtensionTypes
    {
        public static void RegisterTypes(this NLog.Config.ISetupExtensionsBuilder setupBuilder)
        {
            #pragma warning disable CS0618 // Type or member is obsolete
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetAppBasePathLayoutRenderer>("aspnet-appbasepath");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetApplicationLayoutRenderer>("aspnet-application");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetHttpContextItemLayoutRenderer>("aspnet-httpcontext-item");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetHttpContextItemLayoutRenderer>("aspnet-item");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetMvcActionLayoutRenderer>("aspnet-mvc-action");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetMvcControllerLayoutRenderer>("aspnet-mvc-controller");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestQueryStringLayoutRenderer>("aspnet-request-querystring");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestClientCertificateLayoutRenderer>("aspnet-request-client-certificate");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestContentLengthLayoutRenderer>("aspnet-request-contentlength");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestContentTypeLayoutRenderer>("aspnet-request-contenttype");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestCookieLayoutRenderer>("aspnet-request-cookie");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestDurationLayoutRenderer>("aspnet-request-duration");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestFormLayoutRenderer>("aspnet-request-form");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestHasPostedBodyLayoutRenderer>("aspnet-request-has-posted-body");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestHeadersLayoutRenderer>("aspnet-request-headers");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestHostLayoutRenderer>("aspnet-request-host");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestHttpMethodLayoutRenderer>("aspnet-request-method");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestIpLayoutRenderer>("aspnet-request-ip");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestIsWebSocketLayoutRenderer>("aspnet-request-is-web-socket");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestLocalIpLayoutRenderer>("aspnet-request-local-ip");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestLocalPortLayoutRenderer>("aspnet-request-local-port");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestPostedBodyLayoutRenderer>("aspnet-request-posted-body");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestReferrerLayoutRenderer>("aspnet-request-referrer");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestRemotePortLayoutRenderer>("aspnet-request-remote-port");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestRouteParametersRenderer>("aspnet-request-routeparameters");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestServerVariableLayoutRenderer>("aspnet-request-servervariable");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestUrlLayoutRenderer>("aspnet-request-url");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestUserAgentLayoutRenderer>("aspnet-request-useragent");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestValueLayoutRenderer>("aspnet-request");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetRequestWebSocketRequestedProtocolsLayoutRenderer>("aspnet-request-web-socket-requested-protocols");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetResponseContentLengthLayoutRenderer>("aspnet-response-contentlength");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetResponseContentTypeLayoutRenderer>("aspnet-response-contenttype");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetResponseCookieLayoutRenderer>("aspnet-response-cookie");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetResponseHasStartedLayoutRenderer>("aspnet-response-has-started");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetResponseHeadersLayoutRenderer>("aspnet-response-headers");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetResponseStatusCodeRenderer>("aspnet-response-statuscode");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetSessionIdLayoutRenderer>("aspnet-sessionid");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetSessionItemLayoutRenderer>("aspnet-session-item");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetSessionItemLayoutRenderer>("aspnet-session");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetTraceIdentifierLayoutRenderer>("aspnet-traceidentifier");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetUserAuthTypeLayoutRenderer>("aspnet-user-authtype");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetUserIdentityLayoutRenderer>("aspnet-user-identity");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetUserIsAuthenticatedLayoutRenderer>("aspnet-user-isAuthenticated");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetWebRootPathLayoutRenderer>("aspnet-webrootpath");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AssemblyVersionLayoutRenderer>("assembly-version");
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.IISSiteNameLayoutRenderer>("iis-site-name");
            setupBuilder.RegisterLayout<NLog.Web.Layouts.W3CExtendedLogLayout>("W3CExtendedLogLayout");
            setupBuilder.RegisterTarget<NLog.Web.Targets.AspNetTraceTarget>("AspNetTrace");
            setupBuilder.RegisterTarget<NLog.Web.Targets.Wrappers.AspNetBufferingTargetWrapper>("AspNetBufferingWrapper");

#if !NET35
            setupBuilder.RegisterLayoutRenderer<NLog.Web.LayoutRenderers.AspNetUserClaimLayoutRenderer>("aspnet-user-claim");
#endif

            #pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}