using System;

namespace Nancy.Tests.Functional.Modules
{
    using Nancy.Cookies;

    public class CookieModule : LegacyNancyModule
    {
        public CookieModule()
        {
            Get["/setcookie"] = _ =>
            {
                const string value = "HakLqr1OEdi+kQ/s92Rzz9hV1w/vzGZKqWeMQRHRJlwhbbgP87UELJZlYDfbVVLo";

                var cookie = new NancyCookie("testcookie", value);

                var response = new Response();
                response.WithCookie(cookie);
                response.StatusCode = HttpStatusCode.OK;

                return response;
            };

            Get["/getcookie"] = _ =>
            {
                const string value = "HakLqr1OEdi+kQ/s92Rzz9hV1w/vzGZKqWeMQRHRJlwhbbgP87UELJZlYDfbVVLo";

                var cookie = Context.Request.Cookies["testcookie"];

                return String.Equals(cookie, value) ?
                    HttpStatusCode.OK :
                    HttpStatusCode.InternalServerError;
            };
        }
    }
}
