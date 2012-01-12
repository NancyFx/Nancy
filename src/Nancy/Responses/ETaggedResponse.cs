

using System.Linq;
using System.Web;

namespace Nancy.Responses
{
    using System.IO;
    using Cryptography;
    using System;
  
    public class ETaggedResponse: Response
    {
        private string _ETag;

        /// <summary>
        /// Gets the E tag.
        /// </summary>
        public string ETag
        {
            get
            {
                if(string.IsNullOrEmpty(_ETag))
                {
                    _ETag = string.Format("\"{0}\"", 
                    GetResponseETag(Contents));
                }

                return _ETag;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ETaggedResponse"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public ETaggedResponse(Response response):
            this(response, GetResponseETag(response.Contents))
        {
           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ETaggedResponse"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="value">The value.</param>
        public ETaggedResponse(Response response, string value)
        {
            _ETag       = value;
            Headers     = response.Headers;
            ContentType = response.ContentType;
            StatusCode  = response.StatusCode;
            Contents    = response.Contents;
            this.WithHeader("ETag", ETag);
        }


        /// <summary>
        /// Gets the result E tag.
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static string GetResponseETag(Action<Stream> contents)
        {
            using (var stream = new MemoryStream())
            {
                contents.Invoke(stream);
                
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                return CRC32Helper.Compute(stream).ToString();
            }
        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <param name="context">The context.</param>
        public bool ProcessResponse(HttpContextBase context)
        {
            var etagString = context.Request.Headers["if-none-match"];
            
            if (!string.IsNullOrEmpty(etagString))
            {
                var etags = etagString.Split(',');
                if (etags.Any(x => x.Trim().Equals(ETag)))
                {
                    context.Response.StatusCode = 304;
                    context.Response.SuppressContent = true;
                    context.Response.ClearContent();
                    context.ApplicationInstance.CompleteRequest();
                    return true;
                }
            }

            return false;
        }
    }
}