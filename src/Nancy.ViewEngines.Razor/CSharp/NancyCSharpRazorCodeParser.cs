namespace Nancy.ViewEngines.Razor.CSharp
{
    using System.Globalization;
    using System.Linq;
    using System.Web.Razor.Parser;
    using System.Web.Razor.Text;
    using System.Web.Razor.Tokenizer.Symbols;

    /// <summary>
    /// Nancy razor parser for csharp files.
    /// </summary>
    public class NancyCSharpRazorCodeParser : CSharpCodeParser
    {
        private readonly RazorAssemblyProvider razorAssemblyProvider;
        private bool modelStatementFound;
        private SourceLocation? endInheritsLocation;
        private readonly ClrTypeResolver<CSharpSymbolType, CSharpSymbol> clrTypeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyCSharpRazorCodeParser"/> class.
        /// </summary>
        /// <param name="razorAssemblyProvider">An <see cref="RazorAssemblyProvider"/> instance.</param>
        public NancyCSharpRazorCodeParser(RazorAssemblyProvider razorAssemblyProvider)
        {
            this.razorAssemblyProvider = razorAssemblyProvider;
            this.MapDirectives(this.ModelDirective, "model");

            this.clrTypeResolver = new CSharpClrTypeResolver(this.razorAssemblyProvider);
        }

        protected virtual void ModelDirective()
        {
            this.AssertDirective("model");

            this.AcceptAndMoveNext();

            var endModelLocation = this.CurrentLocation;

            this.BaseTypeDirective("The 'model' keyword must be followed by a type name on the same line.", s =>
            {
                var symbols = this.Language.TokenizeString(s);
                var modelType = this.clrTypeResolver.Resolve(symbols.ToList());

                if (modelType == null)
                {
                    CodeParserHelper.ThrowTypeNotFound(this.razorAssemblyProvider, s);
                }

                return new ModelCodeGenerator(modelType, modelType.FullName);
            });

            if (this.modelStatementFound)
            {
                this.Context.OnError(endModelLocation, string.Format(CultureInfo.CurrentCulture, "Cannot have more than one @model statement."));
            }

            this.modelStatementFound = true;
            this.CheckForInheritsAndModelStatements();
        }

        protected override void InheritsDirective()
        {
            this.AssertDirective("inherits");
            this.AcceptAndMoveNext();

            this.endInheritsLocation = this.CurrentLocation;

            base.InheritsDirective();

            this.CheckForInheritsAndModelStatements();
        }

        private void CheckForInheritsAndModelStatements()
        {
            if (this.modelStatementFound && this.endInheritsLocation.HasValue)
            {
                this.Context.OnError(this.endInheritsLocation.Value, string.Format(CultureInfo.CurrentCulture, "Cannot have both an @inherits statement and an @model statement."));
            }
        }
    }
}
