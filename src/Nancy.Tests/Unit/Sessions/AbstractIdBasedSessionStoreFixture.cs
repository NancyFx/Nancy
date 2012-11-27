namespace Nancy.Tests.Unit.Sessions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FakeItEasy;
    using Nancy.Cryptography;
    using Nancy.Session;
    using Xunit;
    using Nancy.Tests.Fakes;

    public class AbstractIdBasedSessionStoreFixture
    {
        private readonly AbstractIdBasedSessionStore provider;
        private readonly IEncryptionProvider fakeEncryptionProvider;
        private readonly IHmacProvider fakeHmacProvider;
        private readonly NancyContext context;

        public AbstractIdBasedSessionStoreFixture()
        {
            fakeEncryptionProvider = A.Fake<IEncryptionProvider>();
            fakeHmacProvider = A.Fake<IHmacProvider>();
            provider = new FakeIdBasedSessionStore(new CryptographyConfiguration(fakeEncryptionProvider, fakeHmacProvider));
            context = new NancyContext();
            context.SessionStore = provider;
            context.Request = new Request("GET", "/", "http");
            context.Response = new Response();
        }

        [Fact]
        public void Should_generate_a_session_id()
        {
            var session = new Dictionary<string, object>();
            session.Add("Hello", "World");

            provider.SaveSession(context, session);

            var cookie = context.Response.Cookies.FirstOrDefault();
            cookie.ShouldNotBeNull();
            cookie.Name.ShouldEqual(AbstractIdBasedSessionStore.CookieName);
        }

        [Fact]
        public void Should_create_correctly_configured_cookie()
        {
            var session = new Dictionary<string, object>();
            session.Add("Hello", "World");

            provider.SaveSession(context, session);

            var cookie = context.Response.Cookies.FirstOrDefault();
            cookie.ShouldNotBeNull();
            cookie.Name.ShouldEqual(AbstractIdBasedSessionStore.CookieName);
            cookie.Expires.ShouldBeNull();
            cookie.Path.ShouldBeNull();
            cookie.Domain.ShouldBeNull();
            cookie.HttpOnly.ShouldBeTrue();
        }

        [Fact]
        public void Should_encrypt_and_hash_the_session_id()
        {
            var hashBytes = Encoding.UTF8.GetBytes("HASH");
            var session = new Dictionary<string, object>();
            session.Add("Hello", "World");
            A.CallTo(() => fakeEncryptionProvider.Encrypt(A<string>.Ignored)).Returns("EncryptedValue");
            A.CallTo(() => fakeHmacProvider.GenerateHmac("EncryptedValue")).Returns(hashBytes);

            provider.SaveSession(context, session);

            var cookie = context.Response.Cookies.FirstOrDefault();
            cookie.ShouldNotBeNull();
            cookie.Name.ShouldEqual(AbstractIdBasedSessionStore.CookieName);
            cookie.Value.ShouldEqual(String.Format("{0}{1}", Convert.ToBase64String(hashBytes), "EncryptedValue"));
        }

        [Fact]
        public void Should_not_create_a_new_session_id_if_one_exists()
        {
            var hashBytes = Encoding.UTF8.GetBytes("HASH");
            var session = new Dictionary<string, object>();
            context.Request.Cookies.Add(AbstractIdBasedSessionStore.CookieName, String.Format("{0}{1}", Convert.ToBase64String(hashBytes), "12345"));
            session.Add("Hello", "World");

            A.CallTo(() => fakeEncryptionProvider.Decrypt("12345")).Returns("54321");
            A.CallTo(() => fakeHmacProvider.HmacLength).Returns(4);
            A.CallTo(() => fakeHmacProvider.GenerateHmac("12345")).Returns(hashBytes);

            provider.SaveSession(context, session);

            var cookie = context.Response.Cookies.FirstOrDefault();
            cookie.ShouldBeNull();
        }

        [Fact]
        public void Should_generate_a_new_session_id_when_invalid_id_is_passed()
        {
            var hashBytes = Encoding.UTF8.GetBytes("HASH");
            var session = new Dictionary<string, object>();
            context.Request.Cookies.Add(AbstractIdBasedSessionStore.CookieName, String.Format("{0}{1}", Convert.ToBase64String(hashBytes.Reverse().ToArray()), "xxxxx"));
            session.Add("Hello", "World");

            A.CallTo(() => fakeEncryptionProvider.Encrypt(A<string>.Ignored)).Returns("EncryptedValue");
            A.CallTo(() => fakeEncryptionProvider.Decrypt(A<string>.Ignored)).Returns("12345");
            A.CallTo(() => fakeHmacProvider.HmacLength).Returns(4);
            A.CallTo(() => fakeHmacProvider.GenerateHmac(A<string>.Ignored)).Returns(hashBytes);

            provider.SaveSession(context, session);

            var cookie = context.Response.Cookies.FirstOrDefault();
            cookie.ShouldNotBeNull();
            cookie.Name.ShouldEqual(AbstractIdBasedSessionStore.CookieName);
            cookie.Value.ShouldEqual(String.Format("{0}{1}", Convert.ToBase64String(hashBytes), "EncryptedValue"));
        }

        [Fact]
        public void Should_return_false_when_no_cookie_present()
        {
            IDictionary<string,object> session;
            
            var result = provider.TryLoadSession(context.Request, out session);

            result.ShouldBeFalse();
            session.ShouldNotBeNull();
            session.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_be_able_to_load_a_valid_session()
        {
            IDictionary<string, object> session;
            var hashBytes = Encoding.UTF8.GetBytes("HASH");
            context.Request.Cookies.Add(AbstractIdBasedSessionStore.CookieName, String.Format("{0}{1}", Convert.ToBase64String(hashBytes), "12345"));
            ((FakeIdBasedSessionStore)provider).Items.Add("54321", new Dictionary<string, object> { { "Hello", "World" } });

            A.CallTo(() => fakeEncryptionProvider.Decrypt("12345")).Returns("54321");
            A.CallTo(() => fakeHmacProvider.HmacLength).Returns(4);
            A.CallTo(() => fakeHmacProvider.GenerateHmac("12345")).Returns(hashBytes);

            var result = provider.TryLoadSession(context.Request, out session);

            result.ShouldBeTrue();
            session.ShouldNotBeNull();
            session.Count.ShouldEqual(1);
            session["Hello"].ShouldEqual("World");
        }

        [Fact]
        public void Should_return_false_for_invalid_session_ids()
        {
            IDictionary<string, object> session;
            var hashBytes = Encoding.UTF8.GetBytes("HASH");
            context.Request.Cookies.Add(AbstractIdBasedSessionStore.CookieName, String.Format("{0}{1}", Convert.ToBase64String(hashBytes), "12345"));

            A.CallTo(() => fakeEncryptionProvider.Decrypt("12345")).Returns("54321");
            A.CallTo(() => fakeHmacProvider.HmacLength).Returns(4);
            A.CallTo(() => fakeHmacProvider.GenerateHmac("12345")).Returns(hashBytes);

            var result = provider.TryLoadSession(context.Request, out session);

            result.ShouldBeFalse();
            session.ShouldNotBeNull();
            session.Count.ShouldEqual(0);
        }
    
    }
}