namespace Nancy.Validation.FluentValidation
{
    using global::FluentValidation.Validators;

    /// <summary>
    /// Defines the functionality of a factory for creating <see cref="IFluentAdapter"/> instances.
    /// </summary>
    public interface IFluentAdapterFactory
    {
        /// <summary>
        /// Creates a <see cref="IFluentAdapter"/> instance based on the provided <paramref name="rule"/> and <paramref name="propertyValidator"/>.
        /// </summary>
        /// <param name="propertyValidator">The <see cref="IPropertyValidator"/> for which the adapter should be created.</param>
        /// <returns>An <see cref="IFluentAdapter"/> instance.</returns>
        IFluentAdapter Create(IPropertyValidator propertyValidator);
    }
}