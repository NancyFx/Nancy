namespace Nancy.Prototype.Http
{
    using System;
    using System.Collections.Generic;

    public partial struct MediaType
    {
        private static readonly IDictionary<string, MediaType> Types;

        static MediaType()
        {
            Types = new Dictionary<string, MediaType>(StringComparer.OrdinalIgnoreCase)
            {
                { "*", Wildcard },
                { "application", Application },
                { "text", Text }
            };
        }

        public static MediaType Application { get; } = new MediaType("application");

        public static MediaType Text { get; } = new MediaType("text");

        public static MediaType Wildcard { get; } = new MediaType("*");

        public static MediaType FromString(string value)
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

            MediaType mediaType;
            if (!Types.TryGetValue(trimmedValue, out mediaType))
            {
                Types.Add(trimmedValue, mediaType = new MediaType(trimmedValue));
            }

            return mediaType;
        }
    }
}
