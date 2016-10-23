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
            var userHostAddress = request.UserHostAddress;

            if (!string.IsNullOrEmpty(userHostAddress))
            {
                if (userHostAddress.Equals("127.0.0.1"))
                {
                    return true;
                }

                if (userHostAddress.Equals("::1"))
                {
                    return true;
                }
            }

            var url = request.Url;

            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return uri.IsLoopback;
            }

            // Invalid or relative Request.Url string
            return false;
        }
    }
}
