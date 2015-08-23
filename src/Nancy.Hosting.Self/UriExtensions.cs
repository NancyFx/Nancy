namespace Nancy.Hosting.Self
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Extension methods for working with <see cref="Uri"/> instances.
    /// </summary>
    public static class UriExtensions
    {
        public static bool IsCaseInsensitiveBaseOf(this Uri source, Uri value)
        {
            var uriComponents = source.Host == "localhost" ? (UriComponents.Port | UriComponents.Scheme) : (UriComponents.HostAndPort | UriComponents.Scheme);
            if (Uri.Compare(source, value, uriComponents, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }

            var sourceSegments = source.Segments;
            var valueSegments = value.Segments;

            return sourceSegments.ZipCompare(valueSegments, (s1, s2) => s1.Length == 0 || SegmentEquals(s1, s2));
        }

        public static string MakeAppLocalPath(this Uri appBaseUri, Uri fullUri)
        {
            return string.Concat("/", appBaseUri.Segments.ZipFill(fullUri.Segments, (x, y) => x != null && SegmentEquals(x, y) ? null : y).Join());
        }

        private static string AppendSlashIfNeeded(string segment)
        {
            if (!segment.EndsWith("/"))
            {
                segment = string.Concat(segment, "/");
            }

            return segment;
        }

        private static bool SegmentEquals(string segment1, string segment2)
        {
            return String.Equals(AppendSlashIfNeeded(segment1), AppendSlashIfNeeded(segment2), StringComparison.OrdinalIgnoreCase);
        }

        private static bool ZipCompare(this IEnumerable<string> source1, IEnumerable<string> source2, Func<string, string, bool> comparison)
        {
            using (var enumerator1 = source1.GetEnumerator())
            {
                using (var enumerator2 = source2.GetEnumerator())
                {
                    var has1 = enumerator1.MoveNext();
                    var has2 = enumerator2.MoveNext();

                    while (has1 || has2)
                    {
                        var current1 = has1 ? enumerator1.Current : "";
                        var current2 = has2 ? enumerator2.Current : "";

                        if (!comparison(current1, current2))
                        {
                            return false;
                        }

                        if (has1)
                        {
                            has1 = enumerator1.MoveNext();
                        }

                        if (has2)
                        {
                            has2 = enumerator2.MoveNext();
                        }
                    }

                }
            }

            return true;
        }

        private static IEnumerable<string> ZipFill(this IEnumerable<string> source1, IEnumerable<string> source2, Func<string, string, string> selector)
        {
            using (var enumerator1 = source1.GetEnumerator())
            {
                using (var enumerator2 = source2.GetEnumerator())
                {
                    var has1 = enumerator1.MoveNext();
                    var has2 = enumerator2.MoveNext();

                    while (has1 || has2)
                    {
                        var value1 = has1 ? enumerator1.Current : null;
                        var value2 = has2 ? enumerator2.Current : null;
                        var value = selector(value1, value2);

                        if (value != null)
                        {
                            yield return value;
                        }

                        if (has1)
                        {
                            has1 = enumerator1.MoveNext();
                        }

                        if (has2)
                        {
                            has2 = enumerator2.MoveNext();
                        }
                    }

                }
            }
        }

        private static string Join(this IEnumerable<string> source)
        {
            var builder = new StringBuilder();

            foreach (var value in source)
            {
                builder.Append(value);
            }

            return builder.ToString();
        }
    }
}
