namespace Nancy.Tests.Specifications
{
    using System.Net;
    using Machine.Specifications;

    [Subject("Handling a HEAD request")]
    public class when_head_request_matched_existing_route : RequestSpec
    {
        Establish context = () =>
            request = ManufactureHEADRequestForRoute("/");

        Because of = () => 
            response = engine.HandleRequest(request);

        It should_set_status_code_to_ok = () =>
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_set_content_type_to_text_html = () =>
            response.ContentType.ShouldEqual("text/html");

    	private It should_set_blank_content = () =>
    	    GetStringContentsFromResponse(response).ShouldBeEmpty();
    }

    [Subject("Handling a GET request")]
    public class when_head_request_does_not_matched_existing_route : RequestSpec
    {
        Establish context = () =>
            request = ManufactureGETRequestForRoute("/invalid");

        Because of = () =>
            response = engine.HandleRequest(request);

        It should_set_status_code_to_not_found = () =>
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);

        It should_set_content_type_to_text_html = () =>
            response.ContentType.ShouldEqual("text/html");

        It should_set_blank_content = () =>
			GetStringContentsFromResponse(response).ShouldBeEmpty();
	}
}