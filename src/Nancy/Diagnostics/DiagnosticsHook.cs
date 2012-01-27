namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Bootstrapper;
    using Cryptography;
    using Helpers;
    using ModelBinding;
    using Routing;

    public static class DiagnosticsHook
    {
        internal const string ControlPanelPrefix = "/_Nancy";

        internal const string ResourcePrefix = ControlPanelPrefix + "/Resources/";
        
        private const string PipelineKey = "__Diagnostics";

        private const string DiagsCookieName = "__ncd";

        public static void Enable(DiagnosticsConfiguration diagnosticsConfiguration, IPipelines pipelines, IEnumerable<IDiagnosticsProvider> providers, IRootPathProvider rootPathProvider, IEnumerable<ISerializer> serializers, IRequestTracing requestTracing, NancyInternalConfiguration configuration, IModelBinderLocator modelBinderLocator)
        {
            var keyGenerator = new DefaultModuleKeyGenerator();
            var diagnosticsModuleCatalog = new DiagnosticsModuleCatalog(keyGenerator, providers, rootPathProvider, requestTracing, configuration, diagnosticsConfiguration);

            var diagnosticsRouteCache = new RouteCache(diagnosticsModuleCatalog, keyGenerator, new DefaultNancyContextFactory());

            var diagnosticsRouteResolver = new DefaultRouteResolver(
                diagnosticsModuleCatalog,
                new DefaultRoutePatternMatcher(),
                new DiagnosticsModuleBuilder(rootPathProvider, serializers, modelBinderLocator),
                diagnosticsRouteCache);

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
                                   : GetDiagnosticsHelpView();
                    }));
        }

        public static void Disable(IPipelines pipelines)
        {
            pipelines.BeforeRequest.RemoveByName(PipelineKey);
        }

        private static Response GetDiagnosticsHelpView()
        {
            var renderer = new DiagnosticsViewRenderer();

            return renderer["help"];
        }

        private static Response GetDiagnosticsLoginView()
        {
            var renderer = new DiagnosticsViewRenderer();

            return renderer["login"];
        }

        private static Response ExecuteDiagnostics(NancyContext ctx, IRouteResolver routeResolver, DiagnosticsConfiguration diagnosticsConfiguration, DefaultObjectSerializer serializer)
        {
            var authCookie = GetSession(ctx, diagnosticsConfiguration, serializer);

            if (authCookie == null)
            {
                var view = GetDiagnosticsLoginView();

                view.AddCookie(DiagsCookieName, string.Empty);

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

            // If we duplicate the context this makes more sense :)
            return ctx.Response;
        }

        private static DiagnosticsSession GetSession(NancyContext context, DiagnosticsConfiguration diagnosticsConfiguration, DefaultObjectSerializer serializer)
        {
            if (context.Request == null)
            {
                return null;
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
            
            if (session == null || session.Expiry < DateTime.Now)
            {
                return null;
            }

            return session;
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