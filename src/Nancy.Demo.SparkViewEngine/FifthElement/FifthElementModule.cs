namespace Nancy.Demo.SparkViewEngine.FifthElement
{
    public class FifthElementModule : LegacyNancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INancyModule"/> class.
        /// </summary>
        public FifthElementModule()
        {
            Get["/5"] = (x) =>
                {
                    return View["Fifth.spark"];
                };
        }
    }
}