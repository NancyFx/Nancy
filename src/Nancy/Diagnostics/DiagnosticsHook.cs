namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Nancy.Bootstrapper;
    using Nancy.Configuration;
    using Nancy.Cookies;
    using Nancy.Cryptography;
    using Nancy.Culture;
    using Nancy.Json;
    using Nancy.Localization;
    using Nancy.ModelBinding;
    using Nancy.Responses;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Routing.Constraints;
    using Nancy.Routing.Trie;

    /// <summary>
    /// Pipeline hook to handle diagnostics dashboard requests.
    /// </summary>
    public static class DiagnosticsHook
    {
        private static readonly CancellationToken CancellationToken = new CancellationToken();

        private const string PipelineKey = "__Diagnostics";

        internal const string ItemsKey = "DIAGS_REQUEST";

        /// <summary>
        /// Enables the diagnostics dashboard and will intercept all requests that are passed to
        /// the condigured paths.
        /// </summary>
        public static void Enable(IPipelines pipelines, IEnumerable<IDiagnosticsProvider> providers, IRootPathProvider rootPathProvider, IRequestTracing requestTracing, NancyInternalConfiguration configuration, IModelBinderLocator modelBinderLocator, IEnumerable<IResponseProcessor> responseProcessors, IEnumerable<IRouteSegmentConstraint> routeSegmentConstraints, ICultureService cultureService, IRequestTraceFactory requestTraceFactory, IEnumerable<IRouteMetadataProvider> routeMetadataProviders, ITextResource textResource, INancyEnvironment environment)
        {
            var diagnosticsConfiguration =
                environment.GetValue<DiagnosticsConfiguration>();

            var diagnosticsEnvironment =
                GetDiagnosticsEnvironment();

            var diagnosticsModuleCatalog = new DiagnosticsModuleCatalog(providers, rootPathProvider, requestTracing, configuration, diagnosticsEnvironment);

            var diagnosticsRouteCache = new RouteCache(
                diagnosticsModuleCatalog,
                new DefaultNancyContextFactory(cultureService, requestTraceFactory, textResource),
                new DefaultRouteSegmentExtractor(),
                new DefaultRouteDescriptionProvider(),
                cultureService,
                routeMetadataProviders);

            var diagnosticsRouteResolver = new DefaultRouteResolver(
                diagnosticsModuleCatalog,
                new DiagnosticsModuleBuilder(rootPathProvider, modelBinderLocator, diagnosticsEnvironment, environment),
                diagnosticsRouteCache,
                new RouteResolverTrie(new TrieNodeFactory(routeSegmentConstraints)));

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

                        if (!ctx.Request.Path.StartsWith(diagnosticsConfiguration.Path, StringComparison.OrdinalIgnoreCase))
                        {
                            return null;
                        }

                        ctx.Items[ItemsKey] = true;

                        var resourcePrefix =
                            string.Concat(diagnosticsConfiguration.Path, "/Resources/");

                        if (ctx.Request.Path.StartsWith(resourcePrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            var resourceNamespace = "Nancy.Diagnostics.Resources";

                            var path = Path.GetDirectoryName(ctx.Request.Url.Path.Replace(resourcePrefix, string.Empty)) ?? string.Empty;
                            if (!string.IsNullOrEmpty(path))
                            {
                                resourceNamespace += string.Format(".{0}", path.Replace(Path.DirectorySeparatorChar, '.'));
                            }

                            return new EmbeddedFileResponse(
                                typeof(DiagnosticsHook).Assembly,
                                resourceNamespace,
                                Path.GetFileName(ctx.Request.Url.Path));
                        }

                        RewriteDiagnosticsUrl(diagnosticsConfiguration, ctx);

                        return ValidateConfiguration(diagnosticsConfiguration)
                                   ? ExecuteDiagnostics(ctx, diagnosticsRouteResolver, diagnosticsConfiguration, serializer)
                                   : GetDiagnosticsHelpView(ctx);
                    }));
        }

        /// <summary>
        /// Gets a special <see cref="INancyEnvironment"/> instance that is separate from the
        /// one used by the application.
        /// </summary>
        /// <returns></returns>
        private static INancyEnvironment GetDiagnosticsEnvironment()
        {
            var diagnosticsEnvironment =
                new DefaultNancyEnvironment();

            diagnosticsEnvironment.Json(retainCasing: false);

            return diagnosticsEnvironment;
        }

        private static bool ValidateConfiguration(DiagnosticsConfiguration configuration)
        {
            return !string.IsNullOrWhiteSpace(configuration.Password) &&
                !string.IsNullOrWhiteSpace(configuration.CookieName) &&
                !string.IsNullOrWhiteSpace(configuration.Path) &&
                configuration.SlidingTimeout != 0;
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

                view.WithCookie(
                    new NancyCookie(diagnosticsConfiguration.CookieName, String.Empty, true) { Expires = DateTime.Now.AddDays(-1) });

                return view;
            }

            var resolveResult = routeResolver.Resolve(ctx);

            ctx.Parameters = resolveResult.Parameters;
            ExecuteRoutePreReq(ctx, CancellationToken, resolveResult.Before);

            if (ctx.Response == null)
            {
                // Don't care about async here, so just get the result
                var task = resolveResult.Route.Invoke(resolveResult.Parameters, CancellationToken);
                task.Wait();
                ctx.Response = task.Result;
            }

            if (ctx.Request.Method.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
            {
                ctx.Response = new HeadResponse(ctx.Response);
            }

            if (resolveResult.After != null)
            {
                resolveResult.After.Invoke(ctx, CancellationToken);
            }

            AddUpdateSessionCookie(session, ctx, diagnosticsConfiguration, serializer);

            return ctx.Response;
        }

        private static void AddUpdateSessionCookie(DiagnosticsSession session, NancyContext context, DiagnosticsConfiguration diagnosticsConfiguration, DefaultObjectSerializer serializer)
        {
            if (context.Response == null)
            {
                return;
            }

            session.Expiry = DateTime.Now.AddMinutes(diagnosticsConfiguration.SlidingTimeout);
            var serializedSession = serializer.Serialize(session);

            var encryptedSession = diagnosticsConfiguration.CryptographyConfiguration.EncryptionProvider.Encrypt(serializedSession);
            var hmacBytes = diagnosticsConfiguration.CryptographyConfiguration.HmacProvider.GenerateHmac(encryptedSession);
            var hmacString = Convert.ToBase64String(hmacBytes);

            var cookie = new NancyCookie(diagnosticsConfiguration.CookieName, String.Format("{1}{0}", encryptedSession, hmacString), true);

            context.Response.WithCookie(cookie);
        }

        private static DiagnosticsSession GetSession(NancyContext context, DiagnosticsConfiguration diagnosticsConfiguration, DefaultObjectSerializer serializer)
        {
            if (context.Request == null)
            {
                return null;
            }

            if (IsLoginRequest(context, diagnosticsConfiguration))
            {
                return ProcessLogin(context, diagnosticsConfiguration, serializer);
            }

            if (!context.Request.Cookies.ContainsKey(diagnosticsConfiguration.CookieName))
            {
                return null;
            }

            var encryptedValue = context.Request.Cookies[diagnosticsConfiguration.CookieName];
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
                Expiry = DateTime.Now.AddMinutes(diagnosticsConfiguration.SlidingTimeout)
            };

            return session;
        }

        private static bool IsLoginRequest(NancyContext context, DiagnosticsConfiguration diagnosticsConfiguration)
        {
            return context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                context.Request.Url.BasePath.TrimEnd(new[] { '/' }).EndsWith(diagnosticsConfiguration.Path) &&
                context.Request.Url.Path == "/";
        }

        private static void ExecuteRoutePreReq(NancyContext context, CancellationToken cancellationToken, BeforePipeline resolveResultPreReq)
        {
            if (resolveResultPreReq == null)
            {
                return;
            }

            var resolveResultPreReqResponse = resolveResultPreReq.Invoke(context, cancellationToken).Result;

            if (resolveResultPreReqResponse != null)
            {
                context.Response = resolveResultPreReqResponse;
            }
        }

        private static void RewriteDiagnosticsUrl(DiagnosticsConfiguration diagnosticsConfiguration, NancyContext ctx)
        {
            ctx.Request.Url.BasePath =
                string.Concat(ctx.Request.Url.BasePath, diagnosticsConfiguration.Path);

            ctx.Request.Url.Path =
                ctx.Request.Url.Path.Substring(diagnosticsConfiguration.Path.Length);

            if (ctx.Request.Url.Path.Length.Equals(0))
            {
                ctx.Request.Url.Path = "/";
            }
        }
    }
}
