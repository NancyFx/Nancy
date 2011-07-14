namespace Nancy.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Containing extensions for the Request object
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// An extension method making it easy to check if the reqeuest was done using ajax
        /// </summary>
        /// <param name="request">The request made by client</param>
        /// <returns>True if the request was done using ajax</returns>
        public static bool IsAjaxRequest(this Request request)
        {
            const string ajaxRequestHeaderKey = "X-Requested-With";
            const string ajaxRequestHeaderValue = "XMLHttpRequest";

            return request.Headers[ajaxRequestHeaderKey].Contains(ajaxRequestHeaderValue);
        }
    }
}
