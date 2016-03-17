namespace Nancy
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the link relation in an RFC 6899 Link HTTP Header.
    /// </summary>
    /// <seealso cref="System.Uri" />
    [Serializable]
    public class HttpLinkRelation : Uri, IEquatable<HttpLinkRelation>
    {
        /// <summary>
        /// The URI prefix to use for IANA registered link relations.
        /// </summary>
        public static readonly Uri IanaLinkRelationPrefix = new Uri("http://www.iana.org/assignments/relation/");


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
            this.Prefix = prefix;
            this.Value = value;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLinkRelation"/> class.
        /// </summary>
        /// <param name="serializationInfo">An instance of the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> class containing the information required to serialize the new <see cref="T:System.Uri" /> instance.</param>
        /// <param name="streamingContext">An instance of the <see cref="T:System.Runtime.Serialization.StreamingContext" /> class containing the source of the serialized stream associated with the new <see cref="T:System.Uri" /> instance.</param>
        protected HttpLinkRelation(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }


        /// <summary>
        /// Gets the <see cref="Uri"/> prefix for the link relation. Will be set to <see cref="IanaLinkRelationPrefix"/>
        /// if the <see cref="Value"/> is a relative one.
        /// </summary>
        /// <value>
        /// The <see cref="Uri"/> prefix for the link relation.
        /// </value>
        public Uri Prefix { get; }

        /// <summary>
        /// Gets the link relation value.
        /// </summary>
        /// <value>
        /// The link relation value.
        /// </value>
        public string Value { get; }


        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        bool IEquatable<HttpLinkRelation>.Equals(HttpLinkRelation other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            var selfString = this.ToString();
            var otherString = other.ToString();
            return string.Compare(selfString, otherString, StringComparison.InvariantCultureIgnoreCase) == 0;
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
            return ((IEquatable<HttpLinkRelation>)this).Equals(other as HttpLinkRelation);
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
                throw new ArgumentNullException(nameof(relation));
            }

            Uri parsedRelation;

            if (TryCreate(relation, UriKind.Absolute, out parsedRelation))
            {
                if (parsedRelation.AbsolutePath.Length < 2
                    || !parsedRelation.AbsolutePath.Contains("/")
                    || parsedRelation.AbsolutePath.EndsWith("/"))
                {
                    throw new FormatException($"The link relation '{relation}' is invalid.");
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

            throw new FormatException($"The link relation '{relation}' is invalid.");
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Prefix == IanaLinkRelationPrefix
                ? this.Value
                : base.ToString();
        }
    }
}
