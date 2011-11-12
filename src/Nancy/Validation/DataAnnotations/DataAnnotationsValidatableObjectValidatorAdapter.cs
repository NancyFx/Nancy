namespace Nancy.Validation.DataAnnotations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// An adapter for an IValidatableObject.
    /// </summary>
    public class DataAnnotationsValidatableObjectValidatorAdapter : IDataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Gets the rules.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ValidationRule> GetRules()
        {
            yield return new ValidationRule("Self", s => string.Format("{0} is invalid.", s));
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IEnumerable<ValidationError> Validate(object instance)
        {
            var context = new ValidationContext(instance, null, null);
            var result = ((IValidatableObject)instance).Validate(context);
            return result.Select(r => new ValidationError(r.MemberNames, s => r.ErrorMessage));
        }
    }
}