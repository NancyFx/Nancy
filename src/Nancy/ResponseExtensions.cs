namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Responses;

    public static class ResponseExtensions
    {
        /// <summary>
        /// Force the response to be downloaded as an attachment
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="fileName">Filename for the download</param>
        /// <param name="contentType">Optional content type</param>
        /// <returns>Mopdified Response object</returns>
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

        private static Tuple<string, string> GetTuple(object header)
        {
            var properties = header.GetType()
                                   .GetProperties()
                                   .Where(prop => prop.CanRead && prop.PropertyType == typeof(string))
                                   .ToArray();

            var headerProperty = properties
                                    .Where(p => string.Equals(p.Name, "Header", StringComparison.InvariantCultureIgnoreCase))
                                    .FirstOrDefault();

            var valueProperty = properties
                                    .Where(p => string.Equals(p.Name, "Value", StringComparison.InvariantCultureIgnoreCase))
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