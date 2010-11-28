namespace Nancy
{
    using System.Net;

    public class Response
    {
        public string ContentType { get; set; }

        public string Contents { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public static implicit operator Response(HttpStatusCode statusCode)
        {
            return new Response { StatusCode = statusCode };
        }

        public static implicit operator Response(int statusCode)
        {
            return new Response { StatusCode = (HttpStatusCode)statusCode };
        }

        public static implicit operator Response(string contents)
        {
            return new Response { Contents = contents, ContentType = "text/html", StatusCode = HttpStatusCode.OK };
        }

        public static implicit operator string(Response response)
        {
            return response.Contents;
        }
    }
}