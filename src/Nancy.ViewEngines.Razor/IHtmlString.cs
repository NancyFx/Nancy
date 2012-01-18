namespace Nancy.ViewEngines.Razor
{
    public interface IHtmlString
    {
        /// <summary>
        /// Returns an HTML-encoded string.
        /// </summary>
        /// <returns>An HTML-encoded string.</returns>
        string ToHtmlString();
    }
}