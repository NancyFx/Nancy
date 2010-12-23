namespace Nancy.Tests.Unit
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Sessions;
    using Xunit;

    public class CookieSessionStoreFixture
    {
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
            new CookieSessionStore().Save(new Session(new Dictionary<string, object>{{"key","value"}}), response);
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
            new CookieSessionStore().Save(session, response);
            response.Cookies.Count.ShouldEqual(1);
            response.Cookies.First().Name.ShouldEqual(CookieSessionStore.CookieName);
            response.Cookies.First().Value.ShouldEqual("key1=val1;key2=val2;");
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
            new CookieSessionStore().Save(session, response);
            response.Cookies.First().Value.ShouldEqual("key+1=val%3d1;");
        }

        [Fact]
        public void Should_load_an_empty_session_if_no_session_cookie_exists()
        {
            var request = CreateRequest(null);
            new CookieSessionStore().Load(request).Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_load_a_single_valued_session()
        {
            var request = CreateRequest("key1=value1");
            var session = new CookieSessionStore().Load(request);
            session.Count.ShouldEqual(1);
            session["key1"].ShouldEqual("value1");
        }

        [Fact]
        public void Should_load_a_multi_valued_session()
        {
            var request = CreateRequest("key1=value1;key2=value2");
            var session = new CookieSessionStore().Load(request);
            session.Count.ShouldEqual(2);
            session["key1"].ShouldEqual("value1");
            session["key2"].ShouldEqual("value2");
        }

        [Fact]
        public void Should_load_properly_decode_the_url_safe_session()
        {
            var request = CreateRequest("key+1=val%3d1;");
            var session = new CookieSessionStore().Load(request);
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
            return new Request("GET", "http://goku.power:9001/", headers, new MemoryStream());
        }
    }
}