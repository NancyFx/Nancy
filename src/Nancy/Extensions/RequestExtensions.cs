namespace Nancy.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class RequestExtensions
    {
        public static bool IsAjaxRequest(this Request request)
        {
            const string ajaxRequestHeaderKey = "X-Requested-With";
            const string ajaxRequestHeaderValue = "XMLHttpRequest";

            if (request.Headers.ContainsKey(ajaxRequestHeaderKey))
            {
                IEnumerable<string> values;

                request.Headers.TryGetValue(ajaxRequestHeaderKey, out values);

                if (values.Contains(ajaxRequestHeaderValue))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
