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
        /// Binding configuration that permits that the binder overwrites non-default values.
        /// </summary>
        public static readonly BindingConfig NoOverwrite = new BindingConfig { Overwrite = false };

        /// <summary>
        /// Default binding configuration.
        /// </summary>
        public static readonly BindingConfig Default = new BindingConfig();

        /// <summary>
        /// Gets or sets whether the binder should be happy once it has bound to the request body. In this case,
        /// request and context parameters will not be bound to. If there is no body and this option is enabled,
        /// no binding will take place at all.
        /// </summary>
        /// <value><see langword="true" /> if the binder will stop once the body has been bound, otherwise <see langword="false" />.</value>
        public bool BodyOnly { get; set; }

        /// <summary>
        /// Gets or sets whether binding error should be ignored and the binder should continue with the next property.
        /// </summary>
        /// <remarks>Setting this property to <see langword="true" /> means that no <see cref="ModelBindingException"/> will be thrown if an error occurs.</remarks>
        /// <value><see langword="true" />If the binder should ignore errors, otherwise <see langword="false" />.</value>
        public bool IgnoreErrors { get; set; }

        /// <summary>
        /// Gets or sets whether the binder is allowed to overwrite properties that does not have a default value.
        /// </summary>
        /// <value><see langword="true" /> if the binder is allowed to overwrite non-default values, otherwise <see langword="false" />.</value>
        public bool Overwrite { get; set; }
    }
}