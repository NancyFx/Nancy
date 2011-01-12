using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.ViewEngines.Razor;
using Nancy.Demo.Models;

namespace Nancy.Demo
{
    public interface IDependency
    {
        string GetContent();
    }

    public class DependencyClass : IDependency
    {
        #region IDependency Members

        public string GetContent()
        {
            return "And this text comes from a dependency!";
        }

        #endregion
    }

    public class DependencyModule : NancyModule
    {
        private IDependency _Dependency;

        /// <summary>
        /// Initializes a new instance of the DependencyModule class.
        /// </summary>
        /// <param name="dependency"></param>
        public DependencyModule(IDependency dependency)
        {
            _Dependency = dependency;

            Get["/dependency"] = x =>
            {
                var model = new RatPackWithDependencyText() { FirstName = "Bob", DependencyText = _Dependency.GetContent() };
                return View.Razor("~/views/razor-dependency.cshtml", model);
            };
        }
    }
}