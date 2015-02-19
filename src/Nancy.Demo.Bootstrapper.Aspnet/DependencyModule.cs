namespace Nancy.Demo.Bootstrapping.Aspnet
{
    using Nancy.Demo.Bootstrapping.Aspnet.Models;

    public class DependencyModule : NancyModule
    {
        private readonly IApplicationDependency applicationDependency;
        private readonly IRequestDependency requestDependency;

        public DependencyModule(IApplicationDependency applicationDependency, IRequestDependency requestDependency)
        {
            this.applicationDependency = applicationDependency;
            this.requestDependency = requestDependency;

            Get["/"] = x => {
                var model =
                    new RatPackWithDependencyText
                    {
                        FirstName = "Bob",
                        ApplicationDependencyText = this.applicationDependency.GetContent(),
                        RequestDependencyText = this.requestDependency.GetContent()
                    };

                return View["razor-dependency", model];
            };
        }
    }
}