namespace Nancy.ErrorHandling
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Default error handler
    /// </summary>
    public class DefaultErrorHandler : IErrorHandler
    {
        private IDictionary<HttpStatusCode, string> errorPages;

        public DefaultErrorHandler()
        {
            this.errorPages = new Dictionary<HttpStatusCode, string>
                {
                    { HttpStatusCode.NotFound, this.LoadResource("404.html") }
                };
        }

        /// <summary>
        /// Whether then 
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <returns>True if handled, false otherwise</returns>
        public bool HandlesStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.NotFound;
        }

        /// <summary>
        /// Handle the error code
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <param name="context">Current context</param>
        /// <returns>Nancy Response</returns>
        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            string errorPage;

            if (!this.errorPages.TryGetValue(statusCode, out errorPage))
            {
                return;
            }

            if (String.IsNullOrEmpty(errorPage))
            {
                return;
            }

            this.ModifyResponse(statusCode, context, errorPage);
        }

        private void ModifyResponse(HttpStatusCode statusCode, NancyContext context, string errorPage)
        {
            if (context.Response == null)
            {
                context.Response = new Response() { StatusCode = statusCode };
            }

            if (context.Response.Contents != null)
            {
                return;
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
    }
}