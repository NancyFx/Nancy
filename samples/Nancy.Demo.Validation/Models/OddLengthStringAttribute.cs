namespace Nancy.Demo.Validation.Models
{
    using System.ComponentModel.DataAnnotations;

    public class OddLengthStringAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && ((string)value).Length % 2 != 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(
                this.ErrorMessage = string.Format("The value of {0} was not an odd length", validationContext.MemberName),
                new[] { validationContext.MemberName }
                );
        }
    }
}