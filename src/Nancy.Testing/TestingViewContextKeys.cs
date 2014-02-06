namespace Nancy.Testing
{
    /// <summary>
    /// The key names for where the testing view context data is stored
    /// </summary>
    public static class TestingViewContextKeys
    {
        /// <summary>
        ///  The key in ViewLocationContext.Item for the view model
        /// </summary>
        public const string VIEWMODEL = "__Nancy_Testing_ViewModel";
        /// <summary>
        ///  The key in ViewLocationContext.Item for the view name
        /// </summary>
        public const string VIEWNAME = "__Nancy_Testing_ViewName";
        /// <summary>
        ///  The key in ViewLocationContext.Item for the model name
        /// </summary>
        public const string MODULENAME = "__Nancy_Testing_ModuleName";
        /// <summary>
        ///  The key in ViewLocationContext.Item for the module path        
        /// </summary>
        public const string MODULEPATH = "__Nancy_Testing_ModulePath";
    }
}