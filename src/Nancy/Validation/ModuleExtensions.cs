namespace Nancy.Validation
{
    /// <summary>
    /// Extensions to NancyModule for validation.
    /// </summary>
    public static class ModuleExtensions
    {
        public static ValidationResult Validate<T>(this NancyModule module, T instance)
        {
            var validator = module.ValidatorLocator.GetValidatorForType(typeof(T));
            if (validator == null)
            {
                //TODO: what should we do here?  User is requesting validation for an unconfigured type...
                return ValidationResult.Valid;
            }

            return validator.Validate(instance);
        }
    }
}