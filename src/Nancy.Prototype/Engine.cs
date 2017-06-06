namespace Nancy.Prototype
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Nancy.Prototype.Http;

    public class Engine : IEngine
    {
        public async Task HandleRequest(HttpContext context, CancellationToken cancellationToken)
        {
            Check.NotNull(context, nameof(context));

            context.Response.StatusCode = HttpStatusCode.Ok;
            context.Response.ContentType = MediaRange.ApplicationJson;

            using (var writer = new StreamWriter(context.Response.Body))
            {
                await writer.WriteLineAsync($@"{{ ""url"": ""{context.Request.Url}"" }}").ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}
