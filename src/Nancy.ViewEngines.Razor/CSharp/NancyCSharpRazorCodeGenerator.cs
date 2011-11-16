namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.CodeDom;
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser.SyntaxTree;

    public class NancyCSharpRazorCodeGenerator : CSharpRazorCodeGenerator
    {
        private const string DEFAULT_MODEL_TYPE_NAME = "dynamic";

        public NancyCSharpRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host)
		{
            SetBaseType(DEFAULT_MODEL_TYPE_NAME);
        }

		protected override bool TryVisitSpecialSpan(Span span)
		{
			return RazorCodeGenerator.TryVisit<ModelSpan>(span, new Action<ModelSpan>(VisitModelSpan));
		}

		private void VisitModelSpan(ModelSpan span)
		{
			SetBaseType(span.ModelTypeName);
		}

        private void SetBaseType(string modelTypeName)
        {
            GeneratedClass.BaseTypes.Clear();
            GeneratedClass.BaseTypes.Add(new CodeTypeReference(Host.DefaultBaseClass + "<" + modelTypeName + ">"));
        }
    }
}
