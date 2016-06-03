namespace Nancy
{
    using System;

    /// <summary>
    /// The relation that identifies the semantics of a <see cref="HttpLink"/> contained in an RFC 5988 Link HTTP header,
    /// as built by the <see cref="HttpLinkBuilder"/>.
    /// </summary>
    /// <seealso cref="System.Uri" />
    public class HttpLinkRelation : Uri, IEquatable<HttpLinkRelation>
    {
        /// <summary>
        /// The URI prefix to use for IANA registered link relations.
        /// </summary>
        public static readonly Uri IanaLinkRelationPrefix = new Uri("http://www.iana.org/assignments/relation/");

        private readonly Uri prefix;

        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLinkRelation"/> class.
        /// </summary>
        /// <param name="relation">The relation.</param>
        public HttpLinkRelation(string relation)
            : this(Parse(relation).Prefix, Parse(relation).Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLinkRelation" /> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="value">The value.</param>
        public HttpLinkRelation(Uri prefix, string value)
            : base(prefix + value)
        {
            this.prefix = prefix;
            this.value = value;
        }


        /// <summary>
        /// Gets the <see cref="Uri"/> prefix for the link relation. Will be set to <see cref="IanaLinkRelationPrefix"/>
        /// if the <see cref="Value"/> is a relative one.
        /// </summary>
        /// <value>
        /// The <see cref="Uri"/> prefix for the link relation.
        /// </value>
        public Uri Prefix
        {
            get { return this.prefix; }
        }

        /// <summary>
        /// Gets the link relation value.
        /// </summary>
        /// <value>
        /// The link relation value.
        /// </value>
        public string Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(HttpLinkRelation other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return StringComparer.OrdinalIgnoreCase.Equals(this.ToString(), other.ToString());
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
            return other is HttpLinkRelation && this.Equals((HttpLinkRelation)other);
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
                var p = this.prefix != null
                    ? this.prefix.GetHashCode()
                    : 0;

                var v = this.value != null
                    ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.value)
                    : 0;

                return (p * 397) ^ v;
            }
        }

        /// <summary>
        /// Parses the specified link <paramref name="relation" /> name into an absolute <see cref="Uri" />.
        /// Will be prefixed with <see cref="IanaLinkRelationPrefix" /> if the <paramref name="relation" /> is
        /// a relative value.
        /// </summary>
        /// <param name="relation">The link relation name.</param>
        /// <returns>
        /// A new instance of <see cref="HttpLinkRelation" /> from the parsed link <paramref name="relation"/> value.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="relation"/></exception>
        /// <exception cref="System.ArgumentException"><paramref name="relation"/></exception>
        public static HttpLinkRelation Parse(string relation)
        {
            if (string.IsNullOrEmpty(relation))
            {
                throw new ArgumentNullException("relation");
            }

            Uri parsedRelation;

            if (TryCreate(relation, UriKind.Absolute, out parsedRelation))
            {
                if (parsedRelation.AbsolutePath.Length < 2
                    || !parsedRelation.AbsolutePath.Contains("/")
                    || parsedRelation.AbsolutePath.EndsWith("/"))
                {
                    throw new FormatException(string.Format("The link relation '{0}' is invalid.", relation));
                }

                var slashIndex = relation.LastIndexOf('/');
                var prefix = relation.Substring(0, slashIndex + 1);
                var value = relation.Substring(slashIndex + 1);
                var prefixUri = new Uri(prefix);

                return new HttpLinkRelation(prefixUri, value);
            }

            if (TryCreate(IanaLinkRelationPrefix + relation, UriKind.Absolute, out parsedRelation))
            {
                return new HttpLinkRelation(IanaLinkRelationPrefix, relation);
            }

            throw new FormatException(string.Format("The link relation '{0}' is invalid.", relation));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.prefix == IanaLinkRelationPrefix
                ? this.Value
                : base.ToString();
        }
    }
}
