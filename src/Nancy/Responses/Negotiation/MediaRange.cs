namespace Nancy.Responses.Negotiation
{
    using System;

    /// <summary>
    /// Represents a media range from an accept header
    /// </summary>
    public class MediaRange : IEquatable<string>
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

            var parts = contentType.Split('/');

            if (parts.Length != 2)
            {
                throw new ArgumentException("inputString not in correct Type/SubType format", contentType);
            }

            return new MediaRange { Type = parts[0], Subtype = parts[1] };
        }

        public static implicit operator MediaRange(string contentType)
        {
            return MediaRange.FromString(contentType);
        }

        public bool Equals(string other)
        {
            var range = 
                (MediaRange)other;

            return (this.Type.Equals(range.Type) && this.Subtype.Equals(range.Subtype));
        }
    }
}