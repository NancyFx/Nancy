namespace Nancy.ModelBinding
{
    public static class ModuleExtensions
    {
        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <param name="module">Current module</param>
        /// <returns>Model adapter - cast to a model type to bind it</returns>
        public static dynamic Bind(this NancyModule module)
        {
            return new DynamicModelBinderAdapter(module.ModelBinderLocator, module.Context);
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this NancyModule module)
        {
            return (TModel)module.Bind();
        }
    }
}