namespace Nancy.Owin.Tests
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using global::Owin;
    using Nancy.Testing;
    using Xunit;
    using HttpStatusCode = Nancy.HttpStatusCode;
    using Microsoft.Owin.Builder;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    public class AppBuilderExtensionsFixture
    {
#if !MONO
        [Fact]
        public async Task When_host_Nancy_via_IAppBuilder_then_should_handle_requests()
        {
            // Given
            var app = new AppBuilder();
            var bootstrapper = new ConfigurableBootstrapper(config => config.Module<TestModule>());
            app.UseNancy(opts => opts.Bootstrapper = bootstrapper);
            var appFunc = app.Build();

            var handler = new OwinHttpMessageHandler(appFunc);

            using (var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost")
            })
            {
                // When
                var response = await httpClient.GetAsync(new Uri("http://localhost/"));

                // Then
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task When_host_Nancy_via_IAppBuilder_should_read_X509Certificate2()
        {
            // Given
            var app = new AppBuilder();
            var bootstrapper = new ConfigurableBootstrapper(config => config.Module<TestModule>());

            var cert = @"-----BEGIN CERTIFICATE-----
                            MIICNTCCAZ4CCQC21XwOAYG32zANBgkqhkiG9w0BAQUFADBfMQswCQYDVQQGEwJH
                            QjETMBEGA1UECBMKU29tZS1TdGF0ZTEOMAwGA1UEChMFTmFuY3kxDjAMBgNVBAsT
                            BU5hbmN5MRswGQYDVQQDExJodHRwOi8vbmFuY3lmeC5vcmcwHhcNMTYwMjIyMTE1
                            NzQ3WhcNMTcwMjIxMTE1NzQ3WjBfMQswCQYDVQQGEwJHQjETMBEGA1UECBMKU29t
                            ZS1TdGF0ZTEOMAwGA1UEChMFTmFuY3kxDjAMBgNVBAsTBU5hbmN5MRswGQYDVQQD
                            ExJodHRwOi8vbmFuY3lmeC5vcmcwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGB
                            AMT4vOtIH9Fzad+8KCGjMPkkVpCtn+L5H97bnI3x+y3x5lY0WRsK8FyxVshY/7fv
                            TDeeVKUWanmbMkQjgFRYffA3ep3/AIguswYdANiNVHrx0L7DXNDcgsjRwaa6JVgQ
                            9iavlli0a80AF67FN1wfidlHCX53u3/fAjiSTwf7D+NBAgMBAAEwDQYJKoZIhvcN
                            AQEFBQADgYEAh12A4NntHHdVMGaw+2umXkWqCOyAPuNhyBGUHK5vGON+VG0HPFaf
                            8P8eMtdF4deBHkrfoWxRuGGn2tJzNpZLiEf23BAivEf36IqwfkVP7/zDwI+bjVXC
                            k64Un2uN8ALR/wLwfJzHfOLPtsca7ySWhlv8oZo2nk0vR34asQiGJDQ=
                            -----END CERTIFICATE-----
                            ";

            var embeddedCert = Encoding.UTF8.GetBytes(cert);

            app.Use(new Func<AppFunc, AppFunc>(next => (async env =>
            {
                env.Add("ssl.ClientCertificate", new X509Certificate(embeddedCert));
                await next.Invoke(env);
            })));

            app.UseNancy(opts =>
            {
                opts.Bootstrapper = bootstrapper;
                opts.EnableClientCertificates = true;
            });

            var appFunc = app.Build();

            var handler = new OwinHttpMessageHandler(appFunc);

            using (var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost")
            })
            {
                // When
                var response = await httpClient.GetAsync(new Uri("http://localhost/ssl"));

                // Then
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            }
        }
#endif
        public class TestModule : NancyModule
        {
            public TestModule()
            {
                Get("/", args => HttpStatusCode.OK);

                Get("/ssl", args => this.Request.ClientCertificate != null ? 200 : 500);
            }
        }
    }
}