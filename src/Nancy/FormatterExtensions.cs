namespace Nancy
{
    using System;
    using System.IO;
    using System.Text;
    using Extensions;
    using Responses;

    /// <summary>
    /// Various extensions to return different responses form a <see cref="NancyModule"/>.
    /// </summary>
    public static class FormatterExtensions
    {
        private static ISerializer jsonSerializer;

        private static ISerializer xmlSerializer;

        /// <summary>
        /// Sends the file at <paramref name="applicationRelativeFilePath"/> to the
        /// agent, using <paramref name="contentType"/> for the <c>Content-Type</c> header.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="applicationRelativeFilePath">The application relative file path.</param>
        /// <param name="contentType">Value for the <c>Content-Type</c> header.</param>
        public static Response AsFile(this IResponseFormatter formatter, string applicationRelativeFilePath, string contentType)
        {
            return new GenericFileResponse(applicationRelativeFilePath, contentType);
        }

        /// <summary>
        /// Sends the file at <paramref name="applicationRelativeFilePath"/> to the
        /// agent, using the file extension and <see cref="MimeTypes.GetMimeType"/>
        /// to determine the <c>Content-Type</c> header.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="applicationRelativeFilePath">The application relative file path.</param>
        public static Response AsFile(this IResponseFormatter formatter, string applicationRelativeFilePath)
        {
            return new GenericFileResponse(applicationRelativeFilePath);
        }

        /// <summary>
        /// Returns the <paramref name="contents"/> string to the
        /// agent, using <paramref name="contentType"/> and <paramref name="encoding"/>
        /// for the <c>Content-Type</c> header.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="contents">The contents of the response.</param>
        /// <param name="contentType">Value for the <c>Content-Type</c> header.</param>
        /// <param name="encoding">The encoding to use.</param>
        public static Response AsText(this IResponseFormatter formatter, string contents, string contentType, Encoding encoding)
        {
            return new TextResponse(contents, contentType, encoding);
        }

        /// <summary>
        /// Returns the <paramref name="contents"/> string to the
        /// agent, using <c>text/plain</c> and <paramref name="encoding"/>
        /// for the <c>Content-Type</c> header.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="contents">The contents of the response.</param>
        /// <param name="encoding">The encoding to use.</param>
        public static Response AsText(this IResponseFormatter formatter, string contents, Encoding encoding)
        {
            return new TextResponse(contents, encoding: encoding);
        }

        /// <summary>
        /// Returns the <paramref name="contents"/> string to the
        /// agent, using <paramref name="contentType"/> for the <c>Content-Type</c> header.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="contents">The contents of the response.</param>
        /// <param name="contentType">Value for the <c>Content-Type</c> header.</param>
        public static Response AsText(this IResponseFormatter formatter, string contents, string contentType)
        {
            return new TextResponse(contents, contentType);
        }

        /// <summary>
        /// Returns the <paramref name="contents"/> string as a <c>text/plain</c> response to the agent.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="contents">The contents of the response.</param>
        public static Response AsText(this IResponseFormatter formatter, string contents)
        {
            return new TextResponse(contents);
        }

        /// <summary>
        /// Serializes the <paramref name="model"/> to JSON and returns it to the
        /// agent, optionally using the <paramref name="statusCode"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="model">The model to serialize.</param>
        /// <param name="statusCode">The HTTP status code. Defaults to <see cref="HttpStatusCode.OK"/>.</param>
        public static Response AsJson<TModel>(this IResponseFormatter formatter, TModel model, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var serializer = jsonSerializer ?? (jsonSerializer = formatter.SerializerFactory.GetSerializer("application/json"));

            return new JsonResponse<TModel>(model, serializer, formatter.Environment)
            {
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Returns a redirect response to the agent.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="location">The location to redirect to.</param>
        /// <param name="type">The redirect type. See <see cref="RedirectResponse.RedirectType"/>.</param>
        public static Response AsRedirect(this IResponseFormatter formatter, string location, RedirectResponse.RedirectType type = RedirectResponse.RedirectType.SeeOther)
        {
            return new RedirectResponse(formatter.Context.ToFullPath(location), type);
        }

        /// <summary>
        /// Serializes the <paramref name="model"/> to XML and returns it to the
        /// agent, optionally using the <paramref name="statusCode"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="formatter">The formatter.</param>
        /// <param name="model">The model to serialize.</param>
        /// <param name="statusCode">The HTTP status code. Defaults to <see cref="HttpStatusCode.OK"/>.</param>
        public static Response AsXml<TModel>(this IResponseFormatter formatter, TModel model, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var serializer = xmlSerializer ?? (xmlSerializer = formatter.SerializerFactory.GetSerializer("application/xml"));

            return new XmlResponse<TModel>(model, serializer, formatter.Environment);
        }

        /// <summary>
        /// Writes the data from the given <paramref name="stream"/> to the
        /// agent, using <paramref name="contentType"/> for the <c>Content-Type</c> header.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="stream">The stream to copy from.</param>
        /// <param name="contentType">Value for the <c>Content-Type</c> header.</param>
        public static Response FromStream(this IResponseFormatter formatter, Stream stream, string contentType)
        {
            return new StreamResponse(() => stream, contentType);
        }

        /// <summary>
        /// Invokes the given <paramref name="streamDelegate"/> to write the stream data to the
        /// agent, using <paramref name="contentType"/> for the <c>Content-Type</c> header.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="streamDelegate">A delegate returning a stream to copy from.</param>
        /// <param name="contentType">Value for the <c>Content-Type</c> header.</param>
        public static Response FromStream(this IResponseFormatter formatter, Func<Stream> streamDelegate, string contentType)
        {
            return new StreamResponse(streamDelegate, contentType);
        }
    }
}
