namespace Nancy.Demo.SparkViewEngine.FifthElement
{
    public class FifthElementModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INancyModule"/> class.
        /// </summary>
        public FifthElementModule()
        {
            Get("/5", args =>
            {
                return View["Fifth.spark"];
            });
        }
    }
}