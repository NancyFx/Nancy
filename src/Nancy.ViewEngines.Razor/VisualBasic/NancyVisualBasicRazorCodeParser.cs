namespace Nancy.ViewEngines.Razor.VisualBasic
{
    using System.Globalization;
    using System.Linq;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;
    using System.Web.Razor.Parser.SyntaxTree;
    using System.Web.Razor.Text;
    using System.Web.Razor.Tokenizer.Symbols;

    /// <summary>
    /// Nancy razor parser for visual basic files.
    /// </summary>
    public class NancyVisualBasicRazorCodeParser : VBCodeParser
    {
        private readonly IAssemblyCatalog assemblyCatalog;
        internal const string ModelTypeKeyword = "ModelType";
        private bool modelStatementFound;
        private SourceLocation? endInheritsLocation;
        private readonly VisualBasicClrTypeResolver clrTypeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyVisualBasicRazorCodeParser"/> class.
        /// </summary>
        /// <param name="assemblyCatalog">An <see cref="IAssemblyCatalog"/> instance.</param>
        public NancyVisualBasicRazorCodeParser(IAssemblyCatalog assemblyCatalog)
        {
            this.assemblyCatalog = assemblyCatalog;
            this.MapDirective(ModelTypeKeyword, this.ModelTypeDirective);

            this.clrTypeResolver = new VisualBasicClrTypeResolver(assemblyCatalog);
        }

        protected virtual bool ModelTypeDirective()
        {
            this.AssertDirective(ModelTypeKeyword);

            this.Span.CodeGenerator = SpanCodeGenerator.Null;
            this.Context.CurrentBlock.Type = BlockType.Directive;
            this.AcceptAndMoveNext();

            var endModelLocation = this.CurrentLocation;

            if (this.At(VBSymbolType.WhiteSpace))
            {
                this.Span.EditHandler.AcceptedCharacters = AcceptedCharacters.None;
            }

            this.AcceptWhile(VBSymbolType.WhiteSpace);
            this.Output(SpanKind.MetaCode);

            if (this.modelStatementFound)
            {
                this.Context.OnError(endModelLocation, string.Format(CultureInfo.CurrentCulture, "Cannot have more than one @model statement."));
            }
            this.modelStatementFound = true;

            if (this.EndOfFile || this.At(VBSymbolType.WhiteSpace) || this.At(VBSymbolType.NewLine))
            {
                this.Context.OnError(endModelLocation, "The 'model' keyword must be followed by a type name on the same line.", ModelTypeKeyword);
            }

            this.AcceptUntil(VBSymbolType.NewLine);
            if (!this.Context.DesignTimeMode)
            {
                this.Optional(VBSymbolType.NewLine);
            }

            var baseType = string.Concat(this.Span.Symbols.Select(s => s.Content)).Trim();

            var modelType = this.clrTypeResolver.Resolve(this.Language.TokenizeString(baseType).ToList());

            if (modelType == null)
            {
                CodeParserHelper.ThrowTypeNotFound(this.assemblyCatalog, baseType);
            }

            this.Span.CodeGenerator = new ModelCodeGenerator(modelType, modelType.FullName);

            this.CheckForInheritsAndModelStatements();

            this.Output(SpanKind.Code);

            return false;
        }

        protected override bool InheritsStatement()
        {
            this.AssertDirective("Inherits");

            this.AcceptAndMoveNext();

            this.endInheritsLocation = this.CurrentLocation;

            var result = base.InheritsStatement();

            this.CheckForInheritsAndModelStatements();

            return result;
        }

        private void CheckForInheritsAndModelStatements()
        {
            if (this.modelStatementFound && this.endInheritsLocation.HasValue)
            {
                this.Context.OnError(this.endInheritsLocation.Value, string.Format(CultureInfo.CurrentCulture, "Cannot have both an @Inherits statement and an @ModelType statement."));
            }
        }
    }
}
