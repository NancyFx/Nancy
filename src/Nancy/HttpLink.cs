namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Represents one of possibly many RFC 5988 HTTP Links contained in a <see cref="HttpLinkBuilder"/>.
    /// </summary>
    public class HttpLink : IEquatable<HttpLink>
    {
        private static readonly HttpLinkParameterComparer ParameterComparer = new HttpLinkParameterComparer();

        private readonly IDictionary<string, object> parameters;

        private readonly Uri targetUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The target URI of the link.</param>
        /// <param name="relation">The relation that identifies the semantics of the link.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="targetUri" /> or <paramref name="relation" /></exception>
        public HttpLink(string targetUri, string relation)
            : this(ParseUri(targetUri), relation, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The target URI of the link.</param>
        /// <param name="relation">The relation that identifies the semantics of the link.</param>
        /// <param name="type">The optional type parameter is a hint indicating what the media type of the result of dereferencing the link should be. Note that this is only a hint; for example, it does not override the HTTP Content-Type header of a HTTP response obtained by actually following the link.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="targetUri" /> or <paramref name="relation" /></exception>
        public HttpLink(string targetUri, string relation, string type)
            : this(ParseUri(targetUri), relation, type, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The target URI of the link.</param>
        /// <param name="relation">The relation that identifies the semantics of the link.</param>
        /// <param name="type">The optional type parameter is a hint indicating what the media type of the result of dereferencing the link should be. Note that this is only a hint; for example, it does not override the HTTP Content-Type header of a HTTP response obtained by actually following the link.</param>
        /// <param name="title">The optional title parameter is used to label the destination of a link such that it can be used as a human-readable identifier (e.g., a menu entry) in the language indicated by the HTTP Content-Language header (if present).</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="targetUri" /> or <paramref name="relation" /></exception>
        public HttpLink(string targetUri, string relation, string type, string title)
            : this(ParseUri(targetUri), relation, type, title)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The target URI of the link.</param>
        /// <param name="relation">The relation that identifies the semantics of the link.</param>
        /// <param name="type">The optional type parameter is a hint indicating what the media type of the result of dereferencing the link should be. Note that this is only a hint; for example, it does not override the HTTP Content-Type header of a HTTP response obtained by actually following the link.</param>
        /// <param name="title">The optional title parameter is used to label the destination of a link such that it can be used as a human-readable identifier (e.g., a menu entry) in the language indicated by the HTTP Content-Language header (if present).</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="targetUri" /> or <paramref name="relation" /></exception>
        public HttpLink(Uri targetUri, string relation, string type, string title)
        {
            if (targetUri == null)
            {
                throw new ArgumentNullException("targetUri");
            }

            if (relation == null)
            {
                throw new ArgumentNullException("relation");
            }

            this.parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "rel", HttpLinkRelation.Parse(relation) }
            };
            this.targetUri = targetUri;

            if (!string.IsNullOrWhiteSpace(type))
            {
                this.parameters.Add("type", new MediaRange(type));
            }

            if (!string.IsNullOrEmpty(title))
            {
                this.parameters.Add("title", title);
            }
        }

        /// <summary>
        /// The dictionary of parameters associated with the link.
        /// </summary>
        public IDictionary<string, object> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// The relation that identifies the semantics of the link.
        /// </summary>
        public HttpLinkRelation Relation
        {
            get { return this.GetParameterValue<HttpLinkRelation>("rel"); }
        }

        /// <summary>
        /// Gets the target URI.
        /// </summary>
        public Uri TargetUri
        {
            get { return this.targetUri; }
        }

        /// <summary>
        /// The optional title parameter is used to label the destination of a link such that it can be used as a human-readable identifier (e.g., a menu entry) in the language indicated by the HTTP Content-Language header (if present).
        /// </summary>
        public string Title
        {
            get { return this.GetParameterValue<string>("title"); }
        }

        /// <summary>
        /// The optional type parameter is a hint indicating what the media type of the result of dereferencing the link should be. Note that this is only a hint; for example, it does not override the HTTP Content-Type header of a HTTP response obtained by actually following the link.
        /// </summary>
        public MediaRange Type
        {
            get { return this.GetParameterValue<MediaRange>("type"); }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(HttpLink other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!Equals(this.targetUri, other.targetUri))
            {
                return false;
            }

            foreach (var parameter in other.Parameters)
            {
                object parameterValue;
                if (!this.parameters.TryGetValue(parameter.Key, out parameterValue))
                {
                    return false;
                }

                if (parameterValue == null || parameter.Value == null)
                {
                    continue;
                }

                if (!parameterValue.Equals(parameter.Value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return other is HttpLink && this.Equals((HttpLink)other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var p = 0;

                foreach (var parameter in this.parameters)
                {
                    p ^= StringComparer.OrdinalIgnoreCase.GetHashCode(parameter.Key);
                    p ^= parameter.Value != null ? parameter.Value.GetHashCode() : 0;
                }

                var u = this.targetUri != null
                    ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.targetUri.ToString())
                    : 0;

                return (p * 397) ^ u;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var linkBuilder = new StringBuilder();
            linkBuilder.Append('<');
            linkBuilder.Append(this.TargetUri);
            linkBuilder.Append('>');

            var parameters = this.parameters
                .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Key))
                .OrderBy(parameter => parameter.Key, ParameterComparer);

            foreach (var parameter in parameters)
            {
                linkBuilder.Append("; ");
                linkBuilder.Append(parameter.Key);

                if (parameter.Value == null)
                {
                    continue;
                }

                linkBuilder.Append('=');
                linkBuilder.Append('"');
                linkBuilder.Append(parameter.Value);
                linkBuilder.Append('"');
            }

            return linkBuilder.ToString();
        }

        private T GetParameterValue<T>(string parameterName)
        {
            object parameterValue;
            if (!this.parameters.TryGetValue(parameterName, out parameterValue))
            {
                return default(T);
            }

            return (T)parameterValue;
        }

        /// <summary>
        /// Parses the specified <paramref name="uri"/> string into a <see cref="TargetUri"/>.
        /// </summary>
        /// <param name="uri">The URI string.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">uri</exception>
        private static Uri ParseUri(string uri)
        {
            Uri parsedUri;
            // Mono workaround. See http://www.mono-project.com/docs/faq/known-issues/urikind-relativeorabsolute/ for details. @asbjornu
            var uriKind = uri.StartsWith("/") ? UriKind.Relative : UriKind.RelativeOrAbsolute;
            if (!Uri.TryCreate(uri, uriKind, out parsedUri))
            {
                throw new ArgumentException(string.Format("Can't parse '{0}' into an URI.", uri), "uri");
            }

            return parsedUri;
        }

        private class HttpLinkParameterComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (ReferenceEquals(x, null))
                {
                    return -1;
                }

                if (x == "rel")
                {
                    return -1;
                }

                return string.Compare(x, y, StringComparison.Ordinal);
            }
        }
    }
}
