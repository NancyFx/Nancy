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
        /// <summary>
        /// Checks for Forwarded or X-Forwarded-Proto header and if so makes curent url schemme https
        /// </summary>
        /// <param name="pipelines"></param>
        public static void MakeNancyUrlSecure(IPipelines pipelines)
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
