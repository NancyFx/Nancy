namespace Nancy
{
    using System.Net;

    public class Response
    {
        public string ContentType { get; set; }

        public string Contents { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}