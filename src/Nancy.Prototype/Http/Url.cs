namespace Nancy.Prototype.Http
{
    using System;
    using System.Diagnostics;
    using System.Text;

    [DebuggerDisplay("{ToString(), nq}")]
    public abstract class Url
    {
        public const string HttpScheme = "http";

        public const string HttpsScheme = "https";

        public const string SchemeDelimiter = "://";

        public abstract string Scheme { get; set; }

        // TODO: Split out port? Create and use HostString type?
        public abstract string Host { get; set; }

        // TODO: Create and use PathString type?
        public abstract string PathBase { get; set; }

        // TODO: Create and use PathString type?
        public abstract string Path { get; set; }

        // TODO: Create and use QueryString type?
        public abstract string QueryString { get; set; }

        public bool IsHttps => string.Equals(this.Scheme, HttpsScheme, StringComparison.OrdinalIgnoreCase);

        public override string ToString()
        {
            var scheme = this.Scheme;
            var host = this.Host;
            var pathBase = this.PathBase;
            var path = this.Path;
            var queryString = this.QueryString;

            // PERF: Pre-compute the length to allocate correctly in StringBuilder.
            var length = scheme.Length
                + SchemeDelimiter.Length
                + host.Length
                + pathBase.Length
                + path.Length
                + queryString.Length;

            return new StringBuilder(capacity: length)
                .Append(scheme)
                .Append(SchemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(path)
                .Append(queryString)
                .ToString();
        }
    }
}
