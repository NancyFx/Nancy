namespace Nancy.Prototype.Http
{
    public partial struct HttpStatusCode
    {
        public static HttpStatusCode Ok { get; } = new HttpStatusCode(200);

        public static HttpStatusCode Created { get; } = new HttpStatusCode(201);

        public static HttpStatusCode Accepted { get; } = new HttpStatusCode(202);

        public static HttpStatusCode NonAuthoritativeInformation { get; } = new HttpStatusCode(203);

        public static HttpStatusCode NoContent { get; } = new HttpStatusCode(204);

        // TODO: Generate these...
    }
}
