namespace Nancy.Demo
{
	//Compiles but does not execute as expected under Mono 2.8
	using Nancy.ViewEngines.Razor;
	//using Nancy.ViewEngines.Spark;
	using Nancy.Demo.Models;
	
    public class DependencyModule : NancyModule
    {
        private readonly IApplicationDependency _ApplicationDependency;
        private readonly IRequestDependency _RequestDependency;

        public DependencyModule(IApplicationDependency applicationDependency, IRequestDependency requestDependency)
        {
            _ApplicationDependency = applicationDependency;
            _RequestDependency = requestDependency;

            Get["/dependency"] = x =>
            {
                var model = new RatPackWithDependencyText() 
                    { 
                        FirstName = "Bob", 
                        ApplicationDependencyText = _ApplicationDependency.GetContent(),
                        RequestDependencyText = _RequestDependency.GetContent()
                    };
				
				return View["~/views/razor-dependency.cshtml", model];
            };
        }
    }
}