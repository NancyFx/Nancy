namespace Nancy.Tests.Functional.Modules
{
    using Nancy.Cookies;

    public class CookieModule : NancyModule
    {
        public CookieModule()
        {
            Get("/setcookie", args =>
            {
                const string value = "HakLqr1OEdi+kQ/s92Rzz9hV1w/vzGZKqWeMQRHRJlwhbbgP87UELJZlYDfbVVLo";

                var cookie = new NancyCookie("testcookie", value);

                var response = new Response();
                response.WithCookie(cookie);
                response.StatusCode = HttpStatusCode.OK;

                return response;
            });

            Get("/getcookie", args =>
            {
                const string value = "HakLqr1OEdi+kQ/s92Rzz9hV1w/vzGZKqWeMQRHRJlwhbbgP87UELJZlYDfbVVLo";

                var cookie = Context.Request.Cookies["testcookie"];

                return string.Equals(cookie, value) ?
                    HttpStatusCode.OK :
                    HttpStatusCode.InternalServerError;
            });
        }
    }
}
