namespace Nancy.ViewEngines
{
    public static class StaticViewEngineExtension
    {
        public static Response Static(this IViewEngine engine, string virtualPath)
        {
            return Response.WriteFile(virtualPath);
        }
    }
}