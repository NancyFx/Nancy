namespace Nancy.Extensions
{
    using System;
    using System.Text;
    using Nancy.Responses;

    /// <summary>
    /// Containing extensions for the <see cref="NancyContext"/> object
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// Ascertains if a request originated from an Ajax request or not.
        /// </summary>
        /// <param name="context">The current nancy context</param>
        /// <returns>True if the request was done using ajax, false otherwise</returns>
        public static bool IsAjaxRequest(this NancyContext context)
        {
            return context.Request != null && context.Request.IsAjaxRequest();
        }

        /// <summary>
        /// Expands a path to take into account a base path (if any)
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="path">Path to expand</param>
        /// <returns>Expanded path</returns>
        public static string ToFullPath(this NancyContext context, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            if (context.Request == null)
            {
                return path.TrimStart('~');
            }
            
            if (string.IsNullOrEmpty(context.Request.Url.BasePath))
            {
                return path.TrimStart('~');
            }

            if (!path.StartsWith("~/"))
            {
                return path;
            }

            return string.Format("{0}{1}", context.Request.Url.BasePath, path.TrimStart('~'));
        }

        /// <summary>
        /// Returns a redirect response with the redirect path expanded to take into
        /// account a base path (if any)
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="path">Path to redirect to</param>
        /// <returns>Redirect response</returns>
        public static RedirectResponse GetRedirect(this NancyContext context, string path)
        {
            return new RedirectResponse(context.ToFullPath(path));
        }

        /// <summary>
        /// Retrieves exception details from the context, if any exist
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <returns>Exception details</returns>
        public static string GetExceptionDetails(this NancyContext context)
        {
            object errorObject;
            context.Items.TryGetValue(NancyEngine.ERROR_KEY, out errorObject);

            return (errorObject as string) ?? "None";
        }

        /// <summary>
        /// Shortcut extension method for writing trace information
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="logDelegate">Log delegate</param>
        public static void WriteTraceLog(this NancyContext context, Action<StringBuilder> logDelegate)
        {
            context.Trace.TraceLog.WriteLog(logDelegate);
        }

        /// <summary>
        /// Returns a boolean indicating whether a given url string is local or not
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="url">Url string (relative or absolute)</param>
        /// <returns>True if local, false otherwise</returns>
        public static bool IsLocalUrl(this NancyContext context, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            Uri uri;

            if (Uri.TryCreate(url, UriKind.Relative, out uri))
            {
                return true;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return false;
            }

            return string.Equals(uri.Host, context.Request.Url.HostName, StringComparison.OrdinalIgnoreCase);
        }
    }
}