namespace Nancy.Tests.Specifications
{
    using System.Net;
    using Machine.Specifications;

    [Subject("Handling a GET request")]
    public class when_get_request_matched_existing_route : RequestSpec
    {
        Establish context = () =>
            request = ManufactureGETRequestForRoute("/");

        Because of = () => 
            response = engine.HandleRequest(request);

        It should_set_status_code_to_ok = () =>
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_set_content_type_to_text_html = () =>
            response.ContentType.ShouldEqual("text/html");

        It should_set_content = () =>
            response.Contents.ShouldEqual("Default get root");
    }

    [Subject("Handling a GET request")]
    public class when_get_request_does_not_matched_existing_route : RequestSpec
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
            response.Contents.ShouldEqual(string.Empty);
    }
}