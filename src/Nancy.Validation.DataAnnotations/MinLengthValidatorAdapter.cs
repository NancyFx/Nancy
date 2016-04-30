namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel.DataAnnotations;

    public class MinLengthValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        public MinLengthValidatorAdapter()
            : base("MinLength")
        {
        }

        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute is MinLengthAttribute;
        }
    }
}
