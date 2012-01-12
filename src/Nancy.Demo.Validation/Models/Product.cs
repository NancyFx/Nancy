namespace Nancy.Demo.Validation.Models
{
    using FluentValidation;

    public class Product
    {
        public string Name { get; set; }

        public int Price { get; set; }
    }

    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(product => product.Name)
            .NotEmpty()
            .Length(1, 10)
            .Matches("[A-Z]*")
            .EmailAddress();

            RuleFor(product => product.Price).ExclusiveBetween(10, 15);
            RuleFor(product => product.Price).InclusiveBetween(10, 15);
            RuleFor(product => product.Price).Equal(5);
        }
    }
}