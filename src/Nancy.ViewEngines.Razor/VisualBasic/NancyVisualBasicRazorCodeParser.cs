namespace Nancy.ViewEngines.Razor.VisualBasic
{
    using System.Globalization;
    using System.Web.Razor.Parser;
    using System.Web.Razor.Parser.SyntaxTree;
    using System.Web.Razor.Text;

    /// <summary>
    /// Nancy razor parser for visual basic files.
    /// </summary>
    public class NancyVisualBasicRazorCodeParser : VBCodeParser
    {
        private const string ModelTypeKeyword = "ModelType";

        private SourceLocation? endInheritsLocation;
        private bool modelStatementFound;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyVisualBasicRazorCodeParser"/> class.
        /// </summary>
        public NancyVisualBasicRazorCodeParser()
        {
            this.KeywordHandlers.Add("ModelType", new CodeParser.BlockParser(this.ParseModelStatement));
        }

        protected override bool ParseInheritsStatement(CodeBlockInfo block)
        {
            this.endInheritsLocation = CurrentLocation;
            var result = this.ParseInheritsStatement(block);
            this.CheckForInheritsAndModelStatements();
            return result;
        }

        private void CheckForInheritsAndModelStatements()
        {
            if (this.modelStatementFound && this.endInheritsLocation.HasValue)
                this.OnError(this.endInheritsLocation.Value, string.Format(CultureInfo.CurrentCulture, "Cannot have both an @Inherits statement and an @ModelType statement."));
        }

        private bool ParseModelStatement(CodeBlockInfo block)
        {
            using (this.StartBlock(BlockType.Directive))
            {
                block.ResumeSpans(this.Context);
                var currentLocation = CurrentLocation;
                var acceptedCharacters = this.RequireSingleWhiteSpace() ? AcceptedCharacters.None : AcceptedCharacters.Any;
                
                this.End(MetaCodeSpan.Create(this.Context, false, acceptedCharacters));

                if (this.modelStatementFound)
                    this.OnError(currentLocation, string.Format(CultureInfo.CurrentCulture, "Only one @ModelType statement is allowed."));
                
                this.modelStatementFound = true;
                this.Context.AcceptWhiteSpace(false);
                string modelTypeName = null;
                if (ParserHelpers.IsIdentifierStart(this.CurrentCharacter))
                {
                    using (this.Context.StartTemporaryBuffer())
                    {
                        this.Context.AcceptUntil(c => ParserHelpers.IsNewLine(c));
                        modelTypeName = this.Context.ContentBuffer.ToString();
                        this.Context.AcceptTemporaryBuffer();
                    }
                    this.Context.AcceptNewLine();
                }
                else
                {
                    this.OnError(currentLocation, string.Format(CultureInfo.CurrentCulture, "@ModelType must be followed by a type name."));
                }
                this.CheckForInheritsAndModelStatements();
                this.End(new ModelSpan(this.Context, modelTypeName));
            }
            return false;
        }
    }
}