namespace Nancy.Demo.Validation.Models
{
    using FluentValidation;

    public class Product
    {
        public string Name { get; set; }
    }

    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(product => product.Name).NotEmpty();
            RuleFor(product => product.Name).Length(1, 10);
        }
    }
}