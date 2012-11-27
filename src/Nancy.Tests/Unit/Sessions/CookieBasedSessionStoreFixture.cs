namespace Nancy.Tests.Unit.Sessions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FakeItEasy;
    using Nancy.Cryptography;
    using Nancy.Helpers;
    using Nancy.IO;
    using Nancy.Session;
    using Xunit;

    public class CookieBasedSessionStoreFixture
    {
        private const string ValidData = "VgPJvXYwcXkn0gxDvg84tsfV9F5he1ZxhjsTZK1UZHVqWk7izPd9XsWnuFMrtQNRJEfyiqU2J7tAZDQvdKjQij9wUO6mOTCyZ7HPHK/pEnkgDFMXbHDctGQZSbb2WZZxola+Q3nP2tlQ+Tx//N6YyK7BwpsNPrvyHAvU1z5YzHfPT6HEowIl8mz/uUL6o+FME/Goi7RN2getMeYaPCs0fJkiMCAomnnagAy4aXN0Ak/p7Y3K/kpNAS6PvNu4aok0zVpfo1utP84GyyLomfr4urmDNFIe8PBVoKhuomxjsUOddaarHqqmN3PXOp15SPXPDxEKfpuLzhmqXnStiB8nH9qMBYI/AuLHMckDzkeESH5rQ2q2+1RgCN82PujzGhhVnBMk95ZS9k9zKCvKQa2yzVkaHqwSESyOFboU89kLAEQ0h48dtoJ2FTBs9GjsL3Z4fGogeLwjIvP8I8JF39HI+9U3PC2KnicA/bgUL/Z1paDzZYTrqQS4QSyFgy4DOxYz";
        private const string ValidHmac = "un/5uJOoOAyn4AX8VU0HsGYYtr79A40TFF1wVqd/jDQ=";

        private readonly IEncryptionProvider fakeEncryptionProvider;
        private readonly CookieBasedSessionStore cookieStore;
        private readonly IHmacProvider fakeHmacProvider;
        private readonly NancyContext context;

        private RijndaelEncryptionProvider rijndaelEncryptionProvider;
        private DefaultHmacProvider defaultHmacProvider;

        public CookieBasedSessionStoreFixture()
        {
            this.fakeEncryptionProvider = A.Fake<IEncryptionProvider>();
            this.fakeHmacProvider = A.Fake<IHmacProvider>();
            this.cookieStore = new CookieBasedSessionStore(new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider), new Fakes.FakeObjectSerializer());

            this.rijndaelEncryptionProvider = new RijndaelEncryptionProvider(new PassphraseKeyGenerator("password", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000));
            this.defaultHmacProvider = new DefaultHmacProvider(new PassphraseKeyGenerator("anotherpassword", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 1000));

            this.context = new NancyContext();
            this.context.SessionStore = cookieStore;
            this.context.Request = new Request("GET", "/", "http");
            this.context.Response = new Response();
        }

        [Fact]
        public void Should_save_the_session_cookie()
        {
            var items = new Dictionary<string, object> { { "key1", "val1" } };
            items["key2"] = "val2";
            A.CallTo(() => this.fakeEncryptionProvider.Encrypt("key1=val1;key2=val2;")).Returns("encrypted=key1=val1;key2=val2;");

            cookieStore.SaveSession(context, items);

            context.Response.Cookies.Count.ShouldEqual(1);
            var cookie = context.Response.Cookies.First();
            cookie.Name.ShouldEqual(CookieBasedSessionStore.GetCookieName());
            cookie.Value.ShouldEqual("encrypted=key1=val1;key2=val2;");
            cookie.Expires.ShouldBeNull();
            cookie.Path.ShouldBeNull();
            cookie.Domain.ShouldBeNull();
        }

        [Fact]
        public void Should_save_cookie_as_http_only()
        {
            var response = new Response();
            var session = new Dictionary<string, object> { { "key 1", "val=1" } };
            A.CallTo(() => this.fakeEncryptionProvider.Encrypt("key+1=val%3d1;")).Returns("encryptedkey+1=val%3d1;");

            cookieStore.SaveSession(context, session);

            context.Response.Cookies.First().HttpOnly.ShouldEqual(true);
        }

        [Fact]
        public void Should_saves_url_safe_keys_and_values()
        {
            var response = new Response();
            var session = new Dictionary<string, object> { { "key 1", "val=1" } };
            A.CallTo(() => this.fakeEncryptionProvider.Encrypt("key+1=val%3d1;")).Returns("encryptedkey+1=val%3d1;");

            cookieStore.SaveSession(context, session);

            context.Response.Cookies.First().Value.ShouldEqual("encryptedkey+1=val%3d1;");
        }

        [Fact]
        public void Should_return_false_if_no_session_cookie_exists()
        {
            var request = CreateRequest(null);

            IDictionary<string, object> items;
            var result = cookieStore.TryLoadSession(request, out items);

            result.ShouldBeFalse();
            items.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_load_a_single_valued_session()
        {
            var request = CreateRequest("encryptedkey1=value1");
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey1=value1")).Returns("key1=value1;");

            IDictionary<string, object> session;
            var result = cookieStore.TryLoadSession(request, out session);

            result.ShouldBeTrue();
            session.Count.ShouldEqual(1);
            session["key1"].ShouldEqual("value1");
        }

        [Fact]
        public void Should_load_a_multi_valued_session()
        {
            var request = CreateRequest("encryptedkey1=value1;key2=value2");
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey1=value1;key2=value2")).Returns("key1=value1;key2=value2");

            IDictionary<string, object> session;
            var result = cookieStore.TryLoadSession(request, out session);

            result.ShouldBeTrue();
            session.Count.ShouldEqual(2);
            session["key1"].ShouldEqual("value1");
            session["key2"].ShouldEqual("value2");
        }

        [Fact]
        public void Should_load_properly_decode_the_url_safe_session()
        {
            var request = CreateRequest("encryptedkey+1=val%3d1;");
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey+1=val%3d1;")).Returns("key+1=val%3d1;");

            IDictionary<string, object> session;
            var result = cookieStore.TryLoadSession(request, out session);

            result.ShouldBeTrue();
            session.Count.ShouldEqual(1);
            session["key 1"].ShouldEqual("val=1");
        }

        [Fact]
        public void Should_call_formatter_on_load()
        {
            var fakeFormatter = A.Fake<IObjectSerializer>();
            A.CallTo(() => this.fakeEncryptionProvider.Decrypt("encryptedkey1=value1")).Returns("key1=value1;");
            var store = new CookieBasedSessionStore(new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider), fakeFormatter);
            var request = CreateRequest("encryptedkey1=value1", false);

            IDictionary<string, object> session;
            var result = store.TryLoadSession(request, out session);

            A.CallTo(() => fakeFormatter.Deserialize("value1")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_call_the_formatter_on_save()
        {
            var ctx = new NancyContext();
            ctx.Response = new Response();
            var session = new Dictionary<string, object>();
            session["key1"] = "value1";
            var fakeFormatter = A.Fake<IObjectSerializer>();
            var store = new CookieBasedSessionStore(new CryptographyConfiguration(this.fakeEncryptionProvider, this.fakeHmacProvider), fakeFormatter);

            store.SaveSession(ctx, session);

            A.CallTo(() => fakeFormatter.Serialize("value1")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_be_able_to_save_a_complex_object_to_session()
        {
            var ctx = new NancyContext();
            ctx.Response = new Response();
            var session = new Dictionary<string, object>();
            var payload = new DefaultSessionObjectFormatterFixture.Payload(27, true, "Test string");
            var store = new CookieBasedSessionStore(new CryptographyConfiguration(this.rijndaelEncryptionProvider, this.defaultHmacProvider), new DefaultObjectSerializer());
            session["testObject"] = payload;

            store.SaveSession(ctx, session);

            ctx.Response.Cookies.Count.ShouldEqual(1);
            var cookie = ctx.Response.Cookies.First();
            cookie.Name.ShouldEqual(CookieBasedSessionStore.GetCookieName());
            cookie.Value.ShouldNotBeNull();
            cookie.Value.ShouldNotBeEmpty();
        }

        [Fact]
        public void Should_be_able_to_load_an_object_previously_saved_to_session()
        {
            var ctx = new NancyContext();
            ctx.Response = new Response();
            var session = new Dictionary<string, object>();
            var payload = new DefaultSessionObjectFormatterFixture.Payload(27, true, "Test string");
            var store = new CookieBasedSessionStore(new CryptographyConfiguration(this.rijndaelEncryptionProvider, this.defaultHmacProvider), new DefaultObjectSerializer());
            session["testObject"] = payload;
            store.SaveSession(ctx, session);
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(Helpers.HttpUtility.UrlEncode(ctx.Response.Cookies.First().Name), Helpers.HttpUtility.UrlEncode(ctx.Response.Cookies.First().Value));

            IDictionary<string, object> newSession;
            var result = store.TryLoadSession(request, out newSession);

            result.ShouldBeTrue();
            newSession["testObject"].ShouldEqual(payload);
        }

        [Fact]
        public void Should_encrypt_data()
        {
            var session = new Dictionary<string, object> { { "key1", "val1" } };
            session["key2"] = "val2";

            cookieStore.SaveSession(context, session);

            A.CallTo(() => this.fakeEncryptionProvider.Encrypt(A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_generate_hmac()
        {
            var session = new Dictionary<string, object> { { "key1", "val1" } };
            session["key2"] = "val2";

            cookieStore.SaveSession(context, session);

            A.CallTo(() => this.fakeHmacProvider.GenerateHmac(A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_load_valid_test_data()
        {
            IDictionary<string, object> session;
            var inputValue = ValidHmac + ValidData;
            inputValue = HttpUtility.UrlEncode(inputValue);
            var store = new CookieBasedSessionStore(new CryptographyConfiguration(this.rijndaelEncryptionProvider, this.defaultHmacProvider), new DefaultObjectSerializer());
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(CookieBasedSessionStore.GetCookieName(), inputValue);

            var result = store.TryLoadSession(request, out session);

            result.ShouldBeTrue();
            session.Count.ShouldEqual(1);
            session.First().Value.ShouldBeOfType(typeof(DefaultSessionObjectFormatterFixture.Payload));
        }

        [Fact]
        public void Should_return_blank_session_if_hmac_changed()
        {
            IDictionary<string, object> session;
            var inputValue = "b" + ValidHmac.Substring(1) + ValidData;
            inputValue = HttpUtility.UrlEncode(inputValue);
            var store = new CookieBasedSessionStore(new CryptographyConfiguration(this.rijndaelEncryptionProvider, this.defaultHmacProvider), new DefaultObjectSerializer());
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(CookieBasedSessionStore.GetCookieName(), inputValue);

            var result = store.TryLoadSession(request, out session);

            result.ShouldBeTrue();
            session.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_blank_session_if_hmac_missing()
        {
            IDictionary<string, object> session;
            var inputValue = ValidData;
            inputValue = HttpUtility.UrlEncode(inputValue);
            var store = new CookieBasedSessionStore(new CryptographyConfiguration(this.rijndaelEncryptionProvider, this.defaultHmacProvider), new DefaultObjectSerializer());
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(CookieBasedSessionStore.GetCookieName(), inputValue);

            var result = store.TryLoadSession(request, out session);

            session.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_blank_session_if_encrypted_data_modified()
        {
            IDictionary<string, object> session;
            var inputValue = ValidHmac + ValidData.Substring(0, ValidData.Length - 1) + "Z";
            inputValue = HttpUtility.UrlEncode(inputValue);
            var store = new CookieBasedSessionStore(new CryptographyConfiguration(this.rijndaelEncryptionProvider, this.defaultHmacProvider), new DefaultObjectSerializer());
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(CookieBasedSessionStore.GetCookieName(), inputValue);

            var result = store.TryLoadSession(request, out session);

            session.Count.ShouldEqual(0);
        }

        private Request CreateRequest(string sessionValue, bool load = true)
        {
            var headers = new Dictionary<string, IEnumerable<string>>(1);

            if (!string.IsNullOrEmpty(sessionValue))
            {
                headers.Add("cookie", new[] { CookieBasedSessionStore.GetCookieName() + "=" + HttpUtility.UrlEncode(sessionValue) });
            }

            var request = new Request("GET", "http://goku.power:9001/", headers, CreateRequestStream(), "http");

            if (load)
            {
                IDictionary<string, object> items;
                cookieStore.TryLoadSession(request, out items);
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
