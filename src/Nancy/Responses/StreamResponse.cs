using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nancy.Responses
{
    public class StreamResponse<TModel> : Response
    {
        public StreamResponse(TModel source, string contentType)
        {
            this.Contents = GetStream(source);
            this.ContentType = contentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetStream(TModel source)
        {
            return stream =>
            {
                Stream sourceStream = source as Stream;
                if (sourceStream != null && sourceStream.CanRead)
                {
                    sourceStream.Position = 0;
                    sourceStream.CopyTo(stream);
                }
            };
        }
    }
}
