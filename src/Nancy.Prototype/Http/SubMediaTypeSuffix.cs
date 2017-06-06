namespace Nancy.Prototype.Http
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{ToString(), nq}")]
    public partial struct SubMediaTypeSuffix : IEquatable<SubMediaTypeSuffix>
    {
        private SubMediaTypeSuffix(string value)
        {
            this.Value = value ?? string.Empty;
        }

        public string Value { get; }

        public bool HasValue => !string.IsNullOrWhiteSpace(this.Value);

        public bool Equals(SubMediaTypeSuffix other)
        {
            if (this.HasValue && other.HasValue)
            {
                return string.Equals(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
            }

            return !this.HasValue && !other.HasValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is SubMediaTypeSuffix && this.Equals((SubMediaTypeSuffix) obj);
        }

        public override int GetHashCode()
        {
            return this.HasValue ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value) : 0;
        }

        public static bool operator ==(SubMediaTypeSuffix left, SubMediaTypeSuffix right)
        {
            return Equals(left, right);
        }

        public static implicit operator SubMediaTypeSuffix(string value)
        {
            return FromString(value);
        }

        public static bool operator !=(SubMediaTypeSuffix left, SubMediaTypeSuffix right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
