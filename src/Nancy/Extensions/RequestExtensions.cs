namespace Nancy.Extensions
{
    using System.Linq;

    /// <summary>
    /// Containing extensions for the <see cref="Request"/> object
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// An extension method making it easy to check if the reqeuest was done using ajax
        /// </summary>
        /// <param name="request">The request made by client</param>
        /// <returns><see langword="true" /> if the request was done using ajax, otherwise <see langword="false"/>.</returns>
        public static bool IsAjaxRequest(this Request request)
        {
            const string ajaxRequestHeaderKey = "X-Requested-With";
            const string ajaxRequestHeaderValue = "XMLHttpRequest";

            return request.Headers[ajaxRequestHeaderKey].Contains(ajaxRequestHeaderValue);
        }
    }
}
