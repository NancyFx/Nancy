namespace Nancy.ViewEngines
{
    /// <summary>
    /// The context for which a view is being located.
    /// </summary>
    public class ViewLocationContext
    {
        /// <summary>
        /// The module path of the <see cref="NancyModule"/> that is locating a view.
        /// </summary>
        /// <value>A <see cref="string"/> containing the module path.</value>
        public string ModulePath { get; set; }

        /// <summary>
        /// The name of the <see cref="NancyModule"/> that is locating a view.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the module.</value>
        public string ModuleName { get; set; }

        /// <summary>
        /// The request/response context
        /// </summary>
        public NancyContext Context{ get; set; }
    }
}