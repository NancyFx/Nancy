namespace Nancy.Prototype.Http
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{ToString(), nq}")]
    public partial struct HttpMethod : IEquatable<HttpMethod>
    {
        private HttpMethod(string value)
        {
            this.Value = value;
        }

        public string Value { get; }

        public bool Equals(HttpMethod other)
        {
            return string.Equals(this.Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj) && obj is HttpMethod && this.Equals((HttpMethod)obj);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value);
        }

        public static bool operator ==(HttpMethod left, HttpMethod right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HttpMethod left, HttpMethod right)
        {
            return !left.Equals(right);
        }

        public static implicit operator HttpMethod(string value)
        {
            return FromString(value);
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
