using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor.Parser;

namespace Nancy.ViewEngines.Razor
{
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