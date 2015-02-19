namespace Nancy.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Nancy.Helpers;
    using Nancy.Routing;

    /// <summary>
    /// Containing extensions for the <see cref="string"/> object.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// A regular expression used to manipulate parameterized route segments.
        /// </summary>
        /// <value>A <see cref="Regex"/> object.</value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Regex ParameterExpression =
            new Regex(@"{(?<name>[A-Za-z0-9_]*)(?:\?(?<default>[A-Za-z0-9_-]*))?}", RegexOptions.Compiled);

        /// <summary>
        /// Extracts information about the parameters in the <paramref name="segment"/>.
        /// </summary>
        /// <param name="segment">The segment that the information should be extracted from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/>, containing <see cref="ParameterSegmentInformation"/> instances for the parameters in the segment.</returns>
        public static IEnumerable<ParameterSegmentInformation> GetParameterDetails(this string segment)
        {
            var matches = ParameterExpression
                .Matches(segment);

            var nameMatch = matches
                .Cast<Match>()
                .ToList();

            return nameMatch.Select(x => new ParameterSegmentInformation(x.Groups["name"].Value, x.Groups["default"].Value, x.Groups["default"].Success));
        }

        /// <summary>
        /// Checks if a segment contains any parameters.
        /// </summary>
        /// <param name="segment">The segment to check for parameters.</param>
        /// <returns>true if the segment contains a parameter; otherwise false.</returns>
        /// <remarks>A parameter is defined as a string which is surrounded by a pair of curly brackets.</remarks>
        /// <exception cref="ArgumentException">The provided value for the segment parameter was null or empty.</exception>
        public static bool IsParameterized(this string segment)
        {
            var parameterMatch =
                ParameterExpression.Match(segment);

            return parameterMatch.Success;
        }

        /// <summary>
        /// Gets a dynamic dictionary back from a Uri query string
        /// </summary>
        /// <param name="queryString">The query string to extract values from</param>
        /// <returns>A dynamic dictionary containing the query string values</returns>
        public static DynamicDictionary AsQueryDictionary(this string queryString)
        {
            var coll = HttpUtility.ParseQueryString(queryString);
            var ret = new DynamicDictionary();

            var found = 0;
            foreach (var key in coll.AllKeys.Where(key => key != null))
            {
                ret[key] = coll[key];

                found++;

                if (found >= StaticConfiguration.RequestQueryFormMultipartLimit)
                {
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Converts the value from PascalCase to camelCase.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ToCamelCase(this string value)
        {
            return value.ConvertFirstCharacter(x => x.ToLowerInvariant());
        }

        /// <summary>
        /// Converts the value from camelCase to PascalCase.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ToPascalCase(this string value)
        {
            return value.ConvertFirstCharacter(x => x.ToUpperInvariant());
        }

        private static string ConvertFirstCharacter(this string value, Func<string, string> converter)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return string.Concat(converter(value.Substring(0, 1)), value.Substring(1));
        }
    }
}
