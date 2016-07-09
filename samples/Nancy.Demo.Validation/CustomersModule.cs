namespace Nancy.Demo.Validation
{
    using System.Linq;
    using Nancy.Demo.Validation.Database;
    using Nancy.Demo.Validation.Models;
    using Nancy.ModelBinding;
    using Nancy.Validation;

    public class CustomersModule : NancyModule
    {
        public CustomersModule() : base("/customers")
        {
            Get("/", args =>
            {
                var model = DB.Customers.OrderBy(e => e.RenewalDate).ToArray();

                return View["Customers", model];
            });

            Get("/poke", args =>
            {
                var validator =
                    this.ValidatorLocator.GetValidatorForType(typeof(Customer));

                return this.Response.AsJson(validator.Description);
            });

            Post<dynamic>("/", args =>
            {
                Customer model = this.Bind();
                var result = this.Validate(model);

                if (!result.IsValid)
                {
                    return View["CustomerError", result];
                }

                DB.Customers.Add(model);

                return this.Response.AsRedirect("/Customers");
            });
        }
    }
}