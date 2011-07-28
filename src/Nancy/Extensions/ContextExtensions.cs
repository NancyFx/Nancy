namespace Nancy.Extensions
{
    using Nancy.Responses;

    /// <summary>
    /// Containing extensions for the NancyContext object
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
    }
}