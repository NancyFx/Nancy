namespace Nancy
{
    using System.IO;
    using Nancy.Responses;
    using System.Collections.Generic;

    public static class FormatterExtensions
    {
        public static Response AsFile(this IResponseFormatter formatter, string filePath)
        {
            return new GenericFileResponse(Path.Combine(formatter.RootPath, filePath));
        }

        public static Response AsFile(this IResponseFormatter formatter, Stream stream)
        {
            return new GenericFileResponse(stream);
        }

        public static Response AsAttachment(this IResponseFormatter formatter, string filePath)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>() {
            { "Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(filePath) } 
            };
            return new GenericFileResponse(Path.Combine(formatter.RootPath, filePath), headers);
        }
        public static Response AsAttachment(this IResponseFormatter formatter, string filePath, string fileName)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>() {
            { "Content-Disposition", "attachment; filename=" + fileName } 
            };
            return new GenericFileResponse(Path.Combine(formatter.RootPath, filePath), headers);
        }

        public static Response AsAttachment(this IResponseFormatter formatter, Stream stream, string fileName)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>() {
            { "Content-Disposition", "attachment; filename=" + fileName } 
            };
            return new GenericFileResponse(stream, headers);
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