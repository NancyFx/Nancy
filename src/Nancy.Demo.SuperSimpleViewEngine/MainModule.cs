namespace Nancy.Demo.SuperSimpleViewEngine
{
    using Nancy.Demo.SuperSimpleViewEngine.Models;

    public class MainModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NancyModule"/> class.
        /// </summary>
        public MainModule()
        {
            Get["/"] = (x) =>
                {
                    

                    var model = new MainModel(
                        "Jimbo", 
                        new[] { new User("Bob", "Smith"), new User("Jimbo", "Jones"), new User("Bill", "Bobs"), },
                        "<script type=\"text/javascript\">alert('Naughty JavaScript!');</script>");

                    return View["Index", model];
                };
        }
    }
}