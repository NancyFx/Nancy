namespace Nancy.ViewEngines.Razor
{
    public class NancyRazorErrorView : NancyRazorViewBase
    {
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
        /// Writes the literal.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLiteral(object value)
        {
            base.WriteLiteral(Message);
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            base.WriteLiteral(this.Message);
            base.WriteLiteral("<hr />");
            base.WriteLiteral(this.Code);
        }
    }
}
