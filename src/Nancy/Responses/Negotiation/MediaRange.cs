namespace Nancy.Responses.Negotiation
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a media range from an accept header
    /// </summary>
    public class MediaRange : IEquatable<MediaRange>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaRange"/> class from a string representation of a media range
        /// </summary>
        /// <param name="contentType">the content type</param>
        public MediaRange(string contentType)
        {
            this.ParseContentType(contentType);
        }

        private void ParseContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                this.Type = string.Empty;
                this.Subtype = string.Empty;
                this.Parameters = new MediaRangeParameters();
                return;
            }

            if (contentType.Equals("*"))
            {
                contentType = "*/*";
            }

            var parts = contentType.Split('/', ';');

            if (parts.Length < 2)
            {
                {
                    throw new ArgumentException("inputString not in correct Type/SubType format", contentType);
                }
            }

            this.Type = parts[0];
            this.Subtype = parts[1].TrimEnd();

            if (parts.Length <= 2)
            {
                this.Parameters = new MediaRangeParameters();
                return;
            }

            var separator = contentType.IndexOf(';');
            this.Parameters = MediaRangeParameters.FromString(contentType.Substring(separator));
        }

        /// <summary>
        /// Media range type
        /// </summary>
        public MediaType Type { get; private set; }

        /// <summary>
        /// Media range subtype
        /// </summary>
        public MediaType Subtype { get; private set; }

        /// <summary>
        /// Media range parameters
        /// </summary>
        public MediaRangeParameters Parameters { get; private set; }

        /// <summary>
        /// Gets a value indicating if the media range is the */* wildcard
        /// </summary>
        public bool IsWildcard
        {
            get
            {
                return this.Type.IsWildcard && this.Subtype.IsWildcard;
            }
        }

        /// <summary>
        /// Whether or not a media range matches another, taking into account wildcards
        /// </summary>
        /// <param name="other">Other media range</param>
        /// <returns>True if matching, false if not</returns>
        public bool Matches(MediaRange other)
        {
            return this.Type.Matches(other.Type) && this.Subtype.Matches(other.Subtype);
        }

        /// <summary>
        /// Whether or not a media range matches another, taking into account wildcards and parameters
        /// </summary>
        /// <param name="other">Other media range</param>
        /// <returns>True if matching, false if not</returns>
        public bool MatchesWithParameters(MediaRange other)
        {
            return this.Matches(other) && this.Parameters.Matches(other.Parameters);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="MediaRange"/>.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator MediaRange(string contentType)
        {
            return new MediaRange(contentType);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="MediaRange"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="mediaRange">The media range.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(MediaRange mediaRange)
        {
            if (mediaRange.Parameters.Any())
            {
                return string.Concat(mediaRange.Type, "/", mediaRange.Subtype, ";", mediaRange.Parameters);
            }

            return string.Concat(mediaRange.Type, "/",  mediaRange.Subtype);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(MediaRange other)
        {
            return this.Matches(other);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this;
        }
    }
}
