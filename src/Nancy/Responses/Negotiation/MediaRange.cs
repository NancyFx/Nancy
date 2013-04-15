namespace Nancy.Responses.Negotiation
{
    using System;

    /// <summary>
    /// Represents a media range from an accept header
    /// </summary>
    public class MediaRange
    {
        /// <summary>
        /// Media range type
        /// </summary>
        public MediaType Type { get; set; }

        /// <summary>
        /// Media range subtype
        /// </summary>
        public MediaType Subtype { get; set; }

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
        /// Creates a MediaRange from a "Type/SubType" string
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static MediaRange FromString(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentException("inputString cannot be null or empty", contentType);
            }

            if (contentType.Equals("*"))
            {
                contentType = "*/*";
            }

            var parts = contentType.Split('/');

            if (parts.Length != 2)
            {
                {
                    throw new ArgumentException("inputString not in correct Type/SubType format", contentType);
                }
            }

            return new MediaRange { Type = parts[0], Subtype = parts[1] };
        }

        public static implicit operator MediaRange(string contentType)
        {
            return MediaRange.FromString(contentType);
        }

        public static implicit operator string(MediaRange mediaRange)
        {
            return string.Concat(mediaRange.Type, "/", mediaRange.Subtype);
        }

        public override string ToString()
        {
            return this;
        }
    }
}