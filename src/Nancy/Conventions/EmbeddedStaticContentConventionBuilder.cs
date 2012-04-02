using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Nancy.Responses;

namespace Nancy.Conventions
{
	public class EmbeddedStaticContentConventionBuilder
	{
		private static readonly ConcurrentDictionary<string, Func<Response>> ResponseFactoryCache = new ConcurrentDictionary<string, Func<Response>>();

		private static EmbeddedStaticContentConventionBuilder _logger = new EmbeddedStaticContentConventionBuilder();

		private EmbeddedStaticContentConventionBuilder()
		{
		}

		public static Func<NancyContext, string, Response> MapVirtualDirectory(string virtualDirectory, string resourceNamespaceRoot, Assembly assemblyContainingResource)
		{
			return (nancyContext, compositeRootPath) =>
					{
						nancyContext.Trace.TraceLog.WriteLog(x => x.AppendLine(
							string.Format(
								"[EmbeddedStaticContentConventionBuilder] A request was recieved for the embedded resource {0}", nancyContext.Request.Url)));

						var requestedResource =
							nancyContext.Request.Path.TrimStart(new[] { '/' });

						if (!requestedResource.StartsWith(virtualDirectory, StringComparison.OrdinalIgnoreCase))
						{
							return null;
						}

						var responseFactory =
							ResponseFactoryCache.GetOrAdd(requestedResource, buildContentFetchDelegate(nancyContext, virtualDirectory, resourceNamespaceRoot, assemblyContainingResource));

						return responseFactory.Invoke();
					};
		}

		private static Func<string, Func<Response>> buildContentFetchDelegate(NancyContext nancyContext, string virtualDirectory, string resourceNamespaceRoot, Assembly assemblyContainingResource)
		{
			return requestPath =>
					{
						nancyContext.Trace.TraceLog.WriteLog(x => x.AppendLine(
							string.Format(
								"[EmbeddedStaticContentConventionBuilder] The resource {0} is not in the cache, fetching from assembly manifest",
								nancyContext.Request.Url)));

						var rgx = new Regex(virtualDirectory, RegexOptions.IgnoreCase);

						var fullyQualifiedResourceName = rgx.Replace(requestPath, resourceNamespaceRoot, 1).Replace("/", ".");

						nancyContext.Trace.TraceLog.WriteLog(x => x.AppendLine(
							string.Format(
								"[EmbeddedStaticContentConventionBuilder] Looking for embedded resource named {0}", fullyQualifiedResourceName)));

						if (!assemblyContainingResource.GetManifestResourceNames().Any(n=>n.Equals(fullyQualifiedResourceName, StringComparison.InvariantCulture)))
						{
							nancyContext.Trace.TraceLog.WriteLog(x => x.AppendLine(
								string.Format(
									"[EmbeddedStaticContentConventionBuilder] Unable to retrieve resource named {0} from Assembly resource manifest",
									fullyQualifiedResourceName)));

							return () => null;
						}

						var ext = Path.GetExtension(nancyContext.Request.Path);

						return () => new StreamResponse(() => assemblyContainingResource.GetManifestResourceStream(fullyQualifiedResourceName), MimeTypes.GetMimeType(requestPath));
					};
		}
	}
}