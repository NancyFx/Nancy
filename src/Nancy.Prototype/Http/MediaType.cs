namespace Nancy.Prototype.Http
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{ToString(), nq}")]
    public partial struct MediaType : IEquatable<MediaType>
    {
        private MediaType(string value)
        {
            this.Value = value;
        }

        public string Value { get; }

        public bool IsWildcard => this.Value.Equals("*", StringComparison.OrdinalIgnoreCase);

        public bool Equals(MediaType other)
        {
            return string.Equals(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is MediaType && this.Equals((MediaType) obj);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value);
        }

        public bool Matches(MediaType other)
        {
            return this.IsWildcard || other.IsWildcard || this.Equals(other);
        }

        public static bool operator ==(MediaType left, MediaType right)
        {
            return Equals(left, right);
        }

        public static implicit operator MediaType(string value)
        {
            return FromString(value);
        }

        public static bool operator !=(MediaType left, MediaType right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
