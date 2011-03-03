namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using FakeItEasy;
    using Nancy.Bootstrapper;
    using Nancy.Tests.Unit.Sessions;

    using Session;
    using Xunit;

    public class CookieBasedSessionsFixture
    {
        private readonly IEncryptionProvider encryptionProvider;
        private readonly Nancy.Session.CookieBasedSessions cookieStore;

        public CookieBasedSessionsFixture()
        {
            this.encryptionProvider = A.Fake<IEncryptionProvider>();
            this.cookieStore = new Nancy.Session.CookieBasedSessions(this.encryptionProvider, "the passphrase", "the salt", new DefaultSessionObjectFormatter());
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
            A.CallTo(() => this.encryptionProvider.Encrypt("key1=val1;key2=val2;", A<string>.Ignored, A<byte[]>.Ignored)).Returns("encrypted=key1=val1;key2=val2;");

            cookieStore.Save(session, response);

            response.Cookies.Count.ShouldEqual(1);
            var cookie = response.Cookies.First();
            cookie.Name.ShouldEqual(Nancy.Session.CookieBasedSessions.GetCookieName());
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
            A.CallTo(() => this.encryptionProvider.Encrypt("key+1=val%3d1;", A<string>.Ignored, A<byte[]>.Ignored)).Returns("encryptedkey+1=val%3d1;");

            cookieStore.Save(session, response);

            response.Cookies.First().HttpOnly.ShouldEqual(true);
        }

        [Fact]
        public void Should_saves_url_safe_keys_and_values()
        {
            var response = new Response();
            var session = new Session();
            session["key 1"] = "val=1";
            A.CallTo(() => this.encryptionProvider.Encrypt("key+1=val%3d1;", A<string>.Ignored, A<byte[]>.Ignored)).Returns("encryptedkey+1=val%3d1;");

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
            A.CallTo(() => this.encryptionProvider.Decrypt("encryptedkey1=value1", A<string>.Ignored, A<byte[]>.Ignored)).Returns("key1=value1;");

            var session = cookieStore.Load(request);

            session.Count.ShouldEqual(1);
            session["key1"].ShouldEqual("value1");
        }

        [Fact]
        public void Should_load_a_multi_valued_session()
        {
            var request = CreateRequest("encryptedkey1=value1;key2=value2");
            A.CallTo(() => this.encryptionProvider.Decrypt("encryptedkey1=value1;key2=value2", A<string>.Ignored, A<byte[]>.Ignored)).Returns("key1=value1;key2=value2");

            var session = cookieStore.Load(request);

            session.Count.ShouldEqual(2);
            session["key1"].ShouldEqual("value1");
            session["key2"].ShouldEqual("value2");
        }

        [Fact]
        public void Should_load_properly_decode_the_url_safe_session()
        {
            var request = CreateRequest("encryptedkey+1=val%3d1;");
            A.CallTo(() => this.encryptionProvider.Decrypt("encryptedkey+1=val%3d1;", A<string>.Ignored, A<byte[]>.Ignored)).Returns("key+1=val%3d1;");

            var session = cookieStore.Load(request);

            session.Count.ShouldEqual(1);
            session["key 1"].ShouldEqual("val=1");
        }

        [Fact]
        public void Should_throw_if_salt_too_short()
        {
            var exception = Record.Exception(() => new CookieBasedSessions(this.encryptionProvider, "pass", "short", A.Fake<ISessionObjectFormatter>()));

            exception.ShouldBeOfType(typeof(ArgumentException));
        }

        [Fact]
        public void Should_add_pre_and_post_hooks_when_enabled()
        {
            var beforePipeline = new BeforePipeline();
            var afterPipeline = new AfterPipeline();
            var hooks = A.Fake<IApplicationPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);

            CookieBasedSessions.Enable(hooks, encryptionProvider, "this passphrase", "this is a salt");

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
            CookieBasedSessions.Enable(hooks, encryptionProvider, "this passphrase", "this is a salt");
            var request = CreateRequest("encryptedkey1=value1");
            A.CallTo(() => this.encryptionProvider.Decrypt("encryptedkey1=value1", A<string>.Ignored, A<byte[]>.Ignored)).Returns("key1=value1;");
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
            CookieBasedSessions.Enable(hooks, encryptionProvider, "this passphrase", "this is a salt");
            var request = CreateRequest("encryptedkey1=value1");
            A.CallTo(() => this.encryptionProvider.Decrypt("encryptedkey1=value1", A<string>.Ignored, A<byte[]>.Ignored)).Returns("key1=value1;");
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
            A.CallTo(() => this.encryptionProvider.Decrypt("encryptedkey1=value1", A<string>.Ignored, A<byte[]>.Ignored)).Returns("key1=value1;");
            var store = new Nancy.Session.CookieBasedSessions(this.encryptionProvider, "the passphrase", "the salt", fakeFormatter);
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
            var store = new Nancy.Session.CookieBasedSessions(this.encryptionProvider, "the passphrase", "the salt", fakeFormatter);

            store.Save(session, response);

            A.CallTo(() => fakeFormatter.Serialize("value1")).MustHaveHappened(Repeated.Exactly.Once);
        }


        [Fact]
        public void Should_be_able_to_save_a_complex_object_to_session()
        {
            var response = new Response();
            var session = new Session(new Dictionary<string, object>());
            var payload = new DefaultSessionObjectFormatterFixture.Payload(27, true, "Test string");
            var store = new CookieBasedSessions(new DefaultEncryptionProvider(), "the passphrase", "the salt", new DefaultSessionObjectFormatter());
            session["testObject"] = payload;

            store.Save(session, response);

            response.Cookies.Count.ShouldEqual(1);
            var cookie = response.Cookies.First();
            cookie.Name.ShouldEqual(Nancy.Session.CookieBasedSessions.GetCookieName());
            cookie.Value.ShouldNotBeNull();
            cookie.Value.ShouldNotBeEmpty();
        }

        [Fact]
        public void Should_be_able_to_load_an_object_previously_saved_to_session()
        {
            var response = new Response();
            var session = new Session(new Dictionary<string, object>());
            var payload = new DefaultSessionObjectFormatterFixture.Payload(27, true, "Test string");
            var store = new CookieBasedSessions(new DefaultEncryptionProvider(), "the passphrase", "the salt", new DefaultSessionObjectFormatter());
            session["testObject"] = payload;
            store.Save(session, response);
            var request = new Request("GET", "/", "http");
            request.Cookies.Add(Helpers.HttpUtility.UrlEncode(response.Cookies.First().Name), Helpers.HttpUtility.UrlEncode(response.Cookies.First().Value));

            var result = store.Load(request);

            result["testObject"].ShouldEqual(payload);
        }

        private Request CreateRequest(string sessionValue, bool load = true)
        {
            var headers = new Dictionary<string, IEnumerable<string>>(1);

            if (!string.IsNullOrEmpty(sessionValue))
            {
                headers.Add("cookie", new[] { Nancy.Session.CookieBasedSessions.GetCookieName()+ "=" + HttpUtility.UrlEncode(sessionValue) });
            }

            var request = new Request("GET", "http://goku.power:9001/", headers, new MemoryStream(), "http");

            if (load)
            {
                cookieStore.Load(request);
            }

            return request;
        }
    }
}