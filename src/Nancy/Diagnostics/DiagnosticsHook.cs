namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Bootstrapper;
    using Cookies;
    using Cryptography;
    using Helpers;
    using ModelBinding;
    using Responses;
    using Responses.Negotiation;
    using Routing;

    public static class DiagnosticsHook
    {
        internal const string ControlPanelPrefix = "/_Nancy";

        internal const string ResourcePrefix = ControlPanelPrefix + "/Resources/";
        
        private const string PipelineKey = "__Diagnostics";

        private const string DiagsCookieName = "__ncd";
        private const int DiagnosticsSessionTimeoutMinutes = 15;

        public static void Enable(DiagnosticsConfiguration diagnosticsConfiguration, IPipelines pipelines, IEnumerable<IDiagnosticsProvider> providers, IRootPathProvider rootPathProvider, IEnumerable<ISerializer> serializers, IRequestTracing requestTracing, NancyInternalConfiguration configuration, IModelBinderLocator modelBinderLocator, IEnumerable<IResponseProcessor> responseProcessors)
        {
            var keyGenerator = new DefaultModuleKeyGenerator();
            var diagnosticsModuleCatalog = new DiagnosticsModuleCatalog(keyGenerator, providers, rootPathProvider, requestTracing, configuration, diagnosticsConfiguration);

            var diagnosticsRouteCache = new RouteCache(diagnosticsModuleCatalog, keyGenerator, new DefaultNancyContextFactory(), new DefaultRouteSegmentExtractor(), new DefaultRouteDescriptionProvider());

            var diagnosticsRouteResolver = new DefaultRouteResolver(
                diagnosticsModuleCatalog,
                new DefaultRoutePatternMatcher(),
                new DiagnosticsModuleBuilder(rootPathProvider, serializers, modelBinderLocator),
                diagnosticsRouteCache,
                responseProcessors);

            var serializer = new DefaultObjectSerializer();

            pipelines.BeforeRequest.AddItemToStartOfPipeline(
                new PipelineItem<Func<NancyContext, Response>>(
                    PipelineKey,
                    ctx =>
                    {
                        if (!ctx.ControlPanelEnabled)
                        {
                            return null;
                        }

                        if (!ctx.Request.Path.StartsWith(ControlPanelPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            return null;
                        }

                        if (ctx.Request.Path.StartsWith(ResourcePrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            var resourceNamespace = "Nancy.Diagnostics.Resources";

                            var path = Path.GetDirectoryName(ctx.Request.Url.Path.Replace(ResourcePrefix, string.Empty)) ?? string.Empty;
                            if (!string.IsNullOrEmpty(path))
                            {
                                resourceNamespace += string.Format(".{0}", path.Replace('\\', '.'));
                            }

                            return new EmbeddedFileResponse(
                                typeof(DiagnosticsHook).Assembly,
                                resourceNamespace,
                                Path.GetFileName(ctx.Request.Url.Path));
                        }

                        return diagnosticsConfiguration.Valid
                                   ? ExecuteDiagnostics(ctx, diagnosticsRouteResolver, diagnosticsConfiguration, serializer)
                                   : GetDiagnosticsHelpView(ctx);
                    }));
        }

        public static void Disable(IPipelines pipelines)
        {
            pipelines.BeforeRequest.RemoveByName(PipelineKey);
        }

        private static Response GetDiagnosticsHelpView(NancyContext ctx)
        {
            return (StaticConfiguration.IsRunningDebug)
                       ? new DiagnosticsViewRenderer(ctx)["help"]
                       : HttpStatusCode.NotFound;
        }

        private static Response GetDiagnosticsLoginView(NancyContext ctx)
        {
            var renderer = new DiagnosticsViewRenderer(ctx);

            return renderer["login"];
        }

        private static Response ExecuteDiagnostics(NancyContext ctx, IRouteResolver routeResolver, DiagnosticsConfiguration diagnosticsConfiguration, DefaultObjectSerializer serializer)
        {
            var session = GetSession(ctx, diagnosticsConfiguration, serializer);

            if (session == null)
            {
                var view = GetDiagnosticsLoginView(ctx);

                view.AddCookie(
                    new NancyCookie(DiagsCookieName, String.Empty, true) { Expires = DateTime.Now.AddDays(-1) });

                return view;
            }

            // TODO - duplicate the context and strip out the "_/Nancy" bit so we don't need to use it in the module
            var resolveResult = routeResolver.Resolve(ctx);

            ctx.Parameters = resolveResult.Item2;
            var resolveResultPreReq = resolveResult.Item3;
            var resolveResultPostReq = resolveResult.Item4;
            ExecuteRoutePreReq(ctx, resolveResultPreReq);

            if (ctx.Response == null)
            {
                ctx.Response = resolveResult.Item1.Invoke(resolveResult.Item2);
            }

            if (ctx.Request.Method.ToUpperInvariant() == "HEAD")
            {
                ctx.Response = new HeadResponse(ctx.Response);
            }

            if (resolveResultPostReq != null)
            {
                resolveResultPostReq.Invoke(ctx);
            }

            AddUpdateSessionCookie(session, ctx, diagnosticsConfiguration, serializer);

            // If we duplicate the context this makes more sense :)
            return ctx.Response;
        }

        private static void AddUpdateSessionCookie(DiagnosticsSession session, NancyContext context, DiagnosticsConfiguration diagnosticsConfiguration, DefaultObjectSerializer serializer)
        {
            if (context.Response == null)
            {
                return;
            }

            session.Expiry = DateTime.Now.AddMinutes(DiagnosticsSessionTimeoutMinutes);
            var serializedSession = serializer.Serialize(session);

            var encryptedSession = diagnosticsConfiguration.CryptographyConfiguration.EncryptionProvider.Encrypt(serializedSession);
            var hmacBytes = diagnosticsConfiguration.CryptographyConfiguration.HmacProvider.GenerateHmac(encryptedSession);
            var hmacString = Convert.ToBase64String(hmacBytes);

            var cookie = new NancyCookie(DiagsCookieName, String.Format("{1}{0}", encryptedSession, hmacString), true);
            
            context.Response.AddCookie(cookie);
        }

        private static DiagnosticsSession GetSession(NancyContext context, DiagnosticsConfiguration diagnosticsConfiguration, DefaultObjectSerializer serializer)
        {
            if (context.Request == null)
            {
                return null;
            }

            if (IsLoginRequest(context))
            {
                return ProcessLogin(context, diagnosticsConfiguration, serializer);
            }

            if (!context.Request.Cookies.ContainsKey(DiagsCookieName))
            {
                return null;
            }

            var encryptedValue = HttpUtility.UrlDecode(context.Request.Cookies[DiagsCookieName]);
            var hmacStringLength = Base64Helpers.GetBase64Length(diagnosticsConfiguration.CryptographyConfiguration.HmacProvider.HmacLength);
            var encryptedSession = encryptedValue.Substring(hmacStringLength);
            var hmacString = encryptedValue.Substring(0, hmacStringLength);

            var hmacBytes = Convert.FromBase64String(hmacString);
            var newHmac = diagnosticsConfiguration.CryptographyConfiguration.HmacProvider.GenerateHmac(encryptedSession);
            var hmacValid = HmacComparer.Compare(newHmac, hmacBytes, diagnosticsConfiguration.CryptographyConfiguration.HmacProvider.HmacLength);

            if (!hmacValid)
            {
                return null;
            }

            var decryptedValue = diagnosticsConfiguration.CryptographyConfiguration.EncryptionProvider.Decrypt(encryptedSession);
            var session = serializer.Deserialize(decryptedValue) as DiagnosticsSession;
            
            if (session == null || session.Expiry < DateTime.Now || !SessionPasswordValid(session, diagnosticsConfiguration.Password))
            {
                return null;
            }

            return session;
        }

        private static bool SessionPasswordValid(DiagnosticsSession session, string realPassword)
        {
            var newHash = DiagnosticsSession.GenerateSaltedHash(realPassword, session.Salt);

            return (newHash.Length == session.Hash.Length && newHash.SequenceEqual(session.Hash));
        }

        private static DiagnosticsSession ProcessLogin(NancyContext context, DiagnosticsConfiguration diagnosticsConfiguration, DefaultObjectSerializer serializer)
        {
            string password = context.Request.Form.Password;

            if (!string.Equals(password, diagnosticsConfiguration.Password, StringComparison.Ordinal))
            {
                return null;
            }

            var salt = DiagnosticsSession.GenerateRandomSalt();
            var hash = DiagnosticsSession.GenerateSaltedHash(password, salt);
            var session = new DiagnosticsSession
            {
                Hash = hash,
                Salt = salt,
                Expiry = DateTime.Now.AddMinutes(DiagnosticsSessionTimeoutMinutes),
            };

            return session;
        }

        private static bool IsLoginRequest(NancyContext context)
        {
            // This feels dirty :)
            return context.Request.Method == "POST" && context.Request.Path == "/_Nancy/";
        }

        private static void ExecuteRoutePreReq(NancyContext context, Func<NancyContext, Response> resolveResultPreReq)
        {
            if (resolveResultPreReq == null)
            {
                return;
            }

            var resolveResultPreReqResponse = resolveResultPreReq.Invoke(context);

            if (resolveResultPreReqResponse != null)
            {
                context.Response = resolveResultPreReqResponse;
            }
        }
    }
}