namespace Nancy.Tests.Unit.Responses
{
    using Nancy.Responses;

    using Xunit;

    public class RedirectResponseFixture
    {
        [Fact]
        public void Permanent_redirect_should_return_status_code_301()
        {
            var response = new RedirectResponse("/", RedirectResponse.RedirectType.Permanent);
            response.StatusCode.ShouldEqual(HttpStatusCode.MovedPermanently);
        }

        [Fact]
        public void Temporary_redirect_should_return_status_code_307()
        {
            var response = new RedirectResponse("/", RedirectResponse.RedirectType.Temporary);
            response.StatusCode.ShouldEqual(HttpStatusCode.TemporaryRedirect);
        }

        [Fact]
        public void SeeOther_redirect_should_return_status_code_303()
        {
            var response = new RedirectResponse("/", RedirectResponse.RedirectType.SeeOther);
            response.StatusCode.ShouldEqual(HttpStatusCode.SeeOther);
        }

        [Fact]
        public void Default_redirect_should_return_status_code_303()
        {
            var response = new RedirectResponse("/");
            response.StatusCode.ShouldEqual(HttpStatusCode.SeeOther);
        }
    }
}
