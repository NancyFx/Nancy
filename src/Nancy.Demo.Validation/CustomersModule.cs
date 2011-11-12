namespace Nancy.Demo.Validation
{
    using System.Linq;
    using ModelBinding;
    using Nancy.Validation;
    using Database;
    using Models;

    public class CustomersModule : NancyModule
    {
        public CustomersModule()
            : base("/customers")
        {
            Get["/"] = x =>
                { 
                    var model = DB.Customers.OrderBy(e => e.RenewalDate).ToArray();

                    return View["Customers", model];
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
                    return Response.AsRedirect("/Customers");
                };
        }
    }
}