namespace Nancy.Formatters
{
    using Responses;

    public static class FormatterExtensions
    {
        public static Response AsImage(this IResponseFormatter formatter, string imagePath)
        {
            return new ImageResponse(imagePath);
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