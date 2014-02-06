namespace Owin.AutoStartup.Nancy
{
    using System.Collections.Generic;

    using global::Owin;
    using global::Owin.AutoStartup;

    /// <summary>
    /// Provides OWIN.AutoStartup support for Nancy
    /// </summary>
    public class NancyAutoStartup : IAutoStartup
    {
        private readonly string[] defaultBuilderCommands;

        /// <summary>
        /// Gets the name of the provider
        /// e.g. Nancy, SignalR
        /// </summary>
        public string Name
        {
            get
            {
                return "Nancy";
            }
        }

        /// <summary>
        /// Gets the path that the provider will bind to
        /// </summary>
        public string Path
        {
            get
            {
                return "/";
            }
        }

        /// <summary>
        /// Gets the default builder commands that are called in configure.
        /// Used for generating help text.
        /// </summary>
        public IEnumerable<string> DefaultBuilderCommands
        {
            get
            {
                return this.defaultBuilderCommands;
            }
        }

        /// <summary>
        /// Configure the auto startup
        /// </summary>
        /// <param name="builder">App builder interface</param>
        public void Configuration(IAppBuilder builder)
        {
            builder.UseNancy();
        }

        public NancyAutoStartup()
        {
            this.defaultBuilderCommands = new[] { "builder.UseNancy();" };
        }
    }
}