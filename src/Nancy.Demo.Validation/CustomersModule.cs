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
            Get["/"] = x =>
            {
                var model = DB.Customers.OrderBy(e => e.RenewalDate).ToArray();

                return View["Customers", model];
            };

            Get["/poke"] = parameters =>
            {
                var validator =
                    this.ValidatorLocator.GetValidatorForType(typeof(Customer));

                return this.Response.AsJson(validator.Description);
            };

            Post["/"] = x =>
            {
                Customer model = this.Bind();
                var result = this.Validate(model);

                if (!result.IsValid)
                {
                    return View["CustomerError", result];
                }

                DB.Customers.Add(model);

                return this.Response.AsRedirect("/Customers");
            };
        }
    }
}