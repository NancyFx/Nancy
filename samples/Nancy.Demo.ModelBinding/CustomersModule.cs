namespace Nancy.BindingDemo
{
    using System.Linq;

    using Nancy.Demo.ModelBinding.Database;
    using Nancy.Demo.ModelBinding.Models;
    using Nancy.ModelBinding;

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
                    var model2 = this.Bind<Customer>();

                    DB.Customers.Add(model);
                    DB.Customers.Add(model2);

                    return this.Response.AsRedirect("/Customers");
                };
        }
    }
}