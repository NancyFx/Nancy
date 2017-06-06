namespace Nancy.Prototype.Http
{
    using System;
    using System.Collections.Generic;

    public partial struct HttpMethod
    {
        private static readonly IDictionary<string, HttpMethod> Methods;

        static HttpMethod()
        {
            Methods = new Dictionary<string, HttpMethod>(StringComparer.OrdinalIgnoreCase)
            {
                { "GET", Get },
                { "PUT", Put },
                { "POST", Post },
                { "DELETE", Delete },
                { "PATCH", Patch },
                { "HEAD", Head },
                { "OPTIONS", Options },
            };
        }

        public static HttpMethod Get { get; } = new HttpMethod("GET");

        public static HttpMethod Put { get; } = new HttpMethod("PUT");

        public static HttpMethod Post { get; } = new HttpMethod("POST");

        public static HttpMethod Delete { get; } = new HttpMethod("DELETE");

        public static HttpMethod Patch { get; } = new HttpMethod("PATCH");

        public static HttpMethod Head { get; } = new HttpMethod("HEAD");

        public static HttpMethod Options { get; } = new HttpMethod("OPTIONS");

        // TODO: Do we want to generate these from a registry?

        public static HttpMethod FromString(string value)
        {
            Check.NotNull(value, nameof(value));

            var trimmedValue = value.Trim();

            if (trimmedValue.Length == 0 || !HttpUtility.IsValidToken(trimmedValue))
            {
                throw new ArgumentException(string.Format(
                    Resources.Exception_InvalidHttpMethodToken, trimmedValue), nameof(value));
            }

            HttpMethod method;
            if (!Methods.TryGetValue(trimmedValue, out method))
            {
                Methods.Add(trimmedValue, method = new HttpMethod(trimmedValue));
            }

            return method;
        }
    }
}
