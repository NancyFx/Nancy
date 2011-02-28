namespace Nancy.Bootstrapper
{
    public interface INancyBootstrapperPerRequestRegistration<TContainer>
    {
        /// <summary>
        /// Configure the container with per-request registrations
        /// </summary>
        /// <param name="container"></param>
        void ConfigureRequestContainer(TContainer container);
    }
}