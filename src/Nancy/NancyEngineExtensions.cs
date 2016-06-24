namespace Nancy
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extensions for Nancy engine
    /// </summary>
    public static class NancyEngineExtensions
    {
        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="nancyEngine">The <see cref="INancyEngine"/> instance.</param>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <returns>A <see cref="NancyContext"/> instance containing the request/response context.</returns>
        public static Task<NancyContext> HandleRequest(this INancyEngine nancyEngine, Request request)
        {
            return nancyEngine.HandleRequest(request, context => context, CancellationToken.None);
        }
    }
}