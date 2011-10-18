
namespace Nancy.Testing
{
	using System;
	using System.IO;
	using System.Xml.Serialization;
	using Nancy.Json;

    /// <summary>
    /// Defines extensions for the <see cref="NancyContext"/> type.
    /// </summary>
    public static class NancyContextExtensions
    {
		private const string DOCUMENT_WRAPPER_KEY_NAME = "@@@@DOCUMENT_WRAPPER@@@@";
		private const string JSONRESPONSE_KEY_NAME = "@@@@JSONRESPONSE@@@@";
		private const string XMLRESPONSE_KEY_NAME = "@@@@XMLRESPONSE@@@@";

		private static T Cache<T>(NancyContext context, string key, Func<T> getData)
		{
			// We only really want to generate this once, so we'll stick it in the context
			// This isn't ideal, but we don't want to hide the guts of the context from the
			// tests this will have to do.
			if (context.Items.ContainsKey(key))
			{
				return (T)context.Items[key];
			}

			T data = getData.Invoke();
			context.Items[key] = data;
			return data;
		}

		/// <summary>
		/// Returns the HTTP response body, of the specified <see cref="NancyContext"/>, wrapped in an <see cref="DocumentWrapper"/> instance.
		/// </summary>
		/// <param name="context">The <see cref="NancyContext"/> instance that the HTTP response body should be retrieved from.</param>
		/// <returns>A <see cref="DocumentWrapper"/> instance, wrapping the HTTP response body of the context.</returns>
		public static DocumentWrapper DocumentBody(this NancyContext context)
		{
			return Cache(context, DOCUMENT_WRAPPER_KEY_NAME, () =>
			{
				using (var contentsStream = new MemoryStream())
				{
					context.Response.Contents.Invoke(contentsStream);
					contentsStream.Position = 0;
					return new DocumentWrapper(contentsStream);
				}
			});
		}

		public static TModel JsonBody<TModel>(this NancyContext context)
		{
			return Cache(context, JSONRESPONSE_KEY_NAME, () =>
			{
				using (var contentsStream = new MemoryStream())
				{
					context.Response.Contents.Invoke(contentsStream);
					contentsStream.Position = 0;
					using (var contents = new StreamReader(contentsStream))
					{
						var serializer = new JavaScriptSerializer();
						var model = serializer.Deserialize<TModel>(contents.ReadToEnd());
						return model;
					}
				}
			});
		}

		public static TModel XmlBody<TModel>(this NancyContext context)
		{
			return Cache(context, XMLRESPONSE_KEY_NAME, () =>
			{
				using (var contentsStream = new MemoryStream())
				{
					context.Response.Contents.Invoke(contentsStream);
					contentsStream.Position = 0;
					var serializer = new XmlSerializer(typeof (TModel));
					var model = serializer.Deserialize(contentsStream);
					return (TModel) model;
				}
			});
		}
		
	}
}