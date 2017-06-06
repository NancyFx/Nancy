namespace Nancy.Prototype.Http
{
    using System;
    using System.Collections.Generic;

    public partial struct MediaRange
    {
        private static readonly char[] TypeSeparator = { '/' };

        private static readonly IDictionary<string, MediaRange> Ranges;

        static MediaRange()
        {
            Ranges = new Dictionary<string, MediaRange>(StringComparer.OrdinalIgnoreCase)
            {
                { "*/*", Wildcard },
                { "application/json", ApplicationJson },
                { "application/xml", ApplicationXml },
                { "text/plain", TextPlain },
                { "text/html", TextHtml },
                { "text/xml", TextXml }
            };
        }

        public static MediaRange ApplicationJson { get; } = new MediaRange(MediaType.Application, SubMediaType.Json);

        public static MediaRange ApplicationXml { get; } = new MediaRange(MediaType.Application, SubMediaType.Xml);

        public static MediaRange TextHtml { get; } = new MediaRange(MediaType.Text, SubMediaType.Html);

        public static MediaRange TextPlain { get; } = new MediaRange(MediaType.Text, SubMediaType.Plain);

        public static MediaRange TextXml { get; } = new MediaRange(MediaType.Text, SubMediaType.Xml);

        public static MediaRange Wildcard { get; } = new MediaRange(MediaType.Wildcard, SubMediaType.Wildcard);

        public static MediaRange FromString(string value)
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

            var valueString = trimmedValue;
            var parameterString = string.Empty;

            var index = trimmedValue.IndexOf(';');

            if (index != -1)
            {
                valueString = trimmedValue.Substring(0, index).Trim();
                parameterString = trimmedValue.Substring(index, trimmedValue.Length - index).Trim();
            }

            if (valueString.Equals("*", StringComparison.OrdinalIgnoreCase))
            {
                valueString = "*/*";
            }

            MediaRange range;
            if (!Ranges.TryGetValue(valueString, out range))
            {
                Ranges.Add(valueString, range = ParseString(valueString));
            }

            if (parameterString.Length == 0)
            {
                return range;
            }

            return range.WithParameters(MediaRangeParameters.FromString(parameterString));
        }

        private static MediaRange ParseString(string value)
        {
            var parts = value.Split(TypeSeparator, 2);

            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid media range format.", nameof(value));
            }

            return new MediaRange(parts[0], parts[1]);
        }
    }
}
