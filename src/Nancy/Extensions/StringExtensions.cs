namespace Nancy.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Nancy.Helpers;

    public static class StringExtensions
    {
        /// <summary>
        /// A regular expression used to manipulate parameterized route segments.
        /// </summary>
        /// <value>A <see cref="Regex"/> object.</value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Regex ParameterExpression =
            new Regex(@"^\{(?<name>[A-Za-z0-9]*)\}", RegexOptions.Compiled);

        /// <summary>   
        /// Extracts the name of a parameter from a segment.
        /// </summary>
        /// <param name="segment">The segment to extract the name from.</param>
        /// <returns>A string containing the name of the parameter.</returns>
        /// <exception cref="FormatException"></exception>
        public static string GetParameterName(this string segment)
        {
            var nameMatch =
                ParameterExpression.Match(segment);

            if (nameMatch.Success)
            {
                return nameMatch.Groups["name"].Value;
            }

            throw new FormatException("");
        }

        /// <summary>
        /// Checks if a segement contains any parameters.
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
            foreach (var key in coll.AllKeys.Where(key => key != null)) 
                ret[key] = coll[key];
            return ret;
        }
    }
}