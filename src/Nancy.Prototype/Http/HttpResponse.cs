namespace Nancy.Prototype.Http
{
    using System.Diagnostics;
    using System.IO;

    [DebuggerDisplay("{ToString(), nq}")]
    public abstract class HttpResponse
    {
        public abstract HttpContext Context { get; }

        public abstract HttpStatusCode StatusCode { get; set; }

        public abstract string ReasonPhrase { get; set; }

        public abstract IHeaderDictionary Headers { get; }

        public abstract long? ContentLength { get; set; }

        public abstract MediaRange ContentType { get; set; }

        public abstract Stream Body { get; set; }

        // TODO: Cookies

        public override string ToString()
        {
            return $"{this.StatusCode.ToString()} {this.ReasonPhrase}";
        }
    }
}
