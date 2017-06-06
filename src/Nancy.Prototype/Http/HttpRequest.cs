namespace Nancy.Prototype.Http
{
    using System.Diagnostics;
    using System.IO;

    [DebuggerDisplay("{ToString(), nq}")]
    public abstract class HttpRequest
    {
        public abstract HttpContext Context { get; }

        public abstract HttpMethod Method { get; set; }

        public abstract Url Url { get; set; }

        public abstract string Protocol { get; set; }

        public abstract IHeaderDictionary Headers { get; }

        public abstract long? ContentLength { get; set; }

        public abstract MediaRange ContentType { get; set; }

        public abstract Stream Body { get; set; }

        // TODO: Cookies

        // TODO: Form

        public override string ToString()
        {
            return $"{this.Method.ToString()} {this.Url} {this.Protocol}";
        }
    }
}
