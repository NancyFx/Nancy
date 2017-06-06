namespace Nancy.Prototype.Http
{
    using System;
    using System.Collections.Generic;

    public partial struct SubMediaTypeSuffix
    {
        private static readonly IDictionary<string, SubMediaTypeSuffix> Suffixes;

        static SubMediaTypeSuffix()
        {
            Suffixes = new Dictionary<string, SubMediaTypeSuffix>(StringComparer.OrdinalIgnoreCase)
            {
                { "json", Json },
                { "xml", Xml },
                { "zip", Zip }
            };
        }

        public static SubMediaTypeSuffix Empty { get; } = new SubMediaTypeSuffix(string.Empty);

        public static SubMediaTypeSuffix Json { get; } = new SubMediaTypeSuffix("json");

        public static SubMediaTypeSuffix Xml { get; } = new SubMediaTypeSuffix("xml");

        public static SubMediaTypeSuffix Zip { get; } = new SubMediaTypeSuffix("zip");

        public static SubMediaTypeSuffix FromString(string value)
        {
            if (value == null)
            {
                return Empty;
            }

            var trimmedValue = value.Trim();

            if (trimmedValue.Length == 0)
            {
                return Empty;
            }

            SubMediaTypeSuffix suffix;
            if (!Suffixes.TryGetValue(trimmedValue, out suffix))
            {
                Suffixes.Add(trimmedValue, suffix = new SubMediaTypeSuffix(trimmedValue));
            }

            return suffix;
        }
    }
}
