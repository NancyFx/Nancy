namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// An adapter for the <see cref="PhoneAttribute"/>.
    /// </summary>
    public class PhoneValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneValidatorAdapter"/> class.
        /// </summary>
        public PhoneValidatorAdapter()
            : base("Phone")
        {
        }

        /// <summary>
        /// Gets a boolean that indicates if the adapter can handle the
        /// provided <paramref name="attribute"/>.
        /// </summary>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> that should be handled.</param>
        /// <returns><see langword="true" /> if the attribute can be handles, otherwise <see langword="false" />.</returns>
        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute is PhoneAttribute;
        }
    }
}