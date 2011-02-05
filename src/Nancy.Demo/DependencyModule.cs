namespace Nancy.Demo
{
    using Nancy.ViewEngines.Razor;
    using Nancy.Demo.Models;

    public class DependencyModule : NancyModule
    {
        private readonly IApplicationDependency applicationDependency;
        private readonly RequestDependency requestDependency;

        public DependencyModule(IApplicationDependency applicationDependency, RequestDependency requestDependency)
        {
            this.applicationDependency = applicationDependency;
            this.requestDependency = requestDependency;

            Get["/dependency"] = x =>
            {
                var model = new RatPackWithDependencyText() 
                    { 
                        FirstName = "Bob", 
                        ApplicationDependencyText = this.applicationDependency.GetContent(),
                        RequestDependencyText = this.requestDependency.GetContent()
                    };

                return View.Razor("~/views/razor-dependency.cshtml", model);
            };
        }
    }
}