namespace Nancy.Responses
{
    using System;
    using System.IO;

    public class StreamResponse : Response
    {
        public StreamResponse(Func<Stream> source, string contentType)
        {
            this.Contents = GetResponseBodyDelegate(source);
            this.ContentType = contentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetResponseBodyDelegate(Func<Stream> sourceDelegate)
        {
            return stream =>
                {
                    using (var source = sourceDelegate.Invoke())
                    {
                        if (source.CanSeek)
                        {
                            source.Position = 0;
                        }

                        if (source.CanRead)
                        {
                            source.CopyTo(stream);
                        }
                    }
                };
        }
    }
}
