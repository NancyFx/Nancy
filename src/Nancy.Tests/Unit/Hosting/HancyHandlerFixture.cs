namespace Nancy.Tests.Unit.Hosting
{
    using System.Web;
    using FakeItEasy;
    using Nancy.Hosting;

    public class HancyHandlerFixture
    {
        private readonly NancyHandler handler;
        private readonly HttpContextBase context;
        private readonly HttpRequestBase request;
        private readonly HttpResponseBase response;

        public HancyHandlerFixture()
        {
            this.context = A.Fake<HttpContextBase>();
            this.response = A.Fake<HttpResponseBase>();
            this.request = A.Fake<HttpRequestBase>();

            A.CallTo(() => this.context.Request).Returns(this.request);
            A.CallTo(() => this.context.Response).Returns(this.response);
        }
    }
}