namespace Nancy.Demo.SparkViewEngine
{
    public class MainModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NancyModule"/> class.
        /// </summary>
        public MainModule()
        {
            Get["/"] = (x) =>
                {
                    return View["Index.spark"];
                };

           Get[ "/test" ] = _ => View[ "test" ];

           Get[ "/test2" ] = _ => View[ "test2" ];
        }
    }
}