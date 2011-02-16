namespace Nancy.Demo
{
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
                var model = new RatPackWithDependencyText
                    { 
                        FirstName = "Bob", 
                        ApplicationDependencyText = this.applicationDependency.GetContent(),
                        RequestDependencyText = this.requestDependency.GetContent()
                    };

                return View["~/views/razor-dependency.cshtml", model];
            };
        }
    }
}