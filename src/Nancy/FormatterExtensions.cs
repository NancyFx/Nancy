namespace Nancy
{
    using System.IO;
    using Nancy.Responses;

    public static class FormatterExtensions
    {
        public static Response AsCss(this IResponseFormatter formatter, string filePath)
        {
            var resourcePath =
                Path.Combine(formatter.RootPath, filePath);

            return new StaticFileResponse(resourcePath, "text/css");
        }

        public static Response AsImage(this IResponseFormatter formatter, string imagePath)
        {
            return new ImageResponse(imagePath);
        }

        public static Response AsJs(this IResponseFormatter formatter, string filePath)
        {
            var resourcePath =
                Path.Combine(formatter.RootPath, filePath);

            return new StaticFileResponse(resourcePath, "text/javascript");
        }

        public static Response AsJson<TModel>(this IResponseFormatter formatter, TModel model)
        {
            return new JsonResponse<TModel>(model);
        }

        public static Response AsRedirect(this IResponseFormatter response, string location)
        {
            return new RedirectResponse(location);
        }

        public static Response AsXml<TModel>(this IResponseFormatter formatter, TModel model)
        {
            return new XmlResponse<TModel>(model, "application/xml");
        }
    }
}