namespace Nancy.Prototype
{
    using System;
    using Nancy.Prototype.Configuration;
    using Nancy.Prototype.Registration;

    /// <summary>
    ///     The main base class for all bootstrappers.
    ///     This is responsible for creating and configuring the
    ///     application container and the framework as a whole.
    /// </summary>
    public abstract class Bootstrapper<TBuilder, TContainer> : IBootstrapper<TBuilder, TContainer>
        where TContainer : IDisposable
    {
        public IApplication InitializeApplication(IPlatform platform)
        {
            Check.NotNull(platform, nameof(platform));

            // First, we need a container builder.
            // This step is a noop in bootstrappers without the builder/container split.
            var builder = this.CreateBuilder();

            this.Populate(builder, platform);

            // Once everything is registered, it's time to build the container.
            // This step is a noop in bootstrappers without the builder/container split.
            var container = this.BuildContainer(builder);

            // Since we've built the container, we want to dispose it as well.
            var disposable = container.AsConditionalDisposable(shouldDispose: true);

            return this.InitializeApplication(disposable);
        }

        public IBootstrapper<TContainer> Populate(TBuilder builder, IPlatform platform)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(platform, nameof(platform));

            var typeCatalog = platform.TypeCatalog;

            var frameworkConfig = new FrameworkConfiguration(typeCatalog);

            // We'll hang all configuration related stuff off this object.
            // Everything will be pre-configured with Nancy defaults.
            var applicationConfig = new ApplicationConfiguration<TBuilder>(builder, frameworkConfig);

            // This is the main configuration point for the user.
            // Here you can register stuff in the container, swap out
            // Nancy services, change configuration etc.
            this.ConfigureApplication(applicationConfig);

            // Get platform services to register in the container.
            var platformRegistry = platform.GetRegistry();

            this.Register(builder, platformRegistry);

            // Once the user has configured everything, we build a
            // "container registry", this contains all registrations
            // for framework services.
            var frameworkRegistry = frameworkConfig.GetRegistry();

            // We then call out to the bootstrapper implementation
            // to register all the registrations in the registry.
            this.Register(builder, frameworkRegistry);

            return this;
        }

        public IApplication<TContainer> InitializeApplication(TContainer container)
        {
            Check.NotNull(container, nameof(container));

            // In this case, we don't want to control the container
            // lifetime, because it's passed from outside. We don't own it.
            var disposable = container.AsConditionalDisposable(shouldDispose: false);

            return this.InitializeApplication(disposable);
        }

        private IApplication<TContainer> InitializeApplication(ConditionalDisposable<TContainer> container)
        {
            // When the container is built, we offer the bootstrapper
            // implementation a chance to validate the container configuration
            // This could prevent obvious configuration errors.
            this.ValidateContainerConfiguration(container);

            // We finally ask the bootstrapper implementation to give us
            // an IApplication instance before returning it to the caller.
            return this.CreateApplication(container);
        }

        protected abstract TBuilder CreateBuilder();

        protected virtual void ConfigureApplication(IApplicationConfiguration<TBuilder> app)
        {
        }

        protected abstract void Register(TBuilder builder, IContainerRegistry registry);

        protected abstract TContainer BuildContainer(TBuilder builder);

        protected abstract void ValidateContainerConfiguration(TContainer container);

        protected abstract IApplication<TContainer> CreateApplication(ConditionalDisposable<TContainer> container);
    }

    /// <summary>
    ///     A convenience bootstrapper base for containers without a builder/container
    ///     split, i.e. they allow appending to an existing container instance.
    /// </summary>
    public abstract class Bootstrapper<TContainer> : Bootstrapper<TContainer, TContainer>
        where TContainer : IDisposable
    {
        protected sealed override TContainer CreateBuilder() => this.CreateContainer();

        protected sealed override TContainer BuildContainer(TContainer container) => container;

        protected abstract TContainer CreateContainer();
    }
}
