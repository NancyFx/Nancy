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
        private const string ValidData = "VgPJvXYwcXkn0gxDvg84tsfV9F5he1ZxhjsTZK1UZHVqWk7izPd9XsWnuFMrtQNRJEfyiqU2J7tAZDQvdKjQij9wUO6mOTCyZ7HPHK/pEnkgDFMXbHDctGQZSbb2WZZxola+Q3nP2tlQ+Tx//N6YyK7BwpsNPrvyHAvU1z5YzHfPT6HEowIl8mz/uUL6o+FME/Goi7RN2getMeYaPCs0fJkiMCAomnnagAy4aXN0Ak/p7Y3K/kpNAS6PvNu4aok0zVpfo1utP84GyyLomfr4urmDNFIe8PBVoKhuomxjsUOddaarHqqmN3PXOp15SPXPDxEKfpuLzhmqXnStiB8nH9qMBYI/AuLHMckDzkeESH5rQ2q2+1RgCN82PujzGhhVnBMk95ZS9k9zKCvKQa2yzVkaHqwSESyOFboU89kLAEQ0h48dtoJ2FTBs9GjsL3Z4fGogeLwjIvP8I8JF39HI+9U3PC2KnicA/bgUL/Z1paDzZYTrqQS4QSyFgy4DOxYz";
        private const string ValidHmac = "un/5uJOoOAyn4AX8VU0HsGYYtr79A40TFF1wVqd/jDQ=";

        private readonly IEncryptionProvider fakeEncryptionProvider;
        private readonly CookieBasedSessions cookieStore;
        private readonly IHmacProvider fakeHmacProvider;

        private RijndaelEncryptionProvider rijndaelEncryptionProvider;

        private DefaultHmacProvider defaultHmacProvider;

        public CookieBasedSessionsFixture()
        {
            this.fakeEncryptionProvider = A.Fake<IEncryptionProvider>();
            this.fakeHmacProvider = A.Fake<IHmacProvider>();
            this.cookieStore = new CookieBasedSessions(this.fakeEncryptionProvider, this.fakeHmacProvider, new Fakes.FakeObjectSerializer());

            this.rijndaelEncryptionProvider = new RijndaelEncryptionProvider(new PassphraseKeyGenerator("password", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000));
            this.defaultHmacProvider = new DefaultHmacProvider(new PassphraseKeyGenerator("anotherpassword", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000));
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
            var hooks = A.Fake<IPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);

            CookieBasedSessions.Enable(hooks, new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider));

            beforePipeline.PipelineDelegates.Count().ShouldEqual(1);
            afterPipeline.PipelineItems.Count().ShouldEqual(1);
        }

        [Fact]
        public void Should_only_not_add_response_cookie_if_it_has_not_changed()
        {
            var beforePipeline = new BeforePipeline();
            var afterPipeline = new AfterPipeline();
            var hooks = A.Fake<IPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);
            CookieBasedSessions.Enable(hooks, new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider)).WithSerializer(new Fakes.FakeObjectSerializer());
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
            var hooks = A.Fake<IPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);
            CookieBasedSessions.Enable(hooks, new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider)).WithSerializer(new Fakes.FakeObjectSerializer());
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
            var fakeFormatter = A.Fake<IObjectSerializer>();
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
            var fakeFormatter = A.Fake<IObjectSerializer>();
            var store = new CookieBasedSessions(this.fakeEncryptionProvider, this.fakeHmacProvider, fakeFormatter);

            store.Save(session, response);

            A.CallTo(() => fakeFormatter.Serialize("value1")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_formatter_when_using_formatter_selector()
        {
            var beforePipeline = new BeforePipeline();
            var afterPipeline = new AfterPipeline();
            var hooks = A.Fake<IPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);
            var fakeFormatter = A.Fake<IObjectSerializer>();
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey1=value1")).Returns("key1=value1;");
            CookieBasedSessions.Enable(hooks, new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider)).WithSerializer(fakeFormatter);
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
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultObjectSerializer());
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
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultObjectSerializer());
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
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultObjectSerializer());
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
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultObjectSerializer());
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
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultObjectSerializer());
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
            var store = new CookieBasedSessions(this.rijndaelEncryptionProvider, this.defaultHmacProvider, new DefaultObjectSerializer());
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