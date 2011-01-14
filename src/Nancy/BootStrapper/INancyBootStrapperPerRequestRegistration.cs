using System;

namespace Nancy.BootStrapper
{
    public interface INancyBootStrapperPerRequestRegistration<TContainer>
    {
        /// <summary>
        /// Configure the container with per-request registrations
        /// </summary>
        /// <param name="container"></param>
        void ConfigureRequestContainer(TContainer container);
    }
}
