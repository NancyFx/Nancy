namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel.DataAnnotations;

    public class PhoneValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        public PhoneValidatorAdapter()
            : base("Phone")
        {
        }

        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute is PhoneAttribute;
        }
    }
}