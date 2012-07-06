using System;
using System.Collections.Generic;
using System.Linq;
using Nancy.Responses.Negotiation;

namespace Nancy
{
    public static class NegotiatorExtensions
    {
        /// <summary>
        /// Add a header to the response
        /// </summary>
        /// <param name="negotiator">Negotiator object</param>
        /// <param name="header">Header name</param>
        /// <param name="value">Header value</param>
        /// <returns>Modified negotiator</returns>
        public static NancyModule.Negotiator WithHeader(this NancyModule.Negotiator negotiator, string header, string value)
        {
            return negotiator.WithHeaders(new { Header = header, Value = value });
        }

        /// <summary>
        /// Adds headers to the response using anonymous types
        /// </summary>
        /// <param name="negotiator">Negotiator object</param>
        /// <param name="headers">
        /// Array of headers - each header should be an anonymous type with two string properties 
        /// 'Header' and 'Value' to represent the header name and its value.
        /// </param>
        /// <returns>Modified negotiator</returns>
        public static NancyModule.Negotiator WithHeaders(this NancyModule.Negotiator negotiator, params object[] headers)
        {
            return negotiator.WithHeaders(headers.Select(GetTuple).ToArray());
        }

        /// <summary>
        /// Adds headers to the response using anonymous types
        /// </summary>
        /// <param name="negotiator">Negotiator object</param>
        /// <param name="headers">
        /// Array of headers - each header should be a Tuple with two string elements 
        /// for header name and header value
        /// </param>
        /// <returns>Modified negotiator</returns>
        public static NancyModule.Negotiator WithHeaders(this NancyModule.Negotiator negotiator, params Tuple<string, string>[] headers)
        {
            foreach (var keyValuePair in headers)
            {
                negotiator.NegotiationContext.Headers[keyValuePair.Item1] = keyValuePair.Item2;
            }

            return negotiator;
        }

        /// <summary>
        /// Allows the response to be negotiated with any processors available for any content type
        /// </summary>
        /// <param name="negotiator">Negotiator object</param>
        /// <returns>Modified negotiator</returns>
        public static NancyModule.Negotiator WithFullNegotiation(this NancyModule.Negotiator negotiator)
        {
            negotiator.NegotiationContext.PermissableMediaRanges.Clear();
            negotiator.NegotiationContext.PermissableMediaRanges.Add("*/*");

            return negotiator;
        }

        /// <summary>
        /// Allows the response to be negotiated with a specific media range
        /// This will remove the wildcard range if it is already specified
        /// </summary>
        /// <param name="negotiator">Negotiator object</param>
        /// <param name="mediaRange">Media range to add</param>
        /// <returns>Modified negotiator</returns>
        public static NancyModule.Negotiator WithAllowedMediaRange(this NancyModule.Negotiator negotiator, MediaRange mediaRange)
        {
            var wildcards =
                negotiator.NegotiationContext.PermissableMediaRanges.Where(
                    mr => mediaRange.Type.IsWildcard && mediaRange.Subtype.IsWildcard);

            foreach (var wildcard in wildcards)
            {
                negotiator.NegotiationContext.PermissableMediaRanges.Remove(wildcard);
            }

            negotiator.NegotiationContext.PermissableMediaRanges.Add(mediaRange);

            return negotiator;
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