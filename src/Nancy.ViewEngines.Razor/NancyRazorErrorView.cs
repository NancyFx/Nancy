namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;

    /// <summary>
    /// Razor view used when compilation of the view fails.
    /// </summary>
    public class NancyRazorErrorView : NancyRazorViewBase
    {
        private readonly TraceConfiguration traceConfiguration;

        private const string DisplayErrorTracesFalseMessage = "Error details are currently disabled.<br />To enable it, please set <strong>TraceConfiguration.DisplayErrorTraces</strong> to <strong>true</strong>.<br />For example by overriding your Bootstrapper's <strong>Configure</strong> method and calling<br/> <strong>environment.Tracing(enabled: false, displayErrorTraces: true);</strong>.";

        private static string template;

        /// <summary>
        /// Gets or sets the template for rendering errors.
        /// The token "[DETAILS]" will be replaced by the HTML for
        /// the actual error.
        /// </summary>
        public static string Template
        {
            get { return template ?? (template = LoadResource(@"CompilationError.html")); }
            set { template = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyRazorErrorView"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="traceConfiguration">A <see cref="TraceConfiguration"/> instance.</param>
        public NancyRazorErrorView(string message, TraceConfiguration traceConfiguration)
        {
            this.traceConfiguration = traceConfiguration;
            this.Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            base.WriteLiteral(Template.Replace("[DETAILS]", this.traceConfiguration.DisplayErrorTraces ? this.Message : DisplayErrorTracesFalseMessage));
        }

        private static string LoadResource(string filename)
        {
            var resourceStream = typeof(NancyRazorErrorView).Assembly.GetManifestResourceStream(String.Format("Nancy.ViewEngines.Razor.Resources.{0}", filename));

            if (resourceStream == null)
            {
                return string.Empty;
            }

            using (var reader = new StreamReader(resourceStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
