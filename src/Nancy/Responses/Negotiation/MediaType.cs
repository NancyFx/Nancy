namespace Nancy.Responses.Negotiation
{
    using System;

    /// <summary>
    /// Represents a media type or subtype in a <see cref="MediaRange"/>
    /// </summary>
    public class MediaType
    {
        private readonly string typeString;

        /// <summary>
        /// Gets a value indicating whether the media type is a wildcard or not
        /// </summary>
        public bool IsWildcard
        {
            get
            {
                return this.typeString != null && string.Equals(this.typeString, "*", StringComparison.Ordinal);
            }
        }

        public MediaType(string typeString)
        {
            this.typeString = typeString;
        }

        public static implicit operator MediaType(string inputString)
        {
            return new MediaType(inputString);
        }

        public static implicit operator string(MediaType inputMediaType)
        {
            return inputMediaType.typeString;
        }

        public bool Equals(MediaType other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.IsWildcard || 
                   other.IsWildcard || 
                   this.typeString.Equals(other.typeString, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(MediaType))
            {
                return false;
            }
            return Equals((MediaType)obj);
        }

        public override int GetHashCode()
        {
            return (this.typeString != null ? this.typeString.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return this.typeString;
        }
    }
}