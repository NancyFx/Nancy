namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Nancy.Validation;

    /// <summary>
    /// A convenience class that contains various extension methods for modules.
    /// </summary>
    public static class ModuleExtensions
    {
        private static readonly string[] NoBlacklistedProperties = ArrayCache.Empty<string>();

        /// <summary>
        /// Parses an array of expressions like <code>t =&gt; t.Property</code> to a list of strings containing the property names;
        /// </summary>
        /// <typeparam name="T">Type of the model</typeparam>
        /// <param name="expressions">Expressions that tell which property should be ignored</param>
        /// <returns>Array of strings containing the names of the properties.</returns>
        private static string[] ParseBlacklistedPropertiesExpressionTree<T>(this IEnumerable<Expression<Func<T, object>>> expressions)
        {
            return expressions.Select(p => p.GetTargetMemberInfo().Name).ToArray();
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <param name="module">Current module</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Model adapter - cast to a model type to bind it</returns>
        public static dynamic Bind(this INancyModule module, params string[] blacklistedProperties)
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
        public static dynamic Bind(this INancyModule module, BindingConfig configuration, params string[] blacklistedProperties)
        {
            return new DynamicModelBinderAdapter(module.ModelBinderLocator, module.Context, null, configuration, blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this INancyModule module)
        {
            return module.Bind();
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this INancyModule module, params string[] blacklistedProperties)
        {
            return module.Bind(blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="blacklistedProperties">Expressions that tell which property should be ignored</param>
        /// <example>this.Bind&lt;Person&gt;(p =&gt; p.Name, p =&gt; p.Age)</example>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this INancyModule module, params Expression<Func<TModel, object>>[] blacklistedProperties)
        {
            return module.Bind<TModel>(blacklistedProperties.ParseBlacklistedPropertiesExpressionTree());
        }

        /// <summary>
        /// Bind the incoming request to a model and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Bound model instance</returns>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindAndValidate<TModel>(this INancyModule module, params string[] blacklistedProperties)
        {
            var model = module.Bind<TModel>(blacklistedProperties);
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to a model and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="blacklistedProperties">Expressions that tell which property should be ignored</param>
        /// <example>this.Bind&lt;Person&gt;(p =&gt; p.Name, p =&gt; p.Age)</example>
        /// <returns>Bound model instance</returns>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindAndValidate<TModel>(this INancyModule module, params Expression<Func<TModel, object>>[] blacklistedProperties)
        {
            var model = module.Bind<TModel>(blacklistedProperties.ParseBlacklistedPropertiesExpressionTree());
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to a model and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <returns>Bound model instance</returns>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindAndValidate<TModel>(this INancyModule module)
        {
            var model = module.Bind<TModel>(NoBlacklistedProperties);
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this INancyModule module, BindingConfig configuration)
        {
            return module.Bind(configuration);
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this INancyModule module, BindingConfig configuration, params string[] blacklistedProperties)
        {
            return module.Bind(configuration, blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperty">Expressions that tell which property should be ignored</param>
        /// <example>this.Bind&lt;Person&gt;(p =&gt; p.Name, p =&gt; p.Age)</example>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this INancyModule module, BindingConfig configuration, Expression<Func<TModel, object>> blacklistedProperty)
        {
            return module.Bind(configuration, new [] { blacklistedProperty }.ParseBlacklistedPropertiesExpressionTree());
        }

        /// <summary>
        /// Bind the incoming request to a model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperties">Expressions that tell which property should be ignored</param>
        /// <example>this.Bind&lt;Person&gt;(p =&gt; p.Name, p =&gt; p.Age)</example>
        /// <returns>Bound model instance</returns>
        public static TModel Bind<TModel>(this INancyModule module, BindingConfig configuration, params Expression<Func<TModel, object>>[] blacklistedProperties)
        {
            return module.Bind(configuration, blacklistedProperties.ParseBlacklistedPropertiesExpressionTree());
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
        public static TModel BindAndValidate<TModel>(this INancyModule module, BindingConfig configuration, params string[] blacklistedProperties)
        {
            var model = module.Bind<TModel>(configuration, blacklistedProperties);
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to a model and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperties">Expressions that tell which property should be ignored</param>
        /// <example>this.Bind&lt;Person&gt;(p =&gt; p.Name, p =&gt; p.Age)</example>
        /// <returns>Bound model instance</returns>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindAndValidate<TModel>(this INancyModule module, BindingConfig configuration, params Expression<Func<TModel, object>>[] blacklistedProperties)
        {
            var model = module.Bind<TModel>(configuration, blacklistedProperties.ParseBlacklistedPropertiesExpressionTree());
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to a model and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <returns>Bound model instance</returns>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindAndValidate<TModel>(this INancyModule module, BindingConfig configuration)
        {
            var model = module.Bind<TModel>(configuration, NoBlacklistedProperties);
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
        public static TModel BindTo<TModel>(this INancyModule module, TModel instance, params string[] blacklistedProperties)
        {
            return module.BindTo(instance, BindingConfig.NoOverwrite, blacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to an existing instance
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="blacklistedProperties">Expressions that tell which property should be ignored</param>
        /// <example>this.Bind&lt;Person&gt;(p =&gt; p.Name, p =&gt; p.Age)</example>
        public static TModel BindTo<TModel>(this INancyModule module, TModel instance, params Expression<Func<TModel, object>>[] blacklistedProperties)
        {
            return module.BindTo(instance, BindingConfig.NoOverwrite, blacklistedProperties.ParseBlacklistedPropertiesExpressionTree());
        }

        /// <summary>
        /// Bind the incoming request to an existing instance
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        public static TModel BindTo<TModel>(this INancyModule module, TModel instance)
        {
            return module.BindTo(instance, BindingConfig.NoOverwrite, NoBlacklistedProperties);
        }

        /// <summary>
        /// Bind the incoming request to an existing instance and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="blacklistedProperties">Property names to blacklist from binding</param>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindToAndValidate<TModel>(this INancyModule module, TModel instance, params string[] blacklistedProperties)
        {
            var model = module.BindTo(instance, blacklistedProperties);
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to an existing instance and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="blacklistedProperties">Expressions that tell which property should be ignored</param>
        /// <example>this.Bind&lt;Person&gt;(p =&gt; p.Name, p =&gt; p.Age)</example>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindToAndValidate<TModel>(this INancyModule module, TModel instance, params Expression<Func<TModel, object>>[] blacklistedProperties)
        {
            var model = module.BindTo(instance, blacklistedProperties.ParseBlacklistedPropertiesExpressionTree());
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to an existing instance and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindToAndValidate<TModel>(this INancyModule module, TModel instance)
        {
            var model = module.BindTo(instance, NoBlacklistedProperties);
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
        public static TModel BindTo<TModel>(this INancyModule module, TModel instance, BindingConfig configuration, params string[] blacklistedProperties)
        {
            dynamic adapter =
                new DynamicModelBinderAdapter(module.ModelBinderLocator, module.Context, instance, configuration, blacklistedProperties);

            return adapter;
        }

        /// <summary>
        /// Bind the incoming request to an existing instance
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperties">Expressions that tell which property should be ignored</param>
        /// <example>this.Bind&lt;Person&gt;(p =&gt; p.Name, p =&gt; p.Age)</example>
        public static TModel BindTo<TModel>(this INancyModule module, TModel instance, BindingConfig configuration, params Expression<Func<TModel, object>>[] blacklistedProperties)
        {
            return module.BindTo(instance, configuration, blacklistedProperties.ParseBlacklistedPropertiesExpressionTree());
        }

        /// <summary>
        /// Bind the incoming request to an existing instance
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        public static TModel BindTo<TModel>(this INancyModule module, TModel instance, BindingConfig configuration)
        {
            return module.BindTo(instance, configuration, NoBlacklistedProperties);
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
        public static TModel BindToAndValidate<TModel>(this INancyModule module, TModel instance, BindingConfig configuration, params string[] blacklistedProperties)
        {
            var model = module.BindTo(instance, configuration, blacklistedProperties);
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to an existing instance and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blacklistedProperties">Expressions that tell which property should be ignored</param>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        /// <example>this.BindToAndValidate(person, config, p =&gt; p.Name, p =&gt; p.Age)</example>
        public static TModel BindToAndValidate<TModel>(this INancyModule module, TModel instance, BindingConfig configuration, params Expression<Func<TModel, object>>[] blacklistedProperties)
        {
            var model = module.BindTo(instance, configuration, blacklistedProperties.ParseBlacklistedPropertiesExpressionTree());
            module.Validate(model);
            return model;
        }

        /// <summary>
        /// Bind the incoming request to an existing instance and validate
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="module">Current module</param>
        /// <param name="instance">The class instance to bind properties to</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <remarks><see cref="ModelValidationResult"/> is stored in NancyModule.ModelValidationResult and NancyContext.ModelValidationResult.</remarks>
        public static TModel BindToAndValidate<TModel>(this INancyModule module, TModel instance, BindingConfig configuration)
        {
            var model = module.BindTo(instance, configuration, NoBlacklistedProperties);
            module.Validate(model);
            return model;
        }
    }
}
