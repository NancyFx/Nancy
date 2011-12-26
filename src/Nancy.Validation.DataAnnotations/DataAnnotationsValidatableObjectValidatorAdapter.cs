namespace Nancy.Validation.DataAnnotations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// An adapter for an <see cref="IValidatableObject"/>.
    /// </summary>
    public class DataAnnotationsValidatableObjectValidatorAdapter : IDataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Gets the rules.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelValidationRule> GetRules()
        {
            yield return new ModelValidationRule("Self", s => string.Format("{0} is invalid.", s));
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IEnumerable<ModelValidationError> Validate(object instance)
        {
            var context = 
                new ValidationContext(instance, null, null);

            var result = 
                ((IValidatableObject)instance).Validate(context);

            return result.Select(r => new ModelValidationError(r.MemberNames, s => r.ErrorMessage));
        }
    }
}