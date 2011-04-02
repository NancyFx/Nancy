namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Nancy bootstrapper base with per-request container support
    /// </summary>
    /// <typeparam name="TContainer">IoC container type</typeparam>
    public abstract class NancyBootstrapperWithRequestContainerBase<TContainer> : NancyBootstrapperBase<TContainer>
        where TContainer : class
    {
        /// <summary>
        /// Configure the request container
        /// </summary>
        /// <param name="container">Request container instance</param>
        protected abstract void ConfigureRequestContainer(TContainer container);
    }
}