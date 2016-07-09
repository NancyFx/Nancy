namespace Nancy.Demo.Caching
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Nancy;

    /// <summary>
    /// Wraps a regular response in a cached response
    /// The cached response invokes the old response and stores it as a string.
    /// Obviously this only works for ASCII text based responses, so don't use this
    /// in a real application :-)
    /// </summary>
    public class CachedResponse : Response
    {
        private readonly Response response;

        public CachedResponse(Response response)
        {
            this.response = response;

            this.ContentType = response.ContentType;
            this.Headers = response.Headers;
            this.StatusCode = response.StatusCode;
            this.Contents = this.GetContents();
        }

        public override Task PreExecute(NancyContext context)
        {
            return this.response.PreExecute(context);
        }

        private Action<Stream> GetContents()
        {
            return stream =>
            {
                using (var memoryStream = new MemoryStream())
                {
                    this.response.Contents.Invoke(memoryStream);

                    var contents =
                        Encoding.ASCII.GetString(memoryStream.GetBuffer());

                    var writer =
                        new StreamWriter(stream) { AutoFlush = true };

                    writer.Write(contents);
                }
            };
        }
    }
}