using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.ViewEngines.Razor;
using Nancy.Demo.Models;

namespace Nancy.Demo
{
    public class DependencyModule : NancyModule
    {
        private readonly IApplicationDependency _ApplicationDependency;
        private readonly IRequestDependency _RequestDependency;

        /// <summary>
        /// Initializes a new instance of the DependencyModule class.
        /// </summary>
        /// <param name="dependency"></param>
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

                return View.Razor("~/views/razor-dependency.cshtml", model);
            };
        }
    }
}