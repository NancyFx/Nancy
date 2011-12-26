namespace Nancy.Validation
{
    /// <summary>
    /// Extensions to <see cref="NancyModule"/> for validation.
    /// </summary>
    public static class ModuleExtensions
    {
        public static ModelValidationResult Validate<T>(this NancyModule module, T instance)
        {
            var validator = module.ValidatorLocator.GetValidatorForType(typeof(T));
            if (validator == null)
            {
                //TODO: what should we do here?  User is requesting validation for an unconfigured type...
                return ModelValidationResult.Valid;
            }

            return validator.Validate(instance);
        }
    }
}