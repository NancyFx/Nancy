namespace Nancy.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Nancy.Helpers;

    public static class ModuleExtensions
    {
        /// <summary>
        /// A regular expression used to manipulate parameterized route segments.
        /// </summary>
        /// <value>A <see cref="Regex"/> object.</value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Regex ModuleNameExpression =
            new Regex(@"(?<name>[\w]+)Module$", RegexOptions.Compiled);

        /// <summary>   
        /// Extracts the friendly name of a Nancy module given its type.
        /// </summary>
        /// <param name="name">The type name taken from GetType().Name.</param>
        /// <returns>A string containing the name of the parameter.</returns>
        /// <exception cref="FormatException"></exception>
        public static string GetModuleName(this NancyModule module)
        {
            var typeName = module.GetType().Name;
            var nameMatch =
                ModuleNameExpression.Match(typeName);

            if (nameMatch.Success)
            {
                return nameMatch.Groups["name"].Value;
            }

            return typeName;
        }

    }
}