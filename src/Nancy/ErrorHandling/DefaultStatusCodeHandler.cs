namespace Nancy.ErrorHandling
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using IO;
    using Nancy.Extensions;

    /// <summary>
    /// Default error handler
    /// </summary>
    public class DefaultStatusCodeHandler : IStatusCodeHandler
    {
        private const string DisableErrorTracesTrueMessage = "Error details are currently disabled. Please set <code>StaticConfiguration.DisableErrorTraces = false;</code> to enable.";

        private readonly IDictionary<HttpStatusCode, string> errorPages;

        private readonly IDictionary<HttpStatusCode, Func<HttpStatusCode, NancyContext, string, string>> expansionDelegates;

        private readonly HttpStatusCode[] supportedStatusCodes = new[] { HttpStatusCode.NotFound, HttpStatusCode.InternalServerError};

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultStatusCodeHandler"/> type.
        /// </summary>
        public DefaultStatusCodeHandler()
        {
            this.errorPages = new Dictionary<HttpStatusCode, string>
                {
                    { HttpStatusCode.NotFound, LoadResource("404.html") },
                    { HttpStatusCode.InternalServerError, LoadResource("500.html") },
                };

            this.expansionDelegates = new Dictionary<HttpStatusCode, Func<HttpStatusCode, NancyContext, string, string>>
                {
                    { HttpStatusCode.InternalServerError, PopulateErrorInfo}
                };
        }

        /// <summary>
        /// Whether then
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

            string errorPage;

            if (!this.errorPages.TryGetValue(statusCode, out errorPage))
            {
                return;
            }

            if (String.IsNullOrEmpty(errorPage))
            {
                return;
            }

            Func<HttpStatusCode, NancyContext, string, string> expansionDelegate;
            if (this.expansionDelegates.TryGetValue(statusCode, out expansionDelegate))
            {
                errorPage = expansionDelegate.Invoke(statusCode, context, errorPage);
            }

            ModifyResponse(statusCode, context, errorPage);
        }

        private static void ModifyResponse(HttpStatusCode statusCode, NancyContext context, string errorPage)
        {
            if (context.Response == null)
            {
                context.Response = new Response() { StatusCode = statusCode };
            }

            context.Response.ContentType = "text/html";
            context.Response.Contents = s =>
                {
                    using (var writer = new StreamWriter(new UnclosableStreamWrapper(s), Encoding.UTF8))
                    {
                        writer.Write(errorPage);
                    }
                };
        }

        private static string LoadResource(string filename)
        {
            var resourceStream = typeof(INancyEngine).Assembly.GetManifestResourceStream(String.Format("Nancy.ErrorHandling.Resources.{0}", filename));

            if (resourceStream == null)
            {
                return string.Empty;
            }

            using (var reader = new StreamReader(resourceStream))
            {
                return reader.ReadToEnd();
            }
        }

        private static string PopulateErrorInfo(HttpStatusCode httpStatusCode, NancyContext context, string templateContents)
        {
            return templateContents.Replace("[DETAILS]", StaticConfiguration.DisableErrorTraces ? DisableErrorTracesTrueMessage : context.GetExceptionDetails());
        }
    }
}