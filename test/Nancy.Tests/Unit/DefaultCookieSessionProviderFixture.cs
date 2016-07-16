namespace Nancy.Tests.Unit
{
    using FakeItEasy;
    using Fakes;
    using Nancy.Cryptography;
    using Nancy.Helpers;
    using Nancy.Session;
    using Nancy.Testing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class DefaultCookieSessionProviderFixture
    {
        private const string ValidData = "PzQmxxriR9Ht9a1rJMC1yAr5Ty+CIIE1/fXX/B2e32wtf8+KzGaBMOW6Ks9xb503haQnFTm9Z9QsJgoUPClwU6Ke25HRjXdKY7RQIG4XXT7APU/NV3KAJZuwbJObv22PR/yKbkTOEYAvn/m7FLM6jxvn5lCCw75Kw0vNvKiTbyE5ijh5EY4XfQHGt+5s6vrehh26reJBuXiY4hPwopGbOsvHNNw3HpbQPmut1qHiqX/w8naD2vuFmX0Dckv1Kkf+K3zG2BVHtXi+C3kufmUR+l/1C7p4Y7r+6n9+7o7Bf8aWkMNWdAA704+xrT+Zyy5NkKkFGMtNmju6bv+6PH1H6hwXs5QUSWcj3Ke9XQvWaYBHCkBdIvY4FvMZUeQfw5oaECST+Zz+nMxAQ5TgOrvVo6Zr0B+CF+COvXeyhX7YbbM=";
        private const string ValidHmac = "QWc9gRZbQ6pK5ECs2Zp4E5WenpQ/XXgYUDPY46WihPY=";
        private DefaultCookieSessionProvider cookieStore;

        public DefaultCookieSessionProviderFixture()
        {
            cookieStore = new DefaultCookieSessionProvider();
            cookieStore.SetSessionDependencies(new FakeSessionDependency());
            cookieStore.Initialize();
        }

        [Fact]
        public async Task should_save_and_return_session_value()
        {
            Browser browser = new Browser(with=> {
                with.Module<SessionModule>();
            });
            var response = await browser.Get("/getsession");
            string result = response.Body.AsString();
            Assert.Contains(SessionModule.expectedSessionValue, result);
        }

        [Fact]
        public void should_be_using_DefaultCookieSessionProvider()
        {
            Browser browser = new Browser(with => { });
            Assert.Equal(typeof(DefaultCookieSessionProvider), SessionProvider.CurrentSessionProviderType);
        }
        [Fact]
        public void should_be_using_DefaultCookieSessionDependency()
        {
            Browser browser = new Browser(with => { });
            Assert.Equal(typeof(DefaultCookieSessionDependency), SessionProvider.CurrentSessionDependencyType);
        }

        [Fact]
        public void Should_save_nothing_if_the_session_is_null()
        {
            var ctx = new NancyContext();
            ctx.Response = new Response();
            
            cookieStore.Save(null, ctx);

            ctx.Response.Cookies.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_blank_session_if_hmac_changed()
        {
            var inputValue = "b" + ValidHmac.Substring(1) + ValidData;
            inputValue = HttpUtility.UrlEncode(inputValue);
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(cookieStore.CookieName, inputValue);

            var ctx = new NancyContext();
            ctx.Response = new Response();
            ctx.Request = request;

            var result = cookieStore.Load(ctx);

            result.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_blank_session_if_hmac_missing()
        {
            var inputValue = ValidData;
            inputValue = HttpUtility.UrlEncode(inputValue);
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(cookieStore.CookieName, inputValue);

            var ctx = new NancyContext();
            ctx.Response = new Response();
            ctx.Request = request;

            var result = cookieStore.Load(ctx);

            result.Count.ShouldEqual(0);
        }



        [Fact]
        public void Should_save_nothing_if_the_session_has_not_changed()
        {
            var ctx = new NancyContext();
            ctx.Response = new Response();
            
            cookieStore.Save(new NancySession(new Dictionary<string, object> { { "key", "value" } }), ctx);

            ctx.Response.Cookies.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_save_the_session_cookie()
        {
            var ctx = new NancyContext();
            ctx.Response = new Response();
            var session = new NancySession(new Dictionary<string, object>
                                      {
                                          {"key1", "val1"},
                                      });
            session["key2"] = "val2";
            A.CallTo(() => FakeSessionDependency.fakeEncryptionProvider.Encrypt("key1=val1;key2=val2;")).Returns("encrypted=key1=val1;key2=val2;");

            cookieStore.Save(session, ctx);

            ctx.Response.Cookies.Count.ShouldEqual(1);
            var cookie = ctx.Response.Cookies.First();
            cookie.Name.ShouldEqual(cookieStore.CookieName);
            cookie.Value.ShouldEqual("encrypted%3dkey1%3dval1%3bkey2%3dval2%3b");
            cookie.Expires.ShouldBeNull();
            cookie.Path.ShouldBeNull();
            cookie.Domain.ShouldBeNull();
        }



        [Fact]
        public void Should_return_blank_session_if_encrypted_data_modified()
        {
            var inputValue = ValidHmac + ValidData.Substring(0, ValidData.Length - 1) + "Z";
            inputValue = HttpUtility.UrlEncode(inputValue);
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(cookieStore.CookieName, inputValue);

            var ctx = new NancyContext();
            ctx.Response = new Response();
            ctx.Request = request;
            var result = cookieStore.Load(ctx);

            result.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_blank_session_if_encrypted_data_are_invalid_but_contain_semicolon_when_decrypted()
        {
            var bogusEncrypted = new AesEncryptionProvider(new PassphraseKeyGenerator("password", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000));
            
            var inputValue = ValidHmac + bogusEncrypted;
            inputValue = HttpUtility.UrlEncode(inputValue);
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(cookieStore.CookieName, inputValue);

            var ctx = new NancyContext();
            ctx.Response = new Response();
            ctx.Request = request;
            var result = cookieStore.Load(ctx);

            result.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_use_CookieName_when_config_provides_cookiename_value()
        {
            string cookiename = "NamedCookie";
            var fakeSessionDependency = new FakeSessionDependency()
            {
                GetInstancedDependencies = () => {
                    return (new object[] {
                            new CookieBasedSessionsConfiguration()
                            {
                                Serializer = FakeSessionDependency.fakeObjectSerializer,
                                CryptographyConfiguration=new CryptographyConfiguration(FakeSessionDependency.fakeEncryptionProvider,FakeSessionDependency.fakeHmacProvider)
                            },
                            new DefaultCookieConfiguration()
                            {
                                CookieName = cookiename
                            }
                        }).ToList().AsReadOnly();
                }
            };

            cookieStore.SetSessionDependencies(fakeSessionDependency);
            cookieStore.Initialize();

            //When
            var response = new Response();
            var session = new NancySession(new Dictionary<string, object>
                                        {
                                            {"key1", "val1"},
                                        });
            session["key2"] = "val2";


            var ctx = new NancyContext();
            ctx.Response = response;

            cookieStore.Save(session, ctx);

            //Then
            response.Cookies.ShouldHave(c => c.Name == cookiename);
            Assert.Equal(typeof(FakeSessionDependency), SessionProvider.CurrentSessionDependencyType);
        }

        [Fact]
        public void Should_set_Domain_when_config_provides_domain_value()
        {
            //Given
            string domainName = ".nancyfx.org";
            var fakeSessionDependency = new FakeSessionDependency()
            {
                GetInstancedDependencies = () => {
                    return (new object[] {
                            new CookieBasedSessionsConfiguration()
                            {
                                Serializer = FakeSessionDependency.fakeObjectSerializer,
                                CryptographyConfiguration=new CryptographyConfiguration(FakeSessionDependency.fakeEncryptionProvider,FakeSessionDependency.fakeHmacProvider)
                            },
                            new DefaultCookieConfiguration()
                            {
                                Domain = domainName
                            }
                        }).ToList().AsReadOnly();
                }
            };

            cookieStore.SetSessionDependencies(fakeSessionDependency);
            cookieStore.Initialize();

            //When
            var ctx = new NancyContext();
            ctx.Response = new Response();
            var session = new NancySession(new Dictionary<string, object>
                                        {
                                            {"key1", "val1"},
                                        });

            session["key2"] = "val2";
            cookieStore.Save(session, ctx);

            //Then
            var cookie = ctx.Response.Cookies.First(c => c.Name == DefaultCookieConfiguration.DefaultCookieName);
            cookie.Domain.ShouldEqual(domainName);

            Assert.Equal(typeof(FakeSessionDependency), SessionProvider.CurrentSessionDependencyType);
        }

        [Fact]
        public void Should_set_Path_when_config_provides_path_value()
        {
            //Given
            string path = "/";
            var fakeSessionDependency = new FakeSessionDependency()
            {
                GetInstancedDependencies = () => {
                    return (new object[] {
                            new CookieBasedSessionsConfiguration()
                            {
                                Serializer = FakeSessionDependency.fakeObjectSerializer,
                                CryptographyConfiguration=new CryptographyConfiguration(FakeSessionDependency.fakeEncryptionProvider,FakeSessionDependency.fakeHmacProvider)
                            },
                            new DefaultCookieConfiguration()
                            {
                                Path = path
                            }
                        }).ToList().AsReadOnly();
                }
            };
            
            cookieStore.SetSessionDependencies(fakeSessionDependency);
            cookieStore.Initialize();

            //When
            var ctx = new NancyContext();
            ctx.Response = new Response();
            var session = new NancySession(new Dictionary<string, object>
                                          {
                                              {"key1", "val1"},
                                          });
            session["key2"] = "val2";
            cookieStore.Save(session, ctx);

            //Then
            var cookie = ctx.Response.Cookies.First(c => c.Name == DefaultCookieConfiguration.DefaultCookieName);
            cookie.Path.ShouldEqual(path);
            Assert.Equal(typeof(FakeSessionDependency), SessionProvider.CurrentSessionDependencyType);
        }


        public class SessionModule : NancyModule
        {
            internal const string expectedSessionValue = "abc";
            public SessionModule()
            {
                Get<object>("getsession", p => {
                    if (Session["test"] == null)
                    {
                        Session["test"] = expectedSessionValue;
                    }

                    return Session["test"];
                });
            }
        }

        internal class FakeSessionDependency : ISessionDependency
        {
            public static IEncryptionProvider fakeEncryptionProvider = A.Fake<IEncryptionProvider>();
            public static IHmacProvider fakeHmacProvider = A.Fake<IHmacProvider>();
            public static IObjectSerializer fakeObjectSerializer = new FakeObjectSerializer();

            Func<IReadOnlyCollection<object>> _override;
            public Func<IReadOnlyCollection<object>> GetInstancedDependencies
            {
                get
                {
                    return (_override?? ( _override = 
                    () => {
                        return (new object[] {
                            new CookieBasedSessionsConfiguration()
                            {
                                Serializer = fakeObjectSerializer,
                                CryptographyConfiguration=new CryptographyConfiguration(fakeEncryptionProvider,fakeHmacProvider)
                            }
                        }).ToList().AsReadOnly();
                    }));
                }
                set
                {
                    _override = value;
                }
            }

            public Func<IReadOnlyCollection<Type>> GetTypeDependencies
            {
                get
                {
                    return () => {
                        return (new Type[] { typeof(DefaultCookieConfiguration) }).ToList().AsReadOnly();
                    };
                }
            }
        }
    }
}
