namespace Nancy.ViewEngines.Razor
{
    using System.Web.Razor.Parser;

    public class ModelFinder : ParserVisitor
    {
        public string ModelTypeName { get; set; }

        public override void VisitSpan(System.Web.Razor.Parser.SyntaxTree.Span span)
        {
            var modelSpan = span as ModelSpan;
            if(modelSpan == null)
                return;

            ModelTypeName = modelSpan.ModelTypeName;
        }
    }
}