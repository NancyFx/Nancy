namespace Nancy.Demo.Hosting.Aspnet
{
    using Nancy.Configuration;

    /// <summary>
    /// Illustrates how you can create custom environment configurations by hanging
    /// extension methods on INancyEnvironment and sticking custom configuration
    /// objects into environment.
    /// </summary>
    public static class MyConfigExtensions
    {
        public static void MyConfig(this INancyEnvironment environment, string value)
        {
            environment.AddValue(
                typeof(MyConfig).FullName, // Using the full type name of the type to avoid collisions
                new MyConfig(value));
        }
    }
}