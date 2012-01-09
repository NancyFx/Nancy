namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Web.Razor.Parser;
    using System.Web.Razor.Parser.SyntaxTree;
    using System.Web.Razor.Text;

    /// <summary>
    /// A special codespan for the model keyword.
    /// </summary>
    public class ModelSpan : CodeSpan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSpan"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="content">The content.</param>
        /// <param name="modelTypeName">Name of the model type.</param>
        public ModelSpan(SourceLocation start, string content, string modelTypeName)
            : base(start, content)
        {
            this.ModelTypeName = modelTypeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSpan"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="modelTypeName">Name of the model type.</param>
        internal ModelSpan(ParserContext context, string modelTypeName)
            : this(context.CurrentSpanStart, context.ContentBuffer.ToString(), modelTypeName)
        {
        }

        /// <summary>
        /// Gets the name of the model type.
        /// </summary>
        /// <value>
        /// The name of the model type.
        /// </value>
        public string ModelTypeName { get; private set; }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (this.ModelTypeName ?? string.Empty).GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var modelSpan = obj as ModelSpan;

            return modelSpan != null && this.Equals(modelSpan);
        }

        /// <summary>
        /// Determines whether the specified span is equal to this instance.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns></returns>
        private bool Equals(ModelSpan span)
        {
            return base.Equals(span) && string.Equals(this.ModelTypeName, span.ModelTypeName, StringComparison.OrdinalIgnoreCase);
        }
    }

}
