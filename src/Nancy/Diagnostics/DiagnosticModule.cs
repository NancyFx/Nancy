namespace Nancy.Diagnostics
{
    using Configuration;

    /// <summary>
    /// Abstract base class for Nancy diagnostics module.
    /// </summary>
    /// <seealso cref="Nancy.NancyModule" />
    public abstract class DiagnosticModule : NancyModule
    {
        private readonly INancyEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticModule"/> class.
        /// </summary>
        protected DiagnosticModule()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticModule"/> class.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        protected DiagnosticModule(string basePath)
            : base(basePath)
        {
            this.environment = new DefaultNancyEnvironment();
            this.environment.AddValue(ViewConfiguration.Default);
        }

        /// <summary>
        /// Renders a view from inside a route handler.
        /// </summary>
        /// <value>
        /// A <see cref="ViewRenderer" /> instance that is used to determine which view that should be rendered.
        /// </value>
        public new DiagnosticsViewRenderer View
        {
            get { return new DiagnosticsViewRenderer(this.Context, this.environment); }
        }
    }
}
