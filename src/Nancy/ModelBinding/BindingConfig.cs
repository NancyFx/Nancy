namespace Nancy.ModelBinding
{
    /// <summary>
    /// Configurations that controls the behavior of the binder at runtime.
    /// </summary>
    public class BindingConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingConfig"/> class.
        /// </summary>
        public BindingConfig()
        {
            this.Overwrite = true;
        }

        /// <summary>
        /// Binding configuration that permitts that the binder overwrites non-default values.
        /// </summary>
        public static BindingConfig NoOverwrite = new BindingConfig { Overwrite = false };

        /// <summary>
        /// Default binding configuration.
        /// </summary>
        public static BindingConfig Default = new BindingConfig();

        /// <summary>
        /// Gets or sets whether the binder is allowed to overwrite properties that does not have a default value.
        /// </summary>
        /// <returns><see langword="true" /> if the binder is allowed to overwrite non-default values, otherwise <see langword="false" />.</returns>
        public bool Overwrite { get; set; }

        /// <summary>
        /// Gets or sets whether the binder should be happy once it has bound to the request body. This setting
        /// only applies if there is a body to bind to in the first place. With a body, the binder will proceed
        /// with other available binding sources regardless of this setting.
        /// </summary>
        /// <returns><see langword="true" /> if the binder will stop once the body has been bound, otherwise <see langword="false" />.</returns>
        public bool BodyOnly { get; set; }
    }
}