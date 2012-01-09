namespace Nancy.ViewEngines.Razor
{
    using System.Web.Razor.Parser;

    public class ModelFinder : ParserVisitor
    {

        public override void VisitSpan(System.Web.Razor.Parser.SyntaxTree.Span span)
        {
            var modelSpan = span as ModelSpan;

            if(modelSpan == null)
            {
                return;
            }

            this.ModelTypeName = modelSpan.ModelTypeName;
        }

        public string ModelTypeName { get; set; }
    }
}