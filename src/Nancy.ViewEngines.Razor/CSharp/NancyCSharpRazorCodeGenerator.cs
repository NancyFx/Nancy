namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.CodeDom;
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser.SyntaxTree;

    /// <summary>
    /// A nancy version of the csharp code generator for razor.
    /// </summary>
    public class NancyCSharpRazorCodeGenerator : CSharpRazorCodeGenerator
    {
        private const string DEFAULT_MODEL_TYPE_NAME = "dynamic";

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyCSharpRazorCodeGenerator"/> class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="rootNamespaceName">Name of the root namespace.</param>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="host">The host.</param>
        public NancyCSharpRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host)
		{
            SetBaseType(DEFAULT_MODEL_TYPE_NAME);
        }

        protected override CodeWriter CreateCodeWriter()
        {
            return new CSharpCodeWriter();
        }

        protected override void WriteHelperVariable(string type, string name)
        {
            this.HelperVariablesMethod.Statements.Add((CodeStatement)new CodeSnippetStatement("#pragma warning disable 219"));
            this.CurrentBlock.MarkStartGeneratedCode();
            this.CurrentBlock.Writer.WriteSnippet(type);
            this.CurrentBlock.MarkEndGeneratedCode();
            this.CurrentBlock.Writer.WriteSnippet(" ");
            this.CurrentBlock.Writer.WriteSnippet(name);
            this.CurrentBlock.Writer.WriteSnippet(" = null");
            this.CurrentBlock.Writer.WriteEndStatement();
            this.HelperVariablesMethod.Statements.Add((CodeStatement)this.CreateStatement(this.CurrentBlock));
            this.CurrentBlock.ResetBuffer();
            this.HelperVariablesMethod.Statements.Add((CodeStatement)new CodeSnippetStatement("#pragma warning restore 219"));
        }

		protected override bool TryVisitSpecialSpan(Span span)
		{
			return RazorCodeGenerator.TryVisit<ModelSpan>(span, new Action<ModelSpan>(this.VisitModelSpan));
		}

		private void VisitModelSpan(ModelSpan span)
		{
			this.SetBaseType(span.ModelTypeName);
		}

        private void SetBaseType(string modelTypeName)
        {
            this.GeneratedClass.BaseTypes.Clear();
            this.GeneratedClass.BaseTypes.Add(new CodeTypeReference(this.Host.DefaultBaseClass + "<" + modelTypeName + ">"));
        }
    }
}