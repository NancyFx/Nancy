namespace Nancy.Prototype.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    [DebuggerDisplay("{ToString(), nq}")]
    public partial struct MediaRange : IEquatable<MediaRange>
    {
        public MediaRange(MediaType type, SubMediaType subType)
            : this(type, subType, MediaRangeParameters.Empty)
        {
        }

        public MediaRange(MediaType type, SubMediaType subType, MediaRangeParameters parameters)
        {
            this.Type = type;
            this.SubType = subType;
            this.Parameters = parameters;
        }

        public MediaRangeParameters Parameters { get; }

        public SubMediaType SubType { get; }

        public MediaType Type { get; }

        public Encoding Charset => GetCharset(this.Parameters);

        public bool IsWildcard => this.Type.IsWildcard && this.SubType.IsWildcard;

        public MediaRange WithParameters(MediaRangeParameters parameters)
        {
            return new MediaRange(this.Type, this.SubType, parameters);
        }

        public MediaRange WithCharset(Encoding encoding)
        {
            var parameters = new Dictionary<string, string>(capacity: this.Parameters.Count);

            foreach (var parameter in this.Parameters)
            {
                parameters[parameter.Key] = parameter.Value;
            }

            parameters["charset"] = encoding.WebName;

            var newParameters = new MediaRangeParameters(parameters);

            return new MediaRange(this.Type, this.SubType, newParameters);
        }

        public bool Equals(MediaRange other)
        {
            return this.Type.Equals(other.Type)
                && this.SubType.Equals(other.SubType)
                && this.Parameters.Equals(other.Parameters);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is MediaRange && this.Equals((MediaRange) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Parameters.GetHashCode();
                hashCode = (hashCode * 397) ^ this.SubType.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Type.GetHashCode();
                return hashCode;
            }
        }

        public bool Matches(MediaRange other)
        {
            return this.Type.Matches(other.Type) && this.SubType.Matches(other.SubType);
        }

        public bool MatchesWithParameters(MediaRange other)
        {
            return this.Matches(other) && this.Parameters.Matches(other.Parameters);
        }

        public static bool operator ==(MediaRange left, MediaRange right)
        {
            return Equals(left, right);
        }

        public static implicit operator MediaRange(string value)
        {
            return FromString(value);
        }

        public static bool operator !=(MediaRange left, MediaRange right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            if (this.Parameters.IsEmpty)
            {
                return $"{this.Type}/{this.SubType}";
            }

            return $"{this.Type}/{this.SubType}; {this.Parameters}";
        }

        private static Encoding GetCharset(MediaRangeParameters parameters)
        {
            string charset;
            if (parameters.TryGetValue("charset", out charset))
            {
                try
                {
                    return Encoding.GetEncoding(charset);
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
