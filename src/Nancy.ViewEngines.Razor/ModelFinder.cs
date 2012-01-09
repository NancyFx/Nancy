namespace Nancy.ViewEngines.Razor
{
    using System.Web.Razor.Parser;
    using System.Web.Razor.Parser.SyntaxTree;

    /// <summary>
    /// Razor parser visitor to retrieve the name of the model type.
    /// </summary>
    public class ModelFinder : ParserVisitor
    {
        /// <summary>
        /// Visits the current Razor parser span and looks for the model.
        /// </summary>
        /// <param name="span">The <see cref="Span"/> to visit.</param>
        public override void VisitSpan(Span span)
        {
            var modelSpan = span as ModelSpan;

            if(modelSpan == null)
            {
                return;
            }

            this.ModelTypeName = modelSpan.ModelTypeName;
        }

        /// <summary>
        /// Gets or sets the name of the model type.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the model.</value>
        public string ModelTypeName { get; set; }
    }
}