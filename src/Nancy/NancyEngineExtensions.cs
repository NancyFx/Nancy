namespace Nancy
{
    using System.Threading.Tasks;

    public static class NancyEngineExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nancyEngine"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Task<NancyContext> HandleRequestAsync(this INancyEngine nancyEngine, Request request)
        {
            return nancyEngine.HandleRequestAsync(request, context => context);
        }
    }
}