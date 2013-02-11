namespace Nancy.ViewEngines.Razor
{
    /// <summary>
    /// Razor HTML Helper Extensions
    /// </summary>
    public static class HtmlHelpersExtensions
    {
        /// <summary>
        /// Create a hidden input field called X-HTTP-Method-Override for the specified <paramref name="method"/>
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="helpers">A reference to the <see cref="HtmlHelpers{TModel}"/> instance.</param>
        /// <param name="method">The HTTP method to use.</param>
        /// <returns>A string representation of the input field.</returns>
        public static IHtmlString HttpMethodOverride<T>(this HtmlHelpers<T> helpers, string method)
        {
            var tag =
                string.Format("<input name='X-HTTP-Method-Override' type='hidden' value='{0}' />", method);

            return new NonEncodedHtmlString(tag);
        }
    }
}