namespace Nancy.Tests.Unit.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Bootstrapper;
    using Nancy.Configuration;
    using Nancy.Cryptography;
    using Nancy.Culture;
    using Nancy.Diagnostics;
    using Nancy.Helpers;
    using Nancy.Localization;
    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Routing.Constraints;
    using Nancy.Testing;

    using Xunit;
//While this directive is redundant, it's required to build on mono 2.x to allow it to resolve the Should* extension methods

    public class CustomInteractiveDiagnosticsHookFixture
    {
        private const string DiagsCookieName = "__ncd";

        private readonly CryptographyConfiguration cryptoConfig;

        private readonly IObjectSerializer objectSerializer;

        public CustomInteractiveDiagnosticsHookFixture()
        {
            this.cryptoConfig = CryptographyConfiguration.Default;
            this.objectSerializer = new DefaultObjectSerializer();
        }

        private class FakeDiagnostics : IDiagnostics
        {
            private readonly IEnumerable<IDiagnosticsProvider> diagnosticProviders;
            private readonly IRootPathProvider rootPathProvider;
            private readonly IRequestTracing requestTracing;
            private readonly NancyInternalConfiguration configuration;
            private readonly IModelBinderLocator modelBinderLocator;
            private readonly IEnumerable<IResponseProcessor> responseProcessors;
            private readonly IEnumerable<IRouteSegmentConstraint> routeSegmentConstraints;
            private readonly ICultureService cultureService;
            private readonly IRequestTraceFactory requestTraceFactory;
            private readonly IEnumerable<IRouteMetadataProvider> routeMetadataProviders;
            private readonly ITextResource textResource;
            private readonly INancyEnvironment environment;

            public FakeDiagnostics(
                IRootPathProvider rootPathProvider,
                IRequestTracing requestTracing,
                NancyInternalConfiguration configuration,
                IModelBinderLocator modelBinderLocator,
                IEnumerable<IResponseProcessor> responseProcessors,
                IEnumerable<IRouteSegmentConstraint> routeSegmentConstraints,
                ICultureService cultureService,
                IRequestTraceFactory requestTraceFactory,
                IEnumerable<IRouteMetadataProvider> routeMetadataProviders,
                ITextResource textResource,
                INancyEnvironment environment)
            {
                this.diagnosticProviders = (new IDiagnosticsProvider[] { new FakeDiagnosticsProvider() }).ToArray();
                this.rootPathProvider = rootPathProvider;
                this.requestTracing = requestTracing;
                this.configuration = configuration;
                this.modelBinderLocator = modelBinderLocator;
                this.responseProcessors = responseProcessors;
                this.routeSegmentConstraints = routeSegmentConstraints;
                this.cultureService = cultureService;
                this.requestTraceFactory = requestTraceFactory;
                this.routeMetadataProviders = routeMetadataProviders;
                this.textResource = textResource;
                this.environment = environment;
            }

            public void Initialize(IPipelines pipelines)
            {
                DiagnosticsHook.Enable(
                    pipelines,
                    this.diagnosticProviders,
                    this.rootPathProvider,
                    this.requestTracing,
                    this.configuration,
                    this.modelBinderLocator,
                    this.responseProcessors,
                    this.routeSegmentConstraints,
                    this.cultureService,
                    this.requestTraceFactory,
                    this.routeMetadataProviders,
                    this.textResource,
                    this.environment);
            }
        }

        private class FakeDiagnosticsProvider : IDiagnosticsProvider
        {
            public string Name
            {
                get { return "Fake testing provider"; }
            }

            public string Description
            {
                get { return "Fake testing provider"; }
            }

            public object DiagnosticObject
            {
                get { return this; }
            }
        }

        [Fact]
        public void Should_return_main_page_with_valid_auth_cookie()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Configure(env =>
                {
                    env.Diagnostics(
                        password: "password",
                        cryptographyConfiguration: this.cryptoConfig);
                });

                with.EnableAutoRegistration();
                with.Diagnostics<FakeDiagnostics>();
            });

            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Get(DiagnosticsConfiguration.Default.Path + "/interactive/providers/", with =>
                {
                    with.Cookie(DiagsCookieName, this.GetSessionCookieValue("password"));
                });

            // Then should see our fake provider and not the default testing provider
            result.Body.AsString().ShouldContain("Fake testing provider");
            result.Body.AsString().ShouldNotContain("Testing Diagnostic Provider");
        }

        private string GetSessionCookieValue(string password, DateTime? expiry = null)
        {
            var salt = DiagnosticsSession.GenerateRandomSalt();
            var hash = DiagnosticsSession.GenerateSaltedHash(password, salt);
            var session = new DiagnosticsSession
                {
                    Hash = hash,
                    Salt = salt,
                    Expiry = expiry.HasValue ? expiry.Value : DateTime.Now.AddMinutes(15),
                };

            var serializedSession = this.objectSerializer.Serialize(session);

            var encryptedSession = this.cryptoConfig.EncryptionProvider.Encrypt(serializedSession);
            var hmacBytes = this.cryptoConfig.HmacProvider.GenerateHmac(encryptedSession);
            var hmacString = Convert.ToBase64String(hmacBytes);

            return string.Format("{1}{0}", encryptedSession, hmacString);
        }
    }
}
