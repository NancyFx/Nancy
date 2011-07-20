namespace Nancy.Hosting.Owin.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using IO;

    public static class EnvironmentExtensions
    {
        /// <summary>
        /// Convert the OWIN envionment dictionary to Nancy request parameters
        /// </summary>
        /// <param name="environment">OWIN request environment</param>
        /// <returns>Nancy request parameters</returns>
        public static NancyRequestParameters AsNancyRequestParameters(this IDictionary<string, object> environment)
        {
            return new NancyRequestParameters
            {
                Method = GetMethod(environment),
                Url = GetUrl(environment),
                Headers = GetHeaders(environment),
                Body = new RequestStream(GetExpectedRequestLength(environment), false),
            };
        }

        private static int GetExpectedRequestLength(IDictionary<string, object> environment)
        {
            var incomingHeaders = (IDictionary<string, string>)environment["owin.RequestHeaders"];

            if (incomingHeaders == null)
            {
                return 0;
            }

            string contentLengthString;
            if (!incomingHeaders.TryGetValue("Content-Length", out contentLengthString))
            {
                return 0;
            }

            int contentLength;
            if (!int.TryParse(contentLengthString, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength))
            {
                return 0;
            }

            return contentLength;
        }

        private static string GetMethod(IDictionary<string, object> environment)
        {
            return (string)environment["owin.RequestMethod"];
        }

        private static IDictionary<string, IEnumerable<string>> GetHeaders(IDictionary<string, object> environment)
        {
            var incomingHeaders = (IDictionary<string, string>)environment["owin.RequestHeaders"];
            var headers = new Dictionary<string, IEnumerable<string>>(incomingHeaders.Count);

            foreach (var incomingHeader in incomingHeaders)
            {
                headers.Add(incomingHeader.Key, incomingHeader.Value.Split(','));
            }

            return headers;
        }

        private static Url GetUrl(IDictionary<string, object> environment)
        {
            return new Url
            {
                Scheme = (string)environment["owin.RequestScheme"],
                HostName = ((IDictionary<string, string>)environment["owin.RequestHeaders"])["Host"],
                Port = null,
                BasePath = (string)environment["owin.RequestPathBase"],
                Path = (string)environment["owin.RequestPath"],
                Query = (string)environment["owin.RequestQueryString"],
            };
        }
    }
}