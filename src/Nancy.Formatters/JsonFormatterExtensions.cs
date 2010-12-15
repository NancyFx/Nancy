namespace Nancy.Formatters
{
    using Responses;

    public static class JsonFormatterExtensions
    {
        public static Response AsJson<TModel>(this IResponseFormatter formatter, TModel model)
        {
            return new JsonResponse<TModel>(model);
        }
    }
}