namespace Nancy.Validation.DataAnnotations
{
    using System.ComponentModel.DataAnnotations;

    public class FileExtensionsValidatorAdapter : DataAnnotationsValidatorAdapter
    {
        public FileExtensionsValidatorAdapter()
            : base("FileExtension")
        {
        }

        public override bool CanHandle(ValidationAttribute attribute)
        {
            return attribute is FileExtensionsAttribute;
        }
    }
}
