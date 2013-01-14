namespace Nancy.ModelBinding
{
    using Nancy.Validation;

    public static class ModuleExtensions
    {
        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <param name="module">Current module</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Model adapter - cast to a model type to bind it</returns>
        public static dynamic Bind(this NancyModuleBase module, params string[] blacklistedProperties)
        {
            return module.Bind(BindingConfig.Default, blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <param name="module">Current module</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Model adapter - cast to a model type to bind it</returns>
        public static dynamic Bind(this NancyModuleBase module, BindingConfig configuration, params string[] blacklistedProperties)
        {
            return new DynamicModelBinderAdapter(module.ModelBinderLocator, module.Context, null, configuration, blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this NancyModuleBase module, params string[] blacklistedProperties)
        {
            return module.Bind(blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to a model and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Bound model instance</returns>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindAndValidate<TModel>(this NancyModuleBase module, params string[] blacklistedProperties)
        {
            var model = module.Bind<TModel>(blacklistedProperties);
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this NancyModuleBase module, BindingConfig configuration, params string[] blacklistedProperties)
        {
            return module.Bind(configuration, blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to a model and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Bound model instance</returns>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindAndValidate<TModel>(this NancyModuleBase module, BindingConfig configuration, params string[] blacklistedProperties)
        {
            var model = module.Bind<TModel>(configuration, blacklistedProperties);
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to an existing instance
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        public static TModel BindTo<TModel>(this NancyModuleBase module, TModel instance, params string[] blacklistedProperties)
        {
            return module.BindTo(instance, BindingConfig.Default, blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to an existing instance and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindToAndValidate<TModel>(this NancyModuleBase module, TModel instance, params string[] blacklistedProperties)
        {
            var model = module.BindTo<TModel>(instance, blacklistedProperties);
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to an existing instance
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        public static TModel BindTo<TModel>(this NancyModuleBase module, TModel instance, BindingConfig configuration, params string[] blacklistedProperties)
        {
            dynamic adapter =
                new DynamicModelBinderAdapter(module.ModelBinderLocator, module.Context, instance, configuration, blacklistedProperties);

            return adapter;
        }

        /// <summary>
        /// Bind the incoming request to an existing instance and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindToAndValidate<TModel>(this NancyModuleBase module, TModel instance, BindingConfig configuration, params string[] blacklistedProperties)
        {
            var model = module.BindTo<TModel>(instance, configuration, blacklistedProperties);
            module.Validate(model);
            return model;
        }
    }
}