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

            return (errorObject as string) ?? string.Empty;
        }

        /// <summary>
        /// Get a thrown exception from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The thrown exception or <c>null</c> if not exception has been thrown.</returns>
        public static Exception GetException(this NancyContext context)
        {
            return GetException<Exception>(context);
        }

        /// <summary>
        /// Get a thrown exception of the given type from the context.
        /// </summary>
        /// <typeparam name="T">The type of exception to get.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>The thrown exception or <c>null</c> if not exception has been thrown.</returns>
        public static T GetException<T>(this NancyContext context) where T : Exception
        {
            T exception;
            return TryGetException(context, out exception) ? exception : null;
        }

        /// <summary>
        /// Tries to get a thrown exception from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="exception">The thrown exception.</param>
        /// <returns><c>true</c> if an exception has been thrown during the request, <c>false</c> otherwise.</returns>
        public static bool TryGetException(this NancyContext context, out Exception exception)
        {
            return TryGetException<Exception>(context, out exception);
        }

        /// <summary>
        /// Tries to get a thrown exception of the given type from the context.
        /// </summary>
        /// <typeparam name="T">The type of exception to get.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="exception">The thrown exception.</param>
        /// <returns><c>true</c> if an exception of the given type has been thrown during the request, <c>false</c> otherwise.</returns>
        public static bool TryGetException<T>(this NancyContext context, out T exception) where T : Exception
        {
            object exceptionObject;
            if (context.Items.TryGetValue(NancyEngine.ERROR_EXCEPTION, out exceptionObject) && exceptionObject is T)
            {
                exception = exceptionObject as T;
                return true;
            }

            exception = null;
            return false;
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

            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                return false;
            }

            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                var currentHostName = context.Request.Url.HostName;

                // Mono does not populate the uri.Host correctly when url
                // is in //hostname format causing the simple check to fail.
                if (uri.Scheme.Equals("file"))
                {
                    var localFormat = string.Format("//{0}", currentHostName);
                    return !url.StartsWith("//") || url.StartsWith(localFormat);
                }

                return uri.Host == currentHostName;
            }

            return Uri.TryCreate(url, UriKind.Relative, out uri);
        }
    }
}