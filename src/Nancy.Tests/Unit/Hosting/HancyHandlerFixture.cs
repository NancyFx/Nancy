namespace Nancy.Tests.Unit.Hosting
{
    using System;
    using System.IO;
    using System.Web;
    using Cookies;
    using FakeItEasy;
    using Nancy.Hosting;
    using Sessions;
    using Xunit;

    public class HancyHandlerFixture
    {
        private readonly NancyHandler handler;
        private readonly HttpContextBase context;
        private readonly HttpRequestBase request;
        private readonly HttpResponseBase response;
        private readonly INancyEngine engine;        
        private readonly INancyApplication application;
        private readonly ISessionStore sessionStore;

        public HancyHandlerFixture()
        {
            this.context = A.Fake<HttpContextBase>();
            this.request = A.Fake<HttpRequestBase>();
            this.response = A.Fake<HttpResponseBase>();            
            this.engine = A.Fake<INancyEngine>();
            this.application = A.Fake<INancyApplication>();
            this.sessionStore = A.Fake<ISessionStore>();
            handler = new NancyHandler(engine, application);

            A.CallTo(() => this.context.Request).Returns(this.request);
            A.CallTo(() => this.context.Response).Returns(this.response);
            A.CallTo(() => this.response.OutputStream).Returns(new MemoryStream());
            A.CallTo(() => this.application.SessionStore).Returns(this.sessionStore);
        }

        [Fact]
        public void Should_load_the_session_into_the_request()
        {
            // Given
            var request = new Request("GET", "http://leto.dune/about");
            var response = new Response();
            var session = new Session();
            
            A.CallTo(() => sessionStore.Load(A<IRequest>.Ignored.Argument)).Returns(session);
            SetupRequestProcess();

            // When
            handler.ProcessRequest(context);

            // Then
            A.CallTo(() => this.engine.HandleRequest(A<IRequest>.That.Matches(r => r.Session == session).Argument)).MustHaveHappened();
        }

        [Fact]
        public void Should_store_the_session()
        {
            // Given
            var request = new Request("GET", "http://leto.dune/about");
            var response = new Response();
            var session = new Session();

            A.CallTo(() => sessionStore.Load(A<IRequest>.Ignored.Argument)).Returns(session);
            A.CallTo(() => this.engine.HandleRequest(A<IRequest>.Ignored.Argument)).Returns(response);
            SetupRequestProcess();

            // When           
            handler.ProcessRequest(context);

            // Then
            A.CallTo(() => this.sessionStore.Save(session, response)).MustHaveHappened();
        }


        [Fact]
        public void Should_output_the_responses_cookies()
        {
            var cookie1 = A.Fake<INancyCookie>();
            var cookie2 = A.Fake<INancyCookie>();
            var response = new Response();
            response.AddCookie(cookie1).AddCookie(cookie2);

            A.CallTo(() => cookie1.ToString()).Returns("the first cookie");
            A.CallTo(() => cookie2.ToString()).Returns("the second cookie");
            A.CallTo(() => this.engine.HandleRequest(A<IRequest>.Ignored.Argument)).Returns(response);

            SetupRequestProcess();
            
            handler.ProcessRequest(context);
            A.CallTo(() => this.response.AddHeader("Set-Cookie", "the first cookie")).MustHaveHappened();
            A.CallTo(() => this.response.AddHeader("Set-Cookie", "the second cookie")).MustHaveHappened();
        }

        private void SetupRequestProcess()
        {
            A.CallTo(() => this.request.Url).Returns(new Uri("http://ihatedummydata.com/about"));
            A.CallTo(() => this.request.HttpMethod).Returns("GET");
            
        }
    }
}