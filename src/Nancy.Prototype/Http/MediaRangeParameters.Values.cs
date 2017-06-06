namespace Nancy.Prototype.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial struct MediaRangeParameters
    {
        public static MediaRangeParameters Empty { get; } = new MediaRangeParameters(
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

        public static MediaRangeParameters FromString(string value)
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

            var dictionary = trimmedValue
                .Split(ParameterSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(KeyValueSeparator))
                .ToDictionary(x => x[0].Trim(), x => x[1].Trim());

            return new MediaRangeParameters(dictionary);
        }
    }
}
