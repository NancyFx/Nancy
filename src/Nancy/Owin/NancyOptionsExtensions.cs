namespace Nancy.Owin
{
    using System.Linq;

    /// <summary>
    /// Extensions for the NancyOptions class.
    /// </summary>
    public static class NancyOptionsExtensions
    {
        /// <summary>
        /// Tells the NancyOwinHost to pass through when 
        /// response has one of the given status codes.
        /// </summary>
        /// <param name="nancyOptions">The Nancy options.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        public static void PassThroughWhenStatusCodesAre(this NancyOptions nancyOptions, params HttpStatusCode[] httpStatusCode)
        {
            nancyOptions.PerformPassThrough = context => httpStatusCode.Any(code => context.Response.StatusCode == code);
        }
    }
}