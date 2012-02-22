namespace Nancy.ViewEngines.Razor
{
    public interface IUrlHelpers<TModel>
    {
        /// <summary>
        /// Retrieves the absolute url of the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        string Content(string path);
    }
}