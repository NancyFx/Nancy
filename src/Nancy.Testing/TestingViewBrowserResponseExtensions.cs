namespace Nancy.Testing
{
    /// <summary>
    /// Extension methods for easy access of the properties
    /// stored in the view context by the testing view factory
    /// </summary>
    public static class TestingViewBrowserResponseExtensions
    {
        /// <summary>
        /// Get the model on the view
        /// </summary>
        /// <typeparam name="TType">the type of the model</typeparam>
        /// <param name="response">The <see cref="BrowserResponse"/> that the assert should be made on.</param>
        /// <returns>a model of the <typeparamref name="TType">type</typeparamref></returns>
        /// <remarks>This method requires that the <c>Browser</c> utilize the <see cref="TestingViewFactory"/></remarks>
        public static TType GetModel<TType>(this BrowserResponse response)
        {
            return (TType)response.Context.Items[TestingViewContextKeys.VIEWMODEL];
        }

        /// <summary>
        /// Returns the name of the view
        /// </summary>
        /// <param name="response">The <see cref="BrowserResponse"/> that the assert should be made on.</param>
        /// <returns>the name of the view</returns>
        /// <remarks>This method requires that the <c>Browser</c> utilize the <see cref="TestingViewFactory"/></remarks>
        public static string GetViewName(this BrowserResponse response)
        {
            return GetContextValue(response, TestingViewContextKeys.VIEWNAME);
        }

        /// <summary>
        /// Returns the name of the module
        /// </summary>
        /// <param name="response">The <see cref="BrowserResponse"/> that the assert should be made on.</param>
        /// <returns>the name of the module</returns>
        /// <remarks>This method requires that the <c>Browser</c> utilize the <see cref="TestingViewFactory"/></remarks>
        public static string GetModuleName(this BrowserResponse response)
        {
            return GetContextValue(response, TestingViewContextKeys.MODULENAME);
        }

        /// <summary>
        /// Returns the name of the module
        /// </summary>
        /// <param name="response">The <see cref="BrowserResponse"/> that the assert should be made on.</param>
        /// <returns>the name of the module</returns>
        /// <remarks>This method requires that the <c>Browser</c> utilize the <see cref="TestingViewFactory"/></remarks>
        public static string GetModulePath(this BrowserResponse response)
        {
            return GetContextValue(response, TestingViewContextKeys.MODULEPATH);
        }

        private static string GetContextValue(BrowserResponse response, string key)
        {
            if (!response.Context.Items.ContainsKey(key))
            {
                return string.Empty;
            }

            var value = (string)response.Context.Items[key];
            return string.IsNullOrEmpty(value) ? string.Empty : value;
        }
    }
}
