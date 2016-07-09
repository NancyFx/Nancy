namespace Nancy.Hosting.Self
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A helper class that checks for a header against a list of headers that should be ignored
    ///     when populating the headers of an <see cref="T:System.Net.HttpListenerResponse"/> object.
    /// </summary>
    public static class IgnoredHeaders
    {

        private static readonly HashSet<string> knownHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "content-length",
            "content-type",
            "transfer-encoding",
            "keep-alive"
        };

        /// <summary>
        ///     Determines if a header is ignored when populating the headers of an
        ///     <see cref="T:System.Net.HttpListenerResponse"/> object.
        /// </summary>
        /// <param name="headerName">The name of the header.</param>
        /// <returns><c>true</c> if the header is ignored; otherwise, <c>false</c>.</returns>
        public static bool IsIgnored(string headerName)
        {
            return knownHeaders.Contains(headerName);
        }

    }

}
