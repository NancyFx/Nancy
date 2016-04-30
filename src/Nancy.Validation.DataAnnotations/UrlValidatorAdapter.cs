namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel.DataAnnotations;

    public class UrlValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        public UrlValidatorAdapter()
            : base("Url")
        {
        }

        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute is UrlAttribute;
        }
    }
}
