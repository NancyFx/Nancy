namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Web.Razor.Parser.SyntaxTree;
    using System.Web.Razor.Text;
    using System.Web.Razor.Parser;

    public class ModelSpan : CodeSpan
    {
        public string ModelTypeName { get; private set; }

        public ModelSpan(SourceLocation start, string content, string modelTypeName)
            : base(start, content)
        {
            this.ModelTypeName = modelTypeName;
        }

        internal ModelSpan(ParserContext context, string modelTypeName)
            : this(context.CurrentSpanStart, context.ContentBuffer.ToString(), modelTypeName)
        { }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (this.ModelTypeName ?? string.Empty).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ModelSpan modelSpan = obj as ModelSpan;
            return modelSpan != null && this.Equals(modelSpan);
        }

        private bool Equals(ModelSpan span)
        {
            return base.Equals(span) && string.Equals(this.ModelTypeName, span.ModelTypeName, StringComparison.Ordinal);
        }
    }

}
