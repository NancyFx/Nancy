namespace Nancy.Tests.Unit.Hosting
{
    using System;
    using System.IO;
    using System.Web;
    using Cookies;
    using FakeItEasy;
    using Nancy.Hosting;
    using Xunit;

    public class HancyHandlerFixture
    {
        private readonly NancyHandler handler;
        private readonly HttpContextBase context;
        private readonly HttpRequestBase request;
        private readonly HttpResponseBase response;
        private readonly INancyEngine engine;

        public HancyHandlerFixture()
        {
            this.context = A.Fake<HttpContextBase>();
            this.request = A.Fake<HttpRequestBase>();
            this.response = A.Fake<HttpResponseBase>();
            this.engine = A.Fake<INancyEngine>();
            this.handler = new NancyHandler(engine);

            A.CallTo(() => this.context.Request).Returns(this.request);
            A.CallTo(() => this.context.Response).Returns(this.response);
            A.CallTo(() => this.response.OutputStream).Returns(new MemoryStream());
        }

        [Fact]
        public void Should_output_the_responses_cookies()
        {
            var cookie1 = A.Fake<INancyCookie>();
            var cookie2 = A.Fake<INancyCookie>();
            var r = new Response();
            r.AddCookie(cookie1).AddCookie(cookie2);

            A.CallTo(() => cookie1.ToString()).Returns("the first cookie");
            A.CallTo(() => cookie2.ToString()).Returns("the second cookie");
            
            SetupRequestProcess(r);
            
            this.handler.ProcessRequest(context);

            A.CallTo(() => this.response.AddHeader("Set-Cookie", "the first cookie")).MustHaveHappened();
            A.CallTo(() => this.response.AddHeader("Set-Cookie", "the second cookie")).MustHaveHappened();
        }

        private void SetupRequestProcess(Response response)
        {
            A.CallTo(() => this.request.AppRelativeCurrentExecutionFilePath).Returns("~/about");
            A.CallTo(() => this.request.Url).Returns(new Uri("http://ihatedummydata.com/about"));
            A.CallTo(() => this.request.HttpMethod).Returns("GET");
            A.CallTo(() => this.engine.HandleRequest(A<Request>.Ignored.Argument)).Returns(response);
        }
    }
}