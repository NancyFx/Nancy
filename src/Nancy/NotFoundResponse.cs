namespace Nancy
{
    using System.Net;

    public class NotFoundResponse : Response
    {
        public NotFoundResponse()
        {
            this.Contents = string.Empty;
            this.ContentType = "text/html";
            this.StatusCode = HttpStatusCode.NotFound;
        }
    }
}