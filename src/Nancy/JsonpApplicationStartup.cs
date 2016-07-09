namespace Nancy
{
    using Nancy.Bootstrapper;
    using Nancy.Configuration;

    /// <summary>
    /// Enables JSONP support at application startup.
    /// </summary>
    public class JsonpApplicationStartup : IApplicationStartup
    {
        private readonly INancyEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonpApplicationStartup"/> class,
        /// with the provided <see cref="INancyEnvironment"/> instance.
        /// </summary>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public JsonpApplicationStartup(INancyEnvironment environment)
        {
            this.environment = environment;
        }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
            Jsonp.Enable(pipelines, this.environment);
        }
    }
}
