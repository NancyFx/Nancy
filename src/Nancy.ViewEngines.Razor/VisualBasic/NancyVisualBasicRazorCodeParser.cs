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
        internal const string ModelTypeKeyword = "ModelType";

        private bool modelStatementFound;
        private SourceLocation? endInheritsLocation;
        private VisualBasicClrTypeResolver clrTypeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyVisualBasicRazorCodeParser"/> class.
        /// </summary>
        public NancyVisualBasicRazorCodeParser()
        {
            MapDirective(ModelTypeKeyword, ModelTypeDirective);

            this.clrTypeResolver = new VisualBasicClrTypeResolver();
        }

        protected virtual bool ModelTypeDirective()
        {
            this.AssertDirective(ModelTypeKeyword);

            this.Span.CodeGenerator = SpanCodeGenerator.Null;
            this.Context.CurrentBlock.Type = BlockType.Directive;
            this.AcceptAndMoveNext();

            var endModelLocation = CurrentLocation;

            if (At(VBSymbolType.WhiteSpace))
            {
                Span.EditHandler.AcceptedCharacters = AcceptedCharacters.None;
            }

            this.AcceptWhile(VBSymbolType.WhiteSpace);
            this.Output(SpanKind.MetaCode);

            if (this.modelStatementFound)
            {
                this.Context.OnError(endModelLocation, string.Format(CultureInfo.CurrentCulture, "Cannot have more than one @model statement."));
            }
            this.modelStatementFound = true;

            if (this.EndOfFile || At(VBSymbolType.WhiteSpace) || At(VBSymbolType.NewLine))
            {
                this.Context.OnError(endModelLocation, "The 'model' keyword must be followed by a type name on the same line.", ModelTypeKeyword);
            }

            AcceptUntil(VBSymbolType.NewLine);
            if (!Context.DesignTimeMode)
            {
                this.Optional(VBSymbolType.NewLine);
            }

            var baseType = string.Concat(Span.Symbols.Select(s => s.Content)).Trim();

            var modelType = this.clrTypeResolver.Resolve(this.Language.TokenizeString(baseType).ToList());           

            if (modelType == null)
            {
                CodeParserHelper.ThrowTypeNotFound(baseType);
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