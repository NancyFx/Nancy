namespace Nancy.Tests.Unit.Sessions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Nancy.Cryptography;

    using FakeItEasy;

    using Nancy.Bootstrapper;
    using Nancy.IO;
    using Nancy.Session;

    using Xunit;

    public class CookieBasedSessionsFixture
    {
        private const string ValidData = "VgPJvXYwcXkn0gxDvg84tsfV9F5he1ZxhjsTZK1UZHVqWk7izPd9XsWnuFMrtQNRZTXXQIQC70PUcl2gxUczXtP1Z7hGdTHdqwopP1/WRQtF949V7EO2JXG7sna/AVkPZvF730XD3oCBY4hxhIydTOw+b/PZYsXYQGTRCp9Ynv/hO3zPt/vKAViGP1n+HrSFwLyiw2RpAzl1/9psKzkL2Mh2YRkDjhb3SSHpbjPgkgyvhImy/L4zd3eKV4UdLR8sUNdEMNoA37nECCPmM+A+nveHUn9HkwakkHsVsj3hh78yEHj1StgMdEAipgyqIdDrc5W36Gr8MTeVeU4+i02Dz0p5kRoFrZXb2q1sV5fTVYoDeQ11ovx/XnmWSO6m5zD+6AFGZWzzSkaeGDXa+tBhVD921SMqWxN2v1AMAO7LgWVdxOwF48ryuhmkLKhNouC7Oe8eUke3BENryJnH7Jp+0/WaCyWV5pAbfl2v1ysbZqvhyXowdm9qSF0quBV/vvkp4wuc4TMXjQP5/TGssbojYKshzBF+buSg+lP5W5pFGne28jjx3+matLr+aA3pgvPj7+JYSRNi3emkuTfAX00neJbbk/g6subFD/KDepDk3tqBXlde0Mhhg5jRX9h2kCpo8vuM3bW5TfXbcwUkTW/0cM9Oq7kYjb0F/jcTbS+OmjC/125BxOslYqa9kun5/SILOJuU757jvfTXzoMCNeb2u7n92Y4iLbV0J/yMmLOBzkN+w5pzzKoKltQXDot4FuCeTtZrL3yYPRDASbCIjpwGEl8X5KUwBpCHE3Cbz0DTduzCJSXZV77MFLlNsT+mj8CtZydTi+wQ9p9Ssty8HVgz/0AeuRABpHZ8S+x9jnwy04vkryYgtKHidBRh+X85GXhIdrM91+oGzN9eDHKjhLLvX5cYuSiSW387keD4hZt/7YGHlOFCW9EL8miiBp5BJ1WN";
        private const string ValidHmac = "FD+CB9Zm/n18DdcWIoj55UjImgJliwWjKiArlKaBbFE=";

        private readonly IEncryptionProvider fakeEncryptionProvider;
        private readonly CookieBasedSessions cookieStore;
        private readonly IHmacProvider fakeHmacProvider;

        private RijndaelEncryptionProvider rijndaelEncryptionProvider;

        private DefaultHmacProvider defaultHmacProvider;

        public CookieBasedSessionsFixture()
        {
            this.fakeEncryptionProvider = A.Fake<IEncryptionProvider>();
            this.fakeHmacProvider = A.Fake<IHmacProvider>();
            this.cookieStore = new CookieBasedSessions(this.fakeEncryptionProvider, this.fakeHmacProvider, new Fakes.FakeSessionObjectFormatter());
            var keyGenerator = new PassphraseKeyGenerator("password");
            this.rijndaelEncryptionProvider = new RijndaelEncryptionProvider(keyGenerator);
            this.defaultHmacProvider = new DefaultHmacProvider(keyGenerator);
        }

        [Fact]
        public void Should_save_nothing_if_the_session_is_null()
        {
            var response = new Response();

            cookieStore.Save(null, response);
            
            response.Cookies.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_save_nothing_if_the_session_has_not_changed()
        {
            var response = new Response();

            cookieStore.Save(new Session(new Dictionary<string, object> { { "key", "value" } }), response);

            response.Cookies.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_save_the_session_cookie()
        {
            var response = new Response();
            var session = new Session(new Dictionary<string, object>
                                      {
                                          {"key1", "val1"},                                          
                                      });
            session["key2"] = "val2";
            A.CallTo(() => this.fakeEncryptionProvider.Encrypt("key1=val1;key2=val2;")).Returns("encrypted=key1=val1;key2=val2;");

            cookieStore.Save(session, response);

            response.Cookies.Count.ShouldEqual(1);
            var cookie = response.Cookies.First();
            cookie.Name.ShouldEqual(CookieBasedSessions.GetCookieName());
            cookie.Value.ShouldEqual("encrypted=key1=val1;key2=val2;");
            cookie.Expires.ShouldBeNull();
            cookie.Path.ShouldBeNull();
            cookie.Domain.ShouldBeNull();
        }

        [Fact]
        public void Should_save_cookie_as_http_only()
        {
            var response = new Response();
            var session = new Session();
            session["key 1"] = "val=1";
            A.CallTo(() => this.fakeEncryptionProvider.Encrypt("key+1=val%3d1;")).Returns("encryptedkey+1=val%3d1;");

            cookieStore.Save(session, response);

            response.Cookies.First().HttpOnly.ShouldEqual(true);
        }

        [Fact]
        public void Should_saves_url_safe_keys_and_values()
        {
            var response = new Response();
            var session = new Session();
            session["key 1"] = "val=1";
            A.CallTo(() => this.fakeEncryptionProvider.Encrypt("key+1=val%3d1;")).Returns("encryptedkey+1=val%3d1;");

            cookieStore.Save(session, response);

            response.Cookies.First().Value.ShouldEqual("encryptedkey+1=val%3d1;");
        }

        [Fact]
        public void Should_load_an_empty_session_if_no_session_cookie_exists()
        {
            var request = CreateRequest(null);

            var result = cookieStore.Load(request);
            
            result.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_load_a_single_valued_session()
        {
            var request = CreateRequest("encryptedkey1=value1");
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey1=value1")).Returns("key1=value1;");

            var session = cookieStore.Load(request);

            session.Count.ShouldEqual(1);
            session["key1"].ShouldEqual("value1");
        }

        [Fact]
        public void Should_load_a_multi_valued_session()
        {
            var request = CreateRequest("encryptedkey1=value1;key2=value2");
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey1=value1;key2=value2")).Returns("key1=value1;key2=value2");

            var session = cookieStore.Load(request);

            session.Count.ShouldEqual(2);
            session["key1"].ShouldEqual("value1");
            session["key2"].ShouldEqual("value2");
        }

        [Fact]
        public void Should_load_properly_decode_the_url_safe_session()
        {
            var request = CreateRequest("encryptedkey+1=val%3d1;");
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey+1=val%3d1;")).Returns("key+1=val%3d1;");

            var session = cookieStore.Load(request);

            session.Count.ShouldEqual(1);
            session["key 1"].ShouldEqual("val=1");
        }

        [Fact]
        public void Should_add_pre_and_post_hooks_when_enabled()
        {
            var beforePipeline = new BeforePipeline();
            var afterPipeline = new AfterPipeline();
            var hooks = A.Fake<IApplicationPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);

            CookieBasedSessions.Enable(hooks, new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider));

            beforePipeline.PipelineItems.Count().ShouldEqual(1);
            afterPipeline.PipelineItems.Count().ShouldEqual(1);
        }

        [Fact]
        public void Should_only_not_add_response_cookie_if_it_has_not_changed()
        {
            var beforePipeline = new BeforePipeline();
            var afterPipeline = new AfterPipeline();
            var hooks = A.Fake<IApplicationPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);
            CookieBasedSessions.Enable(hooks, new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider)).WithFormatter(new Fakes.FakeSessionObjectFormatter());
            var request = CreateRequest("encryptedkey1=value1");
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey1=value1")).Returns("key1=value1;");
            var response = A.Fake<Response>();
            var nancyContext = new NancyContext() { Request = request, Response = response };
            beforePipeline.Invoke(nancyContext);

            afterPipeline.Invoke(nancyContext);

            response.Cookies.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_add_response_cookie_if_it_has_changed()
        {
            var beforePipeline = new BeforePipeline();
            var afterPipeline = new AfterPipeline();
            var hooks = A.Fake<IApplicationPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);
            CookieBasedSessions.Enable(hooks, new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider)).WithFormatter(new Fakes.FakeSessionObjectFormatter());
            var request = CreateRequest("encryptedkey1=value1");
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey1=value1")).Returns("key1=value1;");
            var response = A.Fake<Response>();
            var nancyContext = new NancyContext() { Request = request, Response = response };
            beforePipeline.Invoke(nancyContext);
            request.Session["Testing"] = "Test";

            afterPipeline.Invoke(nancyContext);

            response.Cookies.Count.ShouldEqual(1);
        }

        [Fact]
        public void Should_call_formatter_on_load()
        {
            var fakeFormatter = A.Fake<ISessionObjectFormatter>();
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey1=value1")).Returns("key1=value1;");
            var store = new CookieBasedSessions(this.fakeEncryptionProvider, this.fakeHmacProvider, fakeFormatter);
            var request = CreateRequest("encryptedkey1=value1", false);

            store.Load(request);

            A.CallTo(() => fakeFormatter.Deserialize("value1")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_call_the_formatter_on_save()
        {
            var response = new Response();
            var session = new Session(new Dictionary<string, object>());
            session["key1"] = "value1";
            var fakeFormatter = A.Fake<ISessionObjectFormatter>();
            var store = new CookieBasedSessions(this.fakeEncryptionProvider, this.fakeHmacProvider, fakeFormatter);

            store.Save(session, response);

            A.CallTo(() => fakeFormatter.Serialize("value1")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_formatter_when_using_formatter_selector()
        {
            var beforePipeline = new BeforePipeline();
            var afterPipeline = new AfterPipeline();
            var hooks = A.Fake<IApplicationPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);
            var fakeFormatter = A.Fake<ISessionObjectFormatter>();
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey1=value1")).Returns("key1=value1;");
            CookieBasedSessions.Enable(hooks, new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider)).WithFormatter(fakeFormatter);
            var request = CreateRequest("encryptedkey1=value1");
            var nancyContext = new NancyContext() { Request = request };

            beforePipeline.Invoke(nancyContext);

            A.CallTo(() => fakeFormatter.Deserialize(A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_be_able_to_save_a_complex_object_to_session()
        {
            var response = new Response();
            var session = new Session(new Dictionary<string, object>());
            var payload = new DefaultSessionObjectFormatterFixture.Payload(27, true, "Test string");
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultSessionObjectFormatter());
            session["testObject"] = payload;

            store.Save(session, response);

            response.Cookies.Count.ShouldEqual(1);
            var cookie = response.Cookies.First();
            cookie.Name.ShouldEqual(CookieBasedSessions.GetCookieName());
            cookie.Value.ShouldNotBeNull();
            cookie.Value.ShouldNotBeEmpty();
        }

        [Fact]
        public void Should_be_able_to_load_an_object_previously_saved_to_session()
        {
            var response = new Response();
            var session = new Session(new Dictionary<string, object>());
            var payload = new DefaultSessionObjectFormatterFixture.Payload(27, true, "Test string");
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultSessionObjectFormatter());
            session["testObject"] = payload;
            store.Save(session, response);
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(Helpers.HttpUtility.UrlEncode(response.Cookies.First().Name), Helpers.HttpUtility.UrlEncode(response.Cookies.First().Value));

            var result = store.Load(request);

            result["testObject"].ShouldEqual(payload);
        }

        [Fact]
        public void Should_encrypt_data()
        {
            var response = new Response();
            var session = new Session(new Dictionary<string, object>
                                      {
                                          {"key1", "val1"},                                          
                                      });
            session["key2"] = "val2";

            cookieStore.Save(session, response);

            A.CallTo(() => this.fakeEncryptionProvider.Encrypt(A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_generate_hmac()
        {
            var response = new Response();
            var session = new Session(new Dictionary<string, object>
                                      {
                                          {"key1", "val1"},                                          
                                      });
            session["key2"] = "val2";

            cookieStore.Save(session, response);

            A.CallTo(() => this.fakeHmacProvider.GenerateHmac(A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_load_valid_test_data()
        {
            var inputValue = ValidHmac + ValidData;
            inputValue = HttpUtility.UrlEncode(inputValue);
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultSessionObjectFormatter());
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(CookieBasedSessions.GetCookieName(), inputValue);

            var result = store.Load(request);

            result.Count.ShouldEqual(1);
            result.First().Value.ShouldBeOfType(typeof(DefaultSessionObjectFormatterFixture.Payload));
        }

        [Fact]
        public void Should_return_blank_session_if_hmac_changed()
        {
            var inputValue = "b" + ValidHmac.Substring(1) + ValidData;
            inputValue = HttpUtility.UrlEncode(inputValue);
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultSessionObjectFormatter());
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(CookieBasedSessions.GetCookieName(), inputValue);

            var result = store.Load(request);

            result.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_blank_session_if_hmac_missing()
        {
            var inputValue = ValidData;
            inputValue = HttpUtility.UrlEncode(inputValue);
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultSessionObjectFormatter());
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(CookieBasedSessions.GetCookieName(), inputValue);

            var result = store.Load(request);

            result.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_blank_session_if_encrypted_data_modified()
        {
            var inputValue = ValidHmac + ValidData.Substring(0, ValidData.Length - 1) + "Z";
            inputValue = HttpUtility.UrlEncode(inputValue);
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultSessionObjectFormatter());
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(CookieBasedSessions.GetCookieName(), inputValue);

            var result = store.Load(request);

            result.Count.ShouldEqual(0);
        }

        private Request CreateRequest(string sessionValue, bool load = true)
        {
            var headers = new Dictionary<string, IEnumerable<string>>(1);

            if (!string.IsNullOrEmpty(sessionValue))
            {
                headers.Add("cookie", new[] { CookieBasedSessions.GetCookieName()+ "=" + HttpUtility.UrlEncode(sessionValue) });
            }

            var request = new Request("GET", "http://goku.power:9001/", headers, CreateRequestStream(), "http");

            if (load)
            {
                cookieStore.Load(request);
            }

            return request;
        }

        private static RequestStream CreateRequestStream()
        {
            return CreateRequestStream(new MemoryStream());
        }

        private static RequestStream CreateRequestStream(Stream stream)
        {
            return RequestStream.FromStream(stream, 0, 1, true);
        }
    }
}