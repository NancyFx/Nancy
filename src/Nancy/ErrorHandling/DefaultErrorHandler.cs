namespace Nancy.ErrorHandling
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Nancy.Extensions;

    /// <summary>
    /// Default error handler
    /// </summary>
    public class DefaultErrorHandler : IErrorHandler
    {
        private IDictionary<HttpStatusCode, string> errorPages;

        private IDictionary<HttpStatusCode, Func<HttpStatusCode, NancyContext, string, string>> expansionDelegates;

        private HttpStatusCode[] supportedStatusCodes = new[] { HttpStatusCode.NotFound, HttpStatusCode.InternalServerError};

        public DefaultErrorHandler()
        {
            this.errorPages = new Dictionary<HttpStatusCode, string>
                {
                    { HttpStatusCode.NotFound, this.LoadResource("404.html") },
                    { HttpStatusCode.InternalServerError, this.LoadResource("500.html") },
                };

            this.expansionDelegates = new Dictionary<HttpStatusCode, Func<HttpStatusCode, NancyContext, string, string>>
                {
                    { HttpStatusCode.InternalServerError, this.PopulateErrorInfo}
                };
        }

        /// <summary>
        /// Whether then 
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <returns>True if handled, false otherwise</returns>
        public bool HandlesStatusCode(HttpStatusCode statusCode)
        {
            return this.supportedStatusCodes.Any(s => s == statusCode);
        }

        /// <summary>
        /// Handle the error code
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <param name="context">Current context</param>
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

            this.ModifyResponse(statusCode, context, errorPage);
        }

        private void ModifyResponse(HttpStatusCode statusCode, NancyContext context, string errorPage)
        {
            if (context.Response == null)
            {
                context.Response = new Response() { StatusCode = statusCode };
            }

            context.Response.ContentType = "text/html";
            context.Response.Contents = s =>
                {
                    using (var writer = new StreamWriter(s, Encoding.UTF8))
                    {
                        writer.Write(errorPage);
                    }
                };
        }

        private string LoadResource(string filename)
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

        private string PopulateErrorInfo(HttpStatusCode httpStatusCode, NancyContext context, string templateContents)
        {
            return templateContents.Replace("[DETAILS]", StaticConfiguration.DisableErrorTraces ? String.Empty : context.GetExceptionDetails());
        }
    }
}