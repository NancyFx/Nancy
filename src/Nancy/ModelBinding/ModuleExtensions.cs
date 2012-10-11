namespace Nancy.ModelBinding
{
    using System;
    using System.ComponentModel;

    public static class ModuleExtensions
    {
        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <param name="module">Current module</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Model adapter - cast to a model type to bind it</returns>
        public static dynamic Bind(this NancyModule module, params string[] blacklistedProperties)
        {
            return new DynamicModelBinderAdapter(module.ModelBinderLocator, module.Context, null, blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this NancyModule module, params string[] blacklistedProperties)
        {
            return module.Bind(blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to an existing instance
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        public static TModel BindTo<TModel>(this NancyModule module, TModel instance, params string[] blacklistedProperties)
        {
            dynamic adapter = new DynamicModelBinderAdapter(module.ModelBinderLocator, module.Context, instance, blacklistedProperties);
            
            return adapter;
        }
    }
}