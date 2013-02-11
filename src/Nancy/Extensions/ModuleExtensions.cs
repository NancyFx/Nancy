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
        public static string GetModuleName(this INancyModule module)
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

        /// <summary>
        /// Derives the module path from the request path when the module path has parameterized
        /// or regex path variables. 
        /// </summary>
        /// <param name="module">The current module</param>
        /// <param name="context">The current context</param>
        /// <returns>A string containing the derived module path</returns>
        public static string GetModulePath(this NancyModule module, NancyContext context)
        {
            if (string.IsNullOrEmpty(module.ModulePath))
            {
                return string.Empty;
            }
           
            if (!IsParameterizedPath(module) && !IsRegexPath(module))   
            {
                return module.ModulePath;
            }

            var segments = module.ModulePath.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
            var requestPathSegments = context.Request.Path.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);

            if (requestPathSegments.Length < segments.Length)
            {
                return string.Empty;
            }

            return string.Concat("/", string.Join("/", requestPathSegments.Take(segments.Length)));
        }

        private static bool IsRegexPath(NancyModule module)
        {
            return module.ModulePath.Contains("(?");
        }

        private static bool IsParameterizedPath(NancyModule module)
        {
            return module.ModulePath.Contains("{");
        }
    }
}