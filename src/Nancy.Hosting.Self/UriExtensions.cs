using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Nancy.Hosting.Self
{
	public static class UriExtensions
	{
		public static bool IsCaseInsensitiveBaseOf(this Uri source, Uri value)
		{
			
			if (Uri.Compare(source, value,UriComponents.Scheme | UriComponents.HostAndPort, UriFormat.Unescaped, StringComparison.InvariantCultureIgnoreCase) != 0)
				return false;

			var sourceSegments = source.Segments;
			var valueSegments = value.Segments;

			return sourceSegments.ZipCompare(valueSegments,
			                                 (s1, s2) =>
			                                 	{
			                                 		if (s1.Length == 0) // s2 is longer that s1
			                                 			return true;

			                                 		// s2 should exaclty match s1 directory parts
			                                 		return SegmentEquals(s1, s2);
			                                 	}
				);
		}

		private static bool ZipCompare(this IEnumerable<string> source1, IEnumerable<string> source2, Func<string, string, bool> comparison)
		{
			using (var enumerator1 = source1.GetEnumerator())
			using (var enumerator2 = source2.GetEnumerator())
			{
				bool has1 = enumerator1.MoveNext();
				bool has2 = enumerator2.MoveNext();

				while (has1 || has2)
				{
					string current1 = has1 ? enumerator1.Current : "";
					string current2 = has2 ? enumerator2.Current : "";

					if (!comparison(current1, current2))
						return false;

					if (has1)
						has1 = enumerator1.MoveNext();
					if (has2)
						has2 = enumerator2.MoveNext();
				}

			}

			return true;
		}

		private static bool SegmentEquals(string segment1, string segment2)
		{
			return String.Equals(AppendSlashIfNeeded(segment1), AppendSlashIfNeeded(segment2), StringComparison.InvariantCultureIgnoreCase);
		}

		private static string AppendSlashIfNeeded(string segment)
		{
			if (!segment.EndsWith("/"))
				segment = segment + "/";
			return segment;
		}


		public static string MakeAppLocalPath(this Uri appBaseUri, Uri fullUri)
		{
			string relativepath = "/" + appBaseUri.Segments.ZipFill(fullUri.Segments,
			                              (x, y) => x != null && AppendSlashIfNeeded(x) == AppendSlashIfNeeded(y) ? null : y).Join();
			
			return relativepath;
		}

		private static IEnumerable<string> ZipFill(this IEnumerable<string> source1, IEnumerable<string> source2, Func<string, string, string> selector)
		{
			using (var enumerator1 = source1.GetEnumerator())
			using (var enumerator2 = source2.GetEnumerator())
			{
				bool has1 = enumerator1.MoveNext();
				bool has2 = enumerator2.MoveNext();

				while (has1 || has2)
				{
					var value1 = has1 ? enumerator1.Current : null;
					var value2 = has2 ? enumerator2.Current : null;
					var value = selector(value1, value2);

					if (value != null)
						yield return value;

					if (has1)
						has1 = enumerator1.MoveNext();
					if (has2)
						has2 = enumerator2.MoveNext();
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