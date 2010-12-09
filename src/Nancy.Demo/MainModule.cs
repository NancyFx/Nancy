using Nancy.Demo.Models;
using Nancy.ViewEngines.Razor;

namespace Nancy.Demo {
    public class Module : NancyModule {
        public Module() {
            Get["/"] = x => {
                return "This is the root";
            };

            Get["/test"] = x => {
                return "Test";
            };

            Get["/razor"] = x => {
                var model = new RatPack { FirstName = "Frank" };
                return View.Razor("~/views/razor.cshtml", model);
            };
        }
    }
}