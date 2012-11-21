namespace Nancy.ModelBinding
{
    /// <summary>
    /// Configurations that controls the behavior of the binder at runtime.
    /// </summary>
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