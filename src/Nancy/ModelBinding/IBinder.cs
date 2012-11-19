namespace Nancy.ModelBinding
{
    using System;

    /// <summary>
    /// Binds incoming request data to a model type
    /// </summary>
    public interface IBinder
    {
        /// <summary>
        /// Bind to the given model type
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="modelType">Model type to bind to</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blackList">Blacklisted property names</param>
        /// <param name="instance">Existing instance of the object</param>
        /// <returns>Bound model</returns>
        object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList);
    }

    public class BindingConfig
    {
        /// <summary>
        /// Binding configuration that permitts that the binder overwrites non-default values.
        /// </summary>
        public static BindingConfig AllowOverwrite = new BindingConfig { Overwrite = true };

        /// <summary>
        /// Default binding configuration.
        /// </summary>
        public static BindingConfig Default = new BindingConfig();

        /// <summary>
        /// Gets or sets whether the binder is allowed to overwrite properties that does not have a default value.
        /// </summary>
        /// <returns><see langword="true" /> if the binder is allowed to overwrite non-default values, otherwise <see langword="false" />.</returns>
        public bool Overwrite { get; set; }
    }
}