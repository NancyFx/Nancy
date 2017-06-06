namespace Nancy.Prototype.Http
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security.Claims;

    [DebuggerDisplay("{ToString(), nq}")]
    public abstract class HttpContext
    {
        public abstract HttpRequest Request { get; }

        public abstract HttpResponse Response { get; }

        public abstract ClaimsPrincipal User { get; set; }

        public abstract IDictionary<object, object> Items { get; }

        public override string ToString()
        {
            return $"{this.Request} -> {this.Response}";
        }
    }
}
