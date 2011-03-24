namespace Nancy.Testing
{
    using Tests;

    public static class BrowserContextExtensions
    {
        public static void MultiPartFormData(this BrowserContext browserContext, BrowserContextMultipartFormData multipartFormData)
        {
            var contextValues =
                (IBrowserContextValues)browserContext;

            contextValues.Body = multipartFormData.Body;
            contextValues.Headers["Content-Type"] = new[] { "multipart/form-data; boundary=NancyMultiPartBoundary123124" };
        }
    }
}