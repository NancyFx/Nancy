namespace Nancy.Prototype
{
    public static class BootstrapperExtensions
    {
        public static IBootstrapper<TContainer> Populate<TBuilder, TContainer>(this IBootstrapper<TBuilder, TContainer> bootstrapper, TBuilder builder)
        {
            return bootstrapper.Populate(builder, DefaultPlatform.Instance);
        }
    }
}
