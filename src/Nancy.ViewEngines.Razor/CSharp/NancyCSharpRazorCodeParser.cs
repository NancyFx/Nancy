namespace Nancy.ViewEngines.Razor.CSharp
{
    using System.Globalization;
    using System.Web.Razor.Parser;
    using System.Web.Razor.Parser.SyntaxTree;
    using System.Web.Razor.Text;

    /// <summary>
    /// Nancy razor parser for csharp files.
    /// </summary>
    public class NancyCSharpRazorCodeParser : CSharpCodeParser
    {
        private bool modelStatementFound;
        private SourceLocation? endInheritsLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyCSharpRazorCodeParser"/> class.
        /// </summary>
        public NancyCSharpRazorCodeParser()
        {
            this.RazorKeywords.Add("model", WrapSimpleBlockParser(BlockType.Directive, new CodeParser.BlockParser(this.ParseModelStatement)));
        }

        protected override bool ParseInheritsStatement(CodeBlockInfo block)
        {
            this.endInheritsLocation = this.CurrentLocation;
            var result = this.ParseInheritsStatement(block);
            this.CheckForInheritsAndModelStatements();
            return result;
        }

        private void CheckForInheritsAndModelStatements()
        {
            if (this.modelStatementFound && this.endInheritsLocation.HasValue)
                this.OnError(this.endInheritsLocation.Value, string.Format(CultureInfo.CurrentCulture, "Cannot have both an @inherits statement and an @model statement."));
        }

        private bool ParseModelStatement(CodeBlockInfo block)
        {
            var currentLocation = this.CurrentLocation;
            var acceptedCharacters = this.RequireSingleWhiteSpace() ? AcceptedCharacters.None : AcceptedCharacters.Any;

            this.End(MetaCodeSpan.Create(this.Context, false, acceptedCharacters));

            if (this.modelStatementFound)
                this.OnError(currentLocation, string.Format(CultureInfo.CurrentCulture, "Only one @model statement is allowed."));
            
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
                this.OnError(currentLocation, string.Format(CultureInfo.CurrentCulture, "@model must be followed by a type name."));
            }
            
            this.CheckForInheritsAndModelStatements();
            this.End(new ModelSpan(this.Context, modelTypeName));
            return false;
        }
    }
}