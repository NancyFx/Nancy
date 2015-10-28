namespace Nancy.Extensions
{
    using System;
    using System.Linq;

    /// <summary>
    /// Containing extensions for the <see cref="Request"/> object
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// An extension method making it easy to check if the request was done using ajax
        /// </summary>
        /// <param name="request">The request made by client</param>
        /// <returns><see langword="true" /> if the request was done using ajax, otherwise <see langword="false"/>.</returns>
        public static bool IsAjaxRequest(this Request request)
        {
            const string ajaxRequestHeaderKey = "X-Requested-With";
            const string ajaxRequestHeaderValue = "XMLHttpRequest";

            return request.Headers[ajaxRequestHeaderKey].Contains(ajaxRequestHeaderValue);
        }
        /// <summary>
        /// Gets a value indicating whether the request is local.
        /// </summary>
        /// <param name="request">The request made by client</param>
        /// <returns><see langword="true" /> if the request is local, otherwise <see langword="false"/>.</returns>
        public static bool IsLocal(this Request request)
        {
            if (string.IsNullOrEmpty(request.UserHostAddress) || string.IsNullOrEmpty(request.Url))
            {
                return false;
            }

            Uri uri = null;
            if (Uri.TryCreate(request.Url, UriKind.Absolute, out uri))
            {
                return uri.IsLoopback;
            }
            else
            {
                // Invalid or relative Request.Url string
                return false;
            }
        }
    }
}
