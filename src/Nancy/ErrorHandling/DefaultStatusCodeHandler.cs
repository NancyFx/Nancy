namespace Nancy.ErrorHandling
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Nancy.Configuration;
    using Nancy.Extensions;
    using Nancy.IO;
    using Nancy.Responses.Negotiation;
    using Nancy.ViewEngines;

    /// <summary>
    /// Default error handler
    /// </summary>
    public class DefaultStatusCodeHandler : IStatusCodeHandler
    {
        private const string DisplayErrorTracesFalseMessage = "Error details are currently disabled.<br />To enable it, please set <strong>TraceConfiguration.DisplayErrorTraces</strong> to <strong>true</strong>.<br />For example by overriding your Bootstrapper's <strong>Configure</strong> method and calling<br/> <strong>environment.Tracing(enabled: false, displayErrorTraces: true);</strong>.";

        private readonly IDictionary<HttpStatusCode, string> errorMessages;
        private readonly IDictionary<HttpStatusCode, string> errorPages;
        private readonly IResponseNegotiator responseNegotiator;
        private readonly HttpStatusCode[] supportedStatusCodes = { HttpStatusCode.NotFound, HttpStatusCode.InternalServerError };
        private readonly TraceConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultStatusCodeHandler"/> type.
        /// </summary>
        /// <param name="responseNegotiator">The response negotiator.</param>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public DefaultStatusCodeHandler(IResponseNegotiator responseNegotiator, INancyEnvironment environment)
        {
            this.errorMessages = new Dictionary<HttpStatusCode, string>
            {
                { HttpStatusCode.NotFound, "The resource you have requested cannot be found." },
                { HttpStatusCode.InternalServerError, "Something went horribly, horribly wrong while servicing your request." }
            };

            this.errorPages = new Dictionary<HttpStatusCode, string>
            {
                { HttpStatusCode.NotFound, LoadResource("404.html") },
                { HttpStatusCode.InternalServerError, LoadResource("500.html") }
            };

            this.responseNegotiator = responseNegotiator;
            this.configuration = environment.GetValue<TraceConfiguration>();
        }

        /// <summary>
        /// Whether the status code is handled
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <param name="context">The <see cref="NancyContext"/> instance of the current request.</param>
        /// <returns>True if handled, false otherwise</returns>
        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return this.supportedStatusCodes.Any(s => s == statusCode);
        }

        /// <summary>
        /// Handle the error code
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <param name="context">The <see cref="NancyContext"/> instance of the current request.</param>
        /// <returns>Nancy Response</returns>
        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            if (context.Response != null && context.Response.Contents != null && !ReferenceEquals(context.Response.Contents, Response.NoBody))
            {
                return;
            }

            if (!this.errorMessages.ContainsKey(statusCode) || !this.errorPages.ContainsKey(statusCode))
            {
                return;
            }

            Response existingResponse = null;

            if (context.Response != null)
            {
                existingResponse = context.Response;
            }

            // Reset negotiation context to avoid any downstream cast exceptions
            // from swapping a view model with a `DefaultStatusCodeHandlerResult`
            context.NegotiationContext = new NegotiationContext();

            var details = !this.configuration.DisplayErrorTraces
                ? DisplayErrorTracesFalseMessage
                : string.Concat("<pre>", context.GetExceptionDetails().Replace("<", "&gt;").Replace(">", "&lt;"), "</pre>");

            var result = new DefaultStatusCodeHandlerResult(statusCode, this.errorMessages[statusCode], details);
            try
            {
                context.Response = this.responseNegotiator.NegotiateResponse(result, context);
                context.Response.StatusCode = statusCode;

                if (existingResponse != null)
                {
                    context.Response.ReasonPhrase = existingResponse.ReasonPhrase;
                }
                return;
            }
            catch (ViewNotFoundException)
            {
                // No view will be found for `DefaultStatusCodeHandlerResult`
                // because it is rendered from embedded resources below
            }

            this.ModifyResponse(statusCode, context, result);
        }

        private void ModifyResponse(HttpStatusCode statusCode, NancyContext context, DefaultStatusCodeHandlerResult result)
        {
            if (context.Response == null)
            {
                context.Response = new Response { StatusCode = statusCode };
            }

            var contents = this.errorPages[statusCode];

            if (!string.IsNullOrEmpty(contents))
            {
                contents = contents.Replace("[DETAILS]", result.Details);
            }

            context.Response.ContentType = "text/html";
            context.Response.Contents = s =>
            {
                using (var writer = new StreamWriter(new UnclosableStreamWrapper(s), Encoding.UTF8))
                {
                    writer.Write(contents);
                }
            };
        }

        private static string LoadResource(string filename)
        {
            var resourceStream = typeof(INancyEngine).GetTypeInfo().Assembly.GetManifestResourceStream(string.Format("Nancy.ErrorHandling.Resources.{0}", filename));


            if (resourceStream == null)
            {
                return string.Empty;
            }

            using (var reader = new StreamReader(resourceStream))
            {
                return reader.ReadToEnd();
            }
        }

        internal class DefaultStatusCodeHandlerResult
        {
            public DefaultStatusCodeHandlerResult(HttpStatusCode statusCode, string message, string details)
            {
                this.StatusCode = statusCode;
                this.Message = message;
                this.Details = details;
            }

            public HttpStatusCode StatusCode { get; private set; }

            public string Message { get; private set; }

            public string Details { get; private set; }
        }
    }
}
