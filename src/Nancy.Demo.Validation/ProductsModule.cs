namespace Nancy.Demo.Validation
{
    using System;
    using ModelBinding;
    using Models;
    using Nancy.Validation;

    public class ProductsModule : NancyModule
    {
        public ProductsModule() : base("/products")
        {
            Get["/"] = parameters => {
                return "Products module";
            };

            Get["/poke"] = parameters => {

                var validator = 
                    this.ValidatorLocator.GetValidatorForType(typeof(Product));

                return Response.AsJson(validator.Description);
            };

            Post["/"] = parameters => {

                Product model = this.Bind();
                var result = this.Validate(model);

                if (!result.IsValid)
                {
                    return View["CustomerError", result];
                }

                return 200;
            };
        }
    }
}