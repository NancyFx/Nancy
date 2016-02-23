namespace Nancy.Owin.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using global::Owin;

    using Microsoft.Owin.Testing;

    using Nancy.Testing;

    using Xunit;

    using HttpStatusCode = Nancy.HttpStatusCode;

    public class AppBuilderExtensionsFixture
    {
#if !__MonoCS__
        [Fact]
        public void When_host_Nancy_via_IAppBuilder_then_should_handle_requests()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(config => config.Module<TestModule>());

            using (var server = TestServer.Create(app => app.UseNancy(opts => opts.Bootstrapper = bootstrapper)))
            {
                // When
                var response = server.HttpClient.GetAsync(new Uri("http://localhost/")).Result;

                // Then
                Assert.Equal(response.StatusCode, System.Net.HttpStatusCode.OK);
            }
        }

        [Fact]
        public void When_host_Nancy_via_IAppBuilder_should_read_X509Certificate2()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(config => config.Module<TestModule>());


            using (var server = TestServer.Create(app => app.UseNancy(opts =>
            {
                opts.Bootstrapper = bootstrapper;
                opts.EnableClientCertificates = true;
            })))
            {
                // When
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

                byte[] embeddedCert = Encoding.UTF8.GetBytes(cert);

                var env = new Dictionary<string, object>()
                {
                    { "owin.RequestPath", "/ssl" },
                    { "owin.RequestScheme", "http" },
                    { "owin.RequestHeaders", new Dictionary<string, string[]>() { { "Host", new[] { "localhost" } } } },
                    { "owin.RequestMethod", "GET" },
                    {"ssl.ClientCertificate", new X509Certificate(embeddedCert) }
                };
                server.Invoke(env);


                // Then
                Assert.Equal(env["owin.ResponseStatusCode"], 200);
            }
        }
#endif

        public class TestModule : LegacyNancyModule
        {
            public TestModule()
            {
                Get["/"] = _ =>
                {
                    return HttpStatusCode.OK;
                };

                Get["/ssl"] = _ =>
                {
                    return this.Request.ClientCertificate != null ? 200 : 500;
                };
            }
        }
    }
}