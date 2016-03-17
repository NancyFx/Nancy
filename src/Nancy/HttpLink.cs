namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Represents one of possibly many RFC 5899 HTTP Links contained in a <see cref="HttpLinkHeader"/>.
    /// </summary>
    public class HttpLink : Dictionary<string, object>, IEquatable<HttpLink>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="relation"/>
        /// or
        /// <paramref name="uri"/>
        /// </exception>
        public HttpLink(string uri, string relation)
            : this(ParseUri(uri), relation)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="relation" />
        /// or
        /// <paramref name="uri" /></exception>
        public HttpLink(string uri, string relation, MediaRange type)
            : this(ParseUri(uri), relation, type)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <param name="title">The title.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="relation"/>
        /// or
        /// <paramref name="uri"/>
        /// or
        /// <paramref name="title"/>
        /// </exception>
        private HttpLink(string uri, string relation, string title)
            : this(ParseUri(uri), relation, title)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="relation"/>
        /// or
        /// <paramref name="targetUri"/>
        /// </exception>
        public HttpLink(Uri targetUri, string relation)
            : this(targetUri, HttpLinkRelation.Parse(relation))
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="relation" />
        /// or
        /// <paramref name="targetUri" /></exception>
        public HttpLink(Uri targetUri, string relation, MediaRange type)
            : this(targetUri, HttpLinkRelation.Parse(relation), type)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <param name="title">The title.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="relation"/>
        /// or
        /// <paramref name="targetUri"/>
        /// or
        /// <paramref name="title"/>
        /// </exception>
        private HttpLink(Uri targetUri, string relation, string title)
            : this(targetUri, HttpLinkRelation.Parse(relation), title)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <param name="title">The title.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="relation" />
        /// or
        /// <paramref name="targetUri" />
        /// or
        /// <paramref name="title" /></exception>
        private HttpLink(Uri targetUri, string relation, string title, MediaRange type)
            : this(targetUri, HttpLinkRelation.Parse(relation), title, type)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <param name="title">The title.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="relation" />
        /// or
        /// <paramref name="targetUri" /></exception>
        public HttpLink(Uri targetUri, HttpLinkRelation relation, string title)
            : this(targetUri, relation)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            this.Add("title", title);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <param name="title">The title.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="relation" />
        /// or
        /// <paramref name="targetUri" /></exception>
        public HttpLink(Uri targetUri, HttpLinkRelation relation, string title, MediaRange type)
            : this(targetUri, relation, title)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.Add("type", type);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="relation" />
        /// or
        /// <paramref name="targetUri" /></exception>
        public HttpLink(Uri targetUri, HttpLinkRelation relation, MediaRange type)
            : this(targetUri, relation)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.Add("type", type);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLink" /> class.
        /// </summary>
        /// <param name="targetUri">The URI.</param>
        /// <param name="relation">The link relation.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="relation"/>
        /// or
        /// <paramref name="targetUri"/>
        /// </exception>
        public HttpLink(Uri targetUri, HttpLinkRelation relation)
        {
            if (relation == null)
            {
                throw new ArgumentNullException(nameof(relation));
            }

            if (targetUri == null)
            {
                throw new ArgumentNullException(nameof(targetUri));
            }

            this.Add("rel", relation);
            this.TargetUri = targetUri;
        }


        /// <summary>
        /// Gets the link relation.
        /// </summary>
        public HttpLinkRelation Relation
        {
            get { return this.GetParamValue<HttpLinkRelation>("rel"); }
        }

        /// <summary>
        /// Gets the target URI.
        /// </summary>
        public Uri TargetUri { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title
        {
            get { return this.GetParamValue<string>("title"); }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public MediaRange Type
        {
            get { return this.GetParamValue<MediaRange>("type"); }
        }


        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        bool IEquatable<HttpLink>.Equals(HttpLink other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (this.TargetUri != other.TargetUri)
            {
                return false;
            }

            foreach (var parameter in other)
            {
                if (!this.ContainsKey(parameter.Key))
                {
                    return false;
                }

                var parameterValue = this[parameter.Key];

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
            return ((IEquatable<HttpLink>)this).Equals(other as HttpLink);
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().ToLowerInvariant().GetHashCode();
        }


        /// <summary>
        /// Parses the specified RFC 5899 link into a new instance of <see cref="HttpLink"/>.
        /// </summary>
        /// <param name="httpLink">The RFC 5899 link to parse.</param>
        /// <returns>
        /// A new instance of <see cref="HttpLink"/>.
        /// </returns>
        public static HttpLink Parse(string httpLink)
        {
            if (string.IsNullOrEmpty(httpLink))
            {
                throw new ArgumentNullException(nameof(httpLink));
            }

            httpLink = httpLink.Trim();

            var linkParts = httpLink.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string uriValue = null;
            string relationValue = null;
            string type = null;
            string title = null;
            var parameters = new Dictionary<string, string>();

            foreach (var linkPart in linkParts.Select(p => p.Trim()))
            {
                if (linkPart.StartsWith("<") && linkPart.EndsWith(">"))
                {
                    uriValue = linkPart.Substring(1, linkPart.Length - 2);
                    continue;
                }

                var paramParts = linkPart.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (paramParts.Length != 2)
                {
                    continue;
                }

                var paramName = paramParts[0].Trim().ToLowerInvariant();
                var paramValue = paramParts[1];

                if (paramValue.StartsWith("\"") && paramValue.EndsWith("\""))
                {
                    paramValue = paramValue.Substring(1, paramValue.Length - 2);
                }

                switch (paramName)
                {
                    case "rel":
                        relationValue = paramValue;
                        continue;
                    case "title":
                        title = paramValue;
                        continue;
                    case "type":
                        type = paramValue;
                        continue;
                }

                if (parameters.ContainsKey(paramName))
                {
                    parameters[paramName] = paramValue;
                }
                else
                {
                    parameters.Add(paramName, paramValue);
                }
            }

            if (string.IsNullOrEmpty(uriValue))
            {
                throw new FormatException($"The string '{httpLink}' doesn't contain a target URI.");
            }

            if (string.IsNullOrEmpty(relationValue))
            {
                throw new FormatException($"The string '{httpLink}' doesn't contain a 'rel' parameter.");
            }

            var uri = ParseUri(uriValue);
            var relation = HttpLinkRelation.Parse(relationValue);

            var link = Create(title, type, uri, relation);

            foreach (var kvp in parameters)
            {
                link.Add(kvp.Key, kvp.Value);
            }

            return link;
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

            foreach (var parameter in this.OrderBy(parameter => parameter.Key, new LinkParameterComparer()))
            {
                linkBuilder.Append("; ");
                linkBuilder.Append(parameter.Key);
                linkBuilder.Append('=');
                linkBuilder.Append('"');
                linkBuilder.Append(parameter.Value);
                linkBuilder.Append('"');
            }

            return linkBuilder.ToString();
        }


        private static HttpLink Create(string title, string type, Uri uri, HttpLinkRelation relation)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(type))
            {
                return new HttpLink(uri, relation, title, type);
            }

            if (!string.IsNullOrEmpty(title))
            {
                return new HttpLink(uri, relation, title);
            }

            if (!string.IsNullOrEmpty(type))
            {
                return new HttpLink(uri, relation, new MediaRange(type));
            }

            return new HttpLink(uri, relation);
        }


        private T GetParamValue<T>(string paramName)
        {
            object paramValue;
            if (!this.TryGetValue(paramName, out paramValue))
            {
                return default(T);
            }

            return (T)paramValue;
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
            if (!Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out parsedUri))
            {
                throw new ArgumentException(string.Format("Can't parse '{0}' into an URI.", uri), "uri");
            }

            return parsedUri;
        }


        private class LinkParameterComparer : IComparer<string>
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
