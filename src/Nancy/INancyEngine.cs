namespace Nancy
{
    public interface INancyEngine
    {
        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <returns>A <see cref="NancyContext"/> instance containing the request/response context.</returns>
        NancyContext HandleRequest(Request request);
    }
}