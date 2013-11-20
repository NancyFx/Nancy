namespace Nancy.Validation
{
    using System.Linq;

    /// <summary>
    /// Extensions to <see cref="INancyModule"/> for validation.
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
        public static ModelValidationResult Validate<T>(this INancyModule module, T instance)
        {
            var validator = 
                module.ValidatorLocator.GetValidatorForType(typeof(T));

            var result = (validator == null) ?
                ModelValidationResult.Valid :
                validator.Validate(instance, module.Context);

            if (module.ModelValidationResult.Errors.Any())
            {
                foreach (var modelValidationError in result.Errors)
                {
                    foreach (var memberName in modelValidationError.MemberNames)
                    {
                        module.ModelValidationResult = module.ModelValidationResult.AddError(memberName, modelValidationError.GetMessage(memberName));
                    }
                }
            }

            module.ModelValidationResult = module.ModelValidationResult.Errors.Any()
                                               ? module.ModelValidationResult
                                               : result;

            return module.ModelValidationResult;
        }
    }
}