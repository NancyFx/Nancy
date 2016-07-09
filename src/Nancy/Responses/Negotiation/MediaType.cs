namespace Nancy.Responses.Negotiation
{
    using System;

    /// <summary>
    /// Represents a media type or subtype in a <see cref="MediaRange"/>.
    /// </summary>
    public class MediaType
    {
        private readonly string type;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaType"/> class for the media type part.
        /// </summary>
        /// <param name="type"></param>
        public MediaType(string type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets a value indicating whether the media type is a wildcard or not
        /// </summary>
        /// <value><see langword="true" /> if the media type is a wildcard, otherwise <see langword="false" />.</value>
        public bool IsWildcard
        {
            get
            {
                return this.type != null && string.Equals(this.type, "*", StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Matched the media type with another media type.
        /// </summary>
        /// <param name="other">The media type that should be matched against.</param>
        /// <returns><see langword="true" /> if the media types match, otherwise <see langword="false" />.</returns>
        public bool Matches(MediaType other)
        {
            return this.IsWildcard ||
                   other.IsWildcard ||
                   this.type.Equals(other.type, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator MediaType(string inputString)
        {
            return new MediaType(inputString);
        }

        public static implicit operator string(MediaType inputMediaType)
        {
            return inputMediaType.type;
        }

        public override string ToString()
        {
            return this.type;
        }
    }
}