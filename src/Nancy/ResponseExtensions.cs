namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Cookies;
    using Nancy.Responses;

    /// <summary>
    /// Containing extensions for the <see cref="Response"/> object.
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// Force the response to be downloaded as an attachment
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="fileName">Filename for the download</param>
        /// <param name="contentType">Optional content type</param>
        /// <returns>Modified Response object</returns>
        public static Response AsAttachment(this Response response, string fileName = null, string contentType = null)
        {
            var actualFilename = fileName;

            if (actualFilename == null && response is GenericFileResponse)
            {
                actualFilename = ((GenericFileResponse)response).Filename;
            }

            if (string.IsNullOrEmpty(actualFilename))
            {
                throw new ArgumentException("fileName cannot be null or empty");
            }

            if (contentType != null)
            {
                response.ContentType = contentType;
            }

            return response.WithHeader("Content-Disposition", "attachment; filename=" + actualFilename);
        }

        /// <summary>
        /// Adds a <see cref="INancyCookie"/> to the response.
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <returns>The <see cref="Response"/> instance.</returns>
        public static Response WithCookie(this Response response, string name, string value)
        {
            return WithCookie(response, name, value, null, null, null);
        }

        /// <summary>
        /// Adds a <see cref="INancyCookie"/> to the response.
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="expires">The expiration date of the cookie. Can be <see langword="null" /> if it should expire at the end of the session.</param>
        /// <returns>The <see cref="Response"/> instance.</returns>
        public static Response WithCookie(this Response response, string name, string value, DateTime? expires)
        {
            return WithCookie(response, name, value, expires, null, null);
        }

        /// <summary>
        /// Adds a <see cref="INancyCookie"/> to the response.
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="expires">The expiration date of the cookie. Can be <see langword="null" /> if it should expire at the end of the session.</param>
        /// <param name="domain">The domain of the cookie.</param>
        /// <param name="path">The path of the cookie.</param>
        /// <returns>The <see cref="Response"/> instance.</returns>
        public static Response WithCookie(this Response response, string name, string value, DateTime? expires, string domain, string path)
        {
            return WithCookie(response, new NancyCookie(name, value) { Expires = expires, Domain = domain, Path = path });
        }

        /// <summary>
        /// Adds a <see cref="INancyCookie"/> to the response.
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="nancyCookie">A <see cref="INancyCookie"/> instance.</param>
        /// <returns></returns>
        public static Response WithCookie(this Response response, INancyCookie nancyCookie)
        {
            response.Cookies.Add(nancyCookie);
            return response;
        }

        /// <summary>
        /// Add a header to the response
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="header">Header name</param>
        /// <param name="value">Header value</param>
        /// <returns>Modified response</returns>
        public static Response WithHeader(this Response response, string header, string value)
        {
            return response.WithHeaders(new { Header = header, Value = value });
        }

        /// <summary>
        /// Adds headers to the response using anonymous types
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="headers">
        /// Array of headers - each header should be an anonymous type with two string properties 
        /// 'Header' and 'Value' to represent the header name and its value.
        /// </param>
        /// <returns>Modified response</returns>
        public static Response WithHeaders(this Response response, params object[] headers)
        {
            return response.WithHeaders(headers.Select(GetTuple).ToArray());
        }

        /// <summary>
        /// Adds headers to the response using anonymous types
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="headers">
        /// Array of headers - each header should be a Tuple with two string elements 
        /// for header name and header value
        /// </param>
        /// <returns>Modified response</returns>
        public static Response WithHeaders(this Response response, params Tuple<string, string>[] headers)
        {
            if (response.Headers == null)
            {
                response.Headers = new Dictionary<string, string>();
            }

            foreach (var keyValuePair in headers)
            {
                response.Headers[keyValuePair.Item1] = keyValuePair.Item2;
            }

            return response;
        }

        /// <summary>
        /// Sets the content type of the response
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="contentType">The type of the content</param>
        /// <returns>Modified response</returns>
        public static Response WithContentType(this Response response, string contentType)
        {
            response.ContentType = contentType;
            return response;
        }

        /// <summary>
        /// Sets the status code of the response
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="statusCode">The http status code</param>
        /// <returns>Modified response</returns>
        public static Response WithStatusCode(this Response response, HttpStatusCode statusCode)
        {
            response.StatusCode = statusCode;
            return response;
        }

        /// <summary>
        /// Sets the status code of the response
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="statusCode">The http status code</param>
        /// <returns>Modified response</returns>
        public static Response WithStatusCode(this Response response, int statusCode)
        {
            response.StatusCode = (HttpStatusCode) statusCode;
            return response;
        }

        private static Tuple<string, string> GetTuple(object header)
        {
            var properties = header.GetType()
                                   .GetProperties()
                                   .Where(prop => prop.CanRead && prop.PropertyType == typeof(string))
                                   .ToArray();

            var headerProperty = properties
                                    .Where(p => string.Equals(p.Name, "Header", StringComparison.OrdinalIgnoreCase))
                                    .FirstOrDefault();

            var valueProperty = properties
                                    .Where(p => string.Equals(p.Name, "Value", StringComparison.OrdinalIgnoreCase))
                                    .FirstOrDefault();

            if (headerProperty == null || valueProperty == null)
            {
                throw new ArgumentException("Unable to extract 'Header' or 'Value' properties from anonymous type.");
            }

            return Tuple.Create(
                (string)headerProperty.GetValue(header, null),
                (string)valueProperty.GetValue(header, null));
        }
    }
}