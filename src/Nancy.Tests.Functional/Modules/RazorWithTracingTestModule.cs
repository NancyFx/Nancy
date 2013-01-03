namespace Nancy.Tests.Functional.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class RazorWithTracingTestModule : NancyModule
    {
        public RazorWithTracingTestModule()
        {
            StaticConfiguration.EnableRequestTracing = true;
            Get["/tracing/razor-viewbag"] = _ =>
                {
                    this.ViewBag.Name = "Bob";

                    return View["RazorPage"];
                };
        }
    }
}
