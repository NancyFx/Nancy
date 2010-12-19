namespace Nancy.Tests.Unit.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Web;
    using FakeItEasy;
    using Nancy.Hosting;
    using Xunit;

    public class HancyHandlerFixture
    {
        private readonly HttpContextBase context;
        private readonly HttpRequestBase request;
        private readonly HttpResponseBase response;
        private readonly INancyEngine engine;

        public HancyHandlerFixture()
        {
            this.context = A.Fake<HttpContextBase>();
            this.response = A.Fake<HttpResponseBase>();            
            this.request = A.Fake<HttpRequestBase>();
            this.engine = A.Fake<INancyEngine>();

            A.CallTo(() => this.context.Request).Returns(this.request);
            A.CallTo(() => this.context.Response).Returns(this.response);
        }

        [Fact]
        public void Should_use_output_the_contents_to_the_response_stream()
        {
            SetUpFakeRequest();
            var expectedActionWasCalled = false;
            var stream = new MemoryStream();
            var action = new Action<Stream>(s =>
            {
                s.ShouldBeSameAs(stream);
                expectedActionWasCalled = true;
            });
            
            A.CallTo(() => this.engine.HandleRequest(A<IRequest>.Ignored.Argument)).Returns(new Response {Contents = action});
            A.CallTo(() => this.response.OutputStream).Returns(stream);

            new NancyHandler(engine).ProcessRequest(context);

            expectedActionWasCalled.ShouldBeTrue();
        }

        [Fact]
        public void Should_use_writefile_api_when_sending_back_a_file()
        {
            SetUpFakeRequest();
            A.CallTo(() => this.engine.HandleRequest(A<IRequest>.Ignored.Argument)).Returns(new Response { File = "/the/file" });

            new NancyHandler(engine).ProcessRequest(context);

            A.CallTo(() => this.response.WriteFile("/the/file")).MustHaveHappened();
        }

        private void SetUpFakeRequest()
        {
            A.CallTo(() => this.request.Url).Returns(new Uri("http://somesite.com/some/url"));
            A.CallTo(() => this.request.HttpMethod).Returns("GET");
            A.CallTo(() => this.request.Headers).Returns(new NameValueCollection());
        }
    }
}