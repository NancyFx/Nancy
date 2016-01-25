namespace Nancy.Demo.Hosting.Aspnet
{
    using Nancy.Demo.Hosting.Aspnet.Models;

    public class DependencyModule : LegacyNancyModule
    {
        private readonly IApplicationDependency applicationDependency;
        private readonly IRequestDependency requestDependency;

        public DependencyModule(IApplicationDependency applicationDependency, IRequestDependency requestDependency)
        {
            this.applicationDependency = applicationDependency;
            this.requestDependency = requestDependency;

            Get["/dependency"] = x =>{
                var model =
                    new RatPackWithDependencyText
                    {
                        FirstName = "Bob",
                        ApplicationDependencyText = this.applicationDependency.GetContent(),
                        RequestDependencyText = this.requestDependency.GetContent()
                    };

                return View["razor-dependency.cshtml", model];
            };
        }
    }
}