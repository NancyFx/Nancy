namespace Nancy.Testing
{
    using Nancy.ViewEngines;

    /// <summary>
    /// A view factory decorator, aimed for test,
    /// that exposes some interesting properties about the generated view
    /// </summary>
    public class TestingViewFactory : IViewFactory
    {
        private readonly DefaultViewFactory decoratedViewFactory;

        /// <summary>
        /// Creates the view based on the view factory sent to the constructor
        /// </summary>
        /// <param name="viewFactory">the view factory that is decorated</param>
        public TestingViewFactory(DefaultViewFactory viewFactory)
        {
            this.decoratedViewFactory = viewFactory;
        }

        /// <summary>
        /// Renders the view and then call into the viewfactory 
        /// that the TestingViewFactory is decorating
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        /// <param name="model">The module path of the module that is rendering the view.</param>
        /// <param name="viewLocationContext">A <see cref="ViewLocationContext"/> instance, containing information about the context for which the view is being rendered.</param>
        /// <returns>A response.</returns>
        public Response RenderView(string viewName, dynamic model, ViewLocationContext viewLocationContext)
        {
            // Intercept and store interesting stuff
            viewLocationContext.Context.Items["###ViewModel###"] = model;
            viewLocationContext.Context.Items["###ViewName###"] = viewName;
            viewLocationContext.Context.Items["###ModuleName###"] = viewLocationContext.ModuleName;
            
            //TODO: Cannot get hold of the module path?
            viewLocationContext.Context.Items["###ModulePath###"] = viewLocationContext.ModulePath;

            return this.decoratedViewFactory.RenderView(viewName, model, viewLocationContext);
        }
    }

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
        /// <returns>a model of the <typeparam name="TType">type</typeparam></returns>
        public static TType GetModel<TType>(this BrowserResponse response)
        {
            return (TType)response.Context.Items["###ViewModel###"];
        }

        /// <summary>
        /// Returns the name of the view
        /// </summary>
        /// <param name="response">The <see cref="BrowserResponse"/> that the assert should be made on.</param>
        /// <returns>the name of the view</returns>
        public static string GetViewName(this BrowserResponse response)
        {
            return GetContextValue(response, "###ViewName###");
        }

        /// <summary>
        /// Returns the name of the module
        /// </summary>
        /// <param name="response">The <see cref="BrowserResponse"/> that the assert should be made on.</param>
        /// <returns>the name of the module</returns>
        public static string GetModuleName(this BrowserResponse response)
        {
            return GetContextValue(response, "###ModuleName###");
        }

        /// <summary>
        /// Returns the name of the module
        /// </summary>
        /// <param name="response">The <see cref="BrowserResponse"/> that the assert should be made on.</param>
        /// <returns>the name of the module</returns>
        public static string GetModulePath(this BrowserResponse response)
        {
            return GetContextValue(response, "###ModulePath###");
        }

        private static string GetContextValue(BrowserResponse response, string key)
        {
            if (!response.Context.Items.ContainsKey(key))
                return string.Empty;

            var value = (string)response.Context.Items[key];
            return string.IsNullOrEmpty(value) ? string.Empty : value;
        }
    }
}
