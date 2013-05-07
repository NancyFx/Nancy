namespace Nancy.Tests.Specifications
{
    using Machine.Specifications;

    using Nancy.Tests.Extensions;

    [Subject("Handling a HEAD request")]
    public class when_head_request_matched_existing_route : RequestSpec
    {
        private Establish context = () =>
            request = ManufactureHEADRequestForRoute("/");

        private Because of = () =>
            response = engine.HandleRequest(request).Response;

        private It should_set_blank_content = () =>
            response.GetStringContentsFromResponse().ShouldBeEmpty();

        private It should_set_content_type_to_text_html = () =>
            response.ContentType.ShouldEqual("text/html");

        private It should_set_status_code_to_ok = () =>
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
    }

    [Subject("Handling a HEAD request")]
    public class when_head_request_does_not_matched_existing_route : RequestSpec
    {
        private Establish context = () =>
            request = ManufactureHEADRequestForRoute("/invalid");

        private Because of = () =>
            response = engine.HandleRequest(request).Response;

        private It should_set_status_code_to_not_found = () =>
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
    }
}