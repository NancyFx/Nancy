namespace Nancy.Security
{
    using System;
    using System.Linq;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Allows a BeforeRequest hook to change Url to HTTPS if X-Forwarded-Proto header present
    /// </summary>
    public class SSLProxy
    {
        public static void Enable(IPipelines pipelines)
        {
            pipelines.BeforeRequest += (ctx) =>
            {
                //X-Forwarded-Proto: https
                if (ctx.Request.Headers.Keys.Any(x => x.Equals("X-Forwarded-Proto", StringComparison.OrdinalIgnoreCase)))
                {
                    ctx.Request.Url.Scheme = "https";
                }

                //RFC7239
                if (ctx.Request.Headers.Keys.Any(x => x.Equals("Forwarded", StringComparison.OrdinalIgnoreCase)))
                {
                    var forwardedHeader = ctx.Request.Headers["Forwarded"];
                    var protoValue = forwardedHeader.FirstOrDefault(x => x.StartsWith("proto", StringComparison.OrdinalIgnoreCase));
                    if (protoValue != null && protoValue.Equals("proto=https", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Request.Url.Scheme = "https";
                    }
                }

                return null;
            };
        }
    }
}
