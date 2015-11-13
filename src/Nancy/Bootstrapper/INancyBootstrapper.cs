namespace Nancy.Bootstrapper
{
    using System;
    using Nancy.Configuration;

    /// <summary>
    /// Bootstrapper for the Nancy Engine
    /// </summary>
    public interface INancyBootstrapper : IDisposable
    {
        /// <summary>
        /// Initialise the bootstrapper.
        /// </summary>
        /// <remarks>Must be called prior to <see cref="GetEngine"/> and <see cref="GetEnvironment"/>.</remarks>
        void Initialise();

        /// <summary>
        /// Gets the configured <see cref="INancyEngine"/>.
        /// </summary>
        /// <returns>An configured <see cref="INancyEngine"/> instance.</returns>
        /// <remarks>The boostrapper must be initialised (<see cref="Initialise"/>) prior to calling this.</remarks>
        INancyEngine GetEngine();

        /// <summary>
        /// Get the <see cref="INancyEnvironment"/> instance.
        /// </summary>
        /// <returns>An configured <see cref="INancyEnvironment"/> instance.</returns>
        /// <remarks>The boostrapper must be initialised (<see cref="Initialise"/>) prior to calling this.</remarks>
        INancyEnvironment GetEnvironment();
    }
}