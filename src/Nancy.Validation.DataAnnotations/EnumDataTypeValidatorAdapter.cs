namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// An adapter for the <see cref="EnumDataTypeAttribute"/>.
    /// </summary>
    public class EnumDataTypeValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumDataTypeValidatorAdapter"/> class.
        /// </summary>
        public EnumDataTypeValidatorAdapter()
            : base("EnumDataType")
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
            return attribute is EnumDataTypeAttribute;
        }
    }
}
