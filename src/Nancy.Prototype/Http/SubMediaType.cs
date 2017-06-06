namespace Nancy.Prototype.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    [DebuggerDisplay("{ToString(), nq}")]
    public partial struct SubMediaType : IEquatable<SubMediaType>
    {
        private static readonly char[] FacetSeparator = { '.' };

        private static readonly char[] SuffixSeparator = { '+' };

        private SubMediaType(string value)
            : this(value, SubMediaTypeSuffix.Empty)
        {
        }

        private SubMediaType(string value, SubMediaTypeSuffix suffix)
        {
            this.Value = value;
            this.Facets = value.Split(FacetSeparator);
            this.Suffix = suffix;
        }

        public IReadOnlyList<string> Facets { get; }

        public SubMediaTypeSuffix Suffix { get; }

        public string Value { get; }

        public bool IsExperimental => this.FirstFacetEquals("x");

        public bool IsPersonal => this.FirstFacetEquals("prs");

        public bool IsVendor => this.FirstFacetEquals("vnd");

        public bool IsWildcard => this.Value.Equals("*", StringComparison.OrdinalIgnoreCase);

        public bool Equals(SubMediaType other)
        {
            return string.Equals(this.Value, other.Value, StringComparison.OrdinalIgnoreCase) && this.Suffix.Equals(other.Suffix);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is SubMediaType && this.Equals((SubMediaType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StringComparer.OrdinalIgnoreCase.GetHashCode(this.Value) * 397) ^ this.Suffix.GetHashCode();
            }
        }

        public bool Matches(SubMediaType other)
        {
            return this.IsWildcard || other.IsWildcard || this.Equals(other);
        }

        public static bool operator ==(SubMediaType left, SubMediaType right)
        {
            return Equals(left, right);
        }

        public static implicit operator SubMediaType(string value)
        {
            return FromString(value);
        }

        public static bool operator !=(SubMediaType left, SubMediaType right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var builder = new StringBuilder(this.Value);

            if (this.Suffix.HasValue)
            {
                builder.Append('+').Append(this.Suffix);
            }

            return builder.ToString();
        }

        private bool FirstFacetEquals(string facet)
        {
            return this.Facets.Count > 0 && this.Facets[0].Equals(facet, StringComparison.OrdinalIgnoreCase);
        }
    }
}
