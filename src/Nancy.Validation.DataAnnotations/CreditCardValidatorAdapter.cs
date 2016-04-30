namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel.DataAnnotations;

    public class CreditCardValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        public CreditCardValidatorAdapter()
            : base("CreditCard")
        {
        }

        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute is CreditCardAttribute;
        }
    }
}
