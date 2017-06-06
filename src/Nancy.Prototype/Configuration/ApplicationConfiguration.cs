namespace Nancy.Prototype.Configuration
{
    internal class ApplicationConfiguration<TContainer> : IApplicationConfiguration<TContainer>
    {
        public ApplicationConfiguration(TContainer container, IFrameworkConfiguration framework)
        {
            Check.NotNull(container, nameof(container));
            Check.NotNull(framework, nameof(framework));

            this.Container = container;
            this.Framework = framework;
        }

        public TContainer Container { get; }

        public IFrameworkConfiguration Framework { get; }
    }
}
