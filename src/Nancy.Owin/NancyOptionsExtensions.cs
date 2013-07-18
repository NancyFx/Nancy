namespace Nancy.Owin
{
    using System.Linq;

    public static class NancyOptionsExtensions
    {
        public static void PassThroughWhenStatusCodesAre(this NancyOptions nancyOptions, params HttpStatusCode[] httpStatusCode)
        {
            nancyOptions.PerformPassThrough = context => httpStatusCode.Any(code => context.Response.StatusCode == code);
        }
    }
}