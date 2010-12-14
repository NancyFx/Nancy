namespace Nancy.Extensions
{
    using Nancy.Routing;

    public static class RouteDescriptionExtensions
    {
        public static string GetModuleQualifiedPath(this RouteDescription description)
        {
            return string.Concat(description.Module.ModulePath, description.Path);
        }
    }
}