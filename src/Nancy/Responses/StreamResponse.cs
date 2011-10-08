using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nancy.Responses
{
    public class StreamResponse<TModel> : Response
    {
        public StreamResponse(Stream source, string contentType)
        {
            this.Contents = GetStream(source);
            this.ContentType = contentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetStream(Stream source)
        {
            return stream =>
            {
                if (source != null)
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
