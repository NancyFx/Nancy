namespace Nancy.Tests.Unit
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;    
    using System.Web;
    using FakeItEasy;
    using Sessions;
    using Xunit;

    public class CookieSessionStoreFixture
    {
        private readonly IEncryption encryption;
        private readonly CookieSessionStore cookieStore;
        private readonly string passphrase;
        private readonly byte[] salt;

        public CookieSessionStoreFixture()
        {
            passphrase = CookieSessionStore.Passphrase = "the passphrase";
            CookieSessionStore.Salt = "the salt";
            //must kill someone
            salt = (byte[]) typeof (CookieSessionStore).GetField("salt", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            this.encryption = A.Fake<IEncryption>();
            cookieStore = new CookieSessionStore(encryption);
        }

        [Fact]
        public void Should_save_nothing_if_the_session_is_null()
        {
            var response = new Response();
            new CookieSessionStore().Save(null, response);
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
            
            A.CallTo(() => encryption.Encrypt("key1=val1;key2=val2;", passphrase,salt)).Returns("encrypted=key1=val1;key2=val2;");

            cookieStore.Save(session, response);            
            response.Cookies.Count.ShouldEqual(1);
            response.Cookies.First().Name.ShouldEqual(CookieSessionStore.CookieName);
            response.Cookies.First().Value.ShouldEqual("encrypted=key1=val1;key2=val2;");
            response.Cookies.First().Expires.ShouldBeNull();
            response.Cookies.First().Path.ShouldBeNull();
            response.Cookies.First().Domain.ShouldBeNull();
        }

        [Fact]
        public void Should_saves_url_safe_keys_and_values()
        {
            var response = new Response();
            var session = new Session();
            session["key 1"] = "val=1";
            A.CallTo(() => encryption.Encrypt("key+1=val%3d1;", passphrase, salt)).Returns("encryptedkey+1=val%3d1;");
            cookieStore.Save(session, response);
            response.Cookies.First().Value.ShouldEqual("encryptedkey+1=val%3d1;");
        }

        [Fact]
        public void Should_load_an_empty_session_if_no_session_cookie_exists()
        {
            //Given
            var request = CreateRequest(null);

            // Then
            cookieStore.Load(request).Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_load_a_single_valued_session()
        {
            // Given
            var request = CreateRequest("encryptedkey1=value1");
            A.CallTo(() => encryption.Decrypt("encryptedkey1=value1", passphrase, salt)).Returns("key1=value1;");
            
            // When
            var session = cookieStore.Load(request);

            // Then
            session.Count.ShouldEqual(1);
            session["key1"].ShouldEqual("value1");
        }

        [Fact]
        public void Should_load_a_multi_valued_session()
        {
            // Given
            var request = CreateRequest("encryptedkey1=value1;key2=value2");
            A.CallTo(() => encryption.Decrypt("encryptedkey1=value1;key2=value2", passphrase, salt)).Returns("key1=value1;key2=value2");

            // When           
            var session = cookieStore.Load(request);

            // Then
            session.Count.ShouldEqual(2);
            session["key1"].ShouldEqual("value1");
            session["key2"].ShouldEqual("value2");
        }

        [Fact]
        public void Should_load_properly_decode_the_url_safe_session()
        {
            // Given
            var request = CreateRequest("encryptedkey+1=val%3d1;");
            A.CallTo(() => encryption.Decrypt("encryptedkey+1=val%3d1;", passphrase, salt)).Returns("key+1=val%3d1;");
            // When
            var session = cookieStore.Load(request);

            // Then
            session.Count.ShouldEqual(1);
            session["key 1"].ShouldEqual("val=1");            
        }

        private static Request CreateRequest(string sessionValue)
        {
            var headers = new Dictionary<string, IEnumerable<string>>(1);
            if (!string.IsNullOrEmpty(sessionValue))
            {
                headers.Add("cookie", new[]{CookieSessionStore.CookieName + "=" + HttpUtility.UrlEncode(sessionValue)});
            }
            return new Request("GET", "http://goku.power:9001/", headers, new MemoryStream(), new CookieSessionStore());
        }
    }
}