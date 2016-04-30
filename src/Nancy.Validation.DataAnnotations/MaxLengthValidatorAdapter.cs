namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel.DataAnnotations;

    public class MaxLengthValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        public MaxLengthValidatorAdapter()
            : base("MaxLength")
        {
        }

        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute is MaxLengthAttribute;
        }
    }
}
