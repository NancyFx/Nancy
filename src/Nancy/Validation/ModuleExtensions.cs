namespace Nancy.Validation
{
    /// <summary>
    /// Extensions to <see cref="NancyModule"/> for validation.
    /// </summary>
    public static class ModuleExtensions
    {
        /// <summary>
        /// Performs validation on the specified <paramref name="instance"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="instance"/> that is being validated.</typeparam>
        /// <param name="module">The module that the validation is performed from.</param>
        /// <param name="instance">The instance that is being validated.</param>
        /// <returns>A <see cref="ModelValidationResult"/> instance.</returns>
        public static ModelValidationResult Validate<T>(this NancyModule module, T instance)
        {
            var validator = 
                module.ValidatorLocator.GetValidatorForType(typeof(T));

            return (validator == null) ? 
                ModelValidationResult.Valid : 
                validator.Validate(instance);
        }
    }
}