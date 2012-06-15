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
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0}/{1}", Type, Subtype);
        }

        /// <summary>
        /// Creates a MediaRange from a "Type/SubType" string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static MediaRange FromString(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                throw new ArgumentException("inputString cannot be null or empty", inputString);
            }

            var parts = inputString.Split('/');

            if (parts.Length != 2)
            {
                throw new ArgumentException("inputString not in correct Type/SubType format", inputString);
            }

            return new MediaRange { Type = parts[0], Subtype = parts[1] };
        }

        public static implicit operator MediaRange(Tuple<string, string> inputTuple)
        {
            return new MediaRange { Type = inputTuple.Item1, Subtype = inputTuple.Item2 };
        }

        public static implicit operator Tuple<string, string>(MediaRange inputRange)
        {
            return new Tuple<string, string>(inputRange.Type, inputRange.Subtype);
        }

        public bool Equals(MediaRange other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Type.Equals(other.Type) && this.Subtype.Equals(other.Subtype);
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
            if (obj.GetType() != typeof(MediaRange))
            {
                return false;
            }
            return Equals((MediaRange)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Type.GetHashCode() * 397) ^ this.Subtype.GetHashCode();
            }
        }
    }
}