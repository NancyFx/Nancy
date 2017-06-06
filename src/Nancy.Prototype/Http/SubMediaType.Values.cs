namespace Nancy.Prototype.Http
{
    using System;
    using System.Collections.Generic;

    public partial struct SubMediaType
    {
        private static readonly IDictionary<string, SubMediaType> SubTypes;

        static SubMediaType()
        {
            SubTypes = new Dictionary<string, SubMediaType>(StringComparer.OrdinalIgnoreCase)
            {
                { "*", Wildcard },
                { "json", Json },
                { "xml", Xml },
                { "plain", Plain },
                { "html", Html }
            };
        }

        public static SubMediaType Html { get; } = new SubMediaType("html");

        public static SubMediaType Json { get; } = new SubMediaType("json");

        public static SubMediaType Plain { get; } = new SubMediaType("plain");

        public static SubMediaType Wildcard { get; } = new SubMediaType("*");

        public static SubMediaType Xml { get; } = new SubMediaType("xml");

        public static SubMediaType FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Wildcard;
            }

            var trimmedValue = value.Trim();

            if (trimmedValue.Length == 0)
            {
                return Wildcard;
            }

            SubMediaType subType;
            if (!SubTypes.TryGetValue(trimmedValue, out subType))
            {
                SubTypes.Add(trimmedValue, subType = ParseString(trimmedValue));
            }

            return subType;
        }

        private static SubMediaType ParseString(string value)
        {
            var suffixIndex = value.IndexOf('+');

            if (suffixIndex == -1)
            {
                return new SubMediaType(value);
            }

            var parts = value.Split(SuffixSeparator, 2);

            return new SubMediaType(parts[0], parts[1]);
        }
    }
}
