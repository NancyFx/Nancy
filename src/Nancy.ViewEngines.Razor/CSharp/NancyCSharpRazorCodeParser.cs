namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web.Razor.Parser;
    using System.Web.Razor.Text;
    using System.Web.Razor.Tokenizer.Symbols;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Nancy razor parser for csharp files.
    /// </summary>
    public class NancyCSharpRazorCodeParser : CSharpCodeParser
    {
        private bool modelStatementFound;
        private SourceLocation? endInheritsLocation;
        private readonly ClrTypeResolver<CSharpSymbolType, CSharpSymbol> clrTypeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyCSharpRazorCodeParser"/> class.
        /// </summary>
        public NancyCSharpRazorCodeParser()
        {
            this.MapDirectives(this.ModelDirective, "model");

            this.clrTypeResolver = new CSharpClrTypeResolver();
        }

        protected virtual void ModelDirective()
        {
            this.AssertDirective("model");

            this.AcceptAndMoveNext();

            var endModelLocation = CurrentLocation;

            this.BaseTypeDirective("The 'model' keyword must be followed by a type name on the same line.", s =>
            {
                var symbols = this.Language.TokenizeString(s);
                var modelType = this.clrTypeResolver.Resolve(symbols.ToList());

                if (modelType == null)
                {
                    CodeParserHelper.ThrowTypeNotFound(s);
                }

                return new ModelCodeGenerator(modelType, modelType.FullName);
            });

            if (this.modelStatementFound)
            {
                this.Context.OnError(endModelLocation, string.Format(CultureInfo.CurrentCulture, "Cannot have more than one @model statement."));
            }

            modelStatementFound = true;

            CheckForInheritsAndModelStatements();
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