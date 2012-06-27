namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;

    public class NancyRazorErrorView : NancyRazorViewBase
    {
        private static string template = LoadResource(@"CompilationError.html");

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyRazorErrorView"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NancyRazorErrorView(string message) {
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
            base.WriteLiteral(template.Replace("[DETAILS]", StaticConfiguration.DisableErrorTraces ? String.Empty : this.Message));
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
