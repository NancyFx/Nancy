namespace Nancy
{
    using System.Net;

    public class NotFoundResponse : Response
    {
        public NotFoundResponse()
        {
            this.StatusCode = HttpStatusCode.NotFound;
        }
    }
}