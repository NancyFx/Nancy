namespace Nancy.Demo.Validation.Models
{
    using System.ComponentModel.DataAnnotations;

    using Nancy.Validation.DataAnnotations;

    public class OddLengthStringAttributeAdapter : DataAnnotationsValidatorAdapter
    {
        public OddLengthStringAttributeAdapter() : base("Compare")
        {
        }

        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute.GetType() == typeof(OddLengthStringAttribute);
        }
    }
}