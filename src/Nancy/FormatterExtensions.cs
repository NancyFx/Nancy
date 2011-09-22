namespace Nancy
{
    using Nancy.Responses;

    public static class FormatterExtensions
    {
        public static Response AsFile(this IResponseFormatter formatter, string filePath)
        {            
            return new GenericFileResponse(filePath);
        }

        public static Response AsCss(this IResponseFormatter formatter, string filePath)
        {
            return AsFile(formatter, filePath);
        }

        public static Response AsImage(this IResponseFormatter formatter, string imagePath)
        {            
            return AsFile(formatter, imagePath);
        }

        public static Response AsJs(this IResponseFormatter formatter, string filePath)
        {
            return AsFile(formatter, filePath);
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