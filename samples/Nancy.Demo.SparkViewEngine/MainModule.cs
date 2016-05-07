namespace Nancy.Demo.SparkViewEngine
{
    public class MainModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INancyModule"/> class.
        /// </summary>
        public MainModule()
        {
            Get("/", args =>
            {
                return View["Index.spark"];
            });

           Get("/test", args => View["test"]);

           Get("/test2", args => View["test2"]);
        }
    }
}