namespace Nancy.Hosting.Self
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extension methods for working with <see cref="Uri"/> instances.
    /// </summary>
    public static class UriExtensions
    {
        public static bool IsCaseInsensitiveBaseOf(this Uri source, Uri url, string rawUrl)
        {
            var uriComponents = source.Host == "localhost" ? (UriComponents.Port | UriComponents.Scheme) : (UriComponents.HostAndPort | UriComponents.Scheme);
            if (Uri.Compare(source, url, uriComponents, UriFormat.Unescaped, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                return false;
            }

            var baseLocalPath = source.LocalPath.TrimEnd('/') + "/";

            var requestPath = Regex.Replace(rawUrl, @"\?.*", String.Empty, RegexOptions.IgnoreCase);
            requestPath = Regex.Replace(requestPath, @"#.*", String.Empty, RegexOptions.IgnoreCase);
            requestPath = requestPath.TrimEnd('/') + "/";

            return requestPath.StartsWith(baseLocalPath, true, CultureInfo.InvariantCulture);
        }

        public static string MapToAppLocalPath(this Uri baseUri, string rawUrl)
        {
            var baseLocalPath = baseUri.LocalPath.TrimEnd('/');
            var path = Regex.Replace(rawUrl, "^" + baseLocalPath, String.Empty, RegexOptions.IgnoreCase);
            path = Regex.Replace(path, @"\?.*", String.Empty, RegexOptions.IgnoreCase);
            path = Regex.Replace(path, @"#.*", String.Empty, RegexOptions.IgnoreCase);

            if (!path.StartsWith("/"))
                path = "/" + path;

            return path;
        }
    }
}
