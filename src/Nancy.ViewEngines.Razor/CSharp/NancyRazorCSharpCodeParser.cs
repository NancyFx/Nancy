namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.Globalization;
    using System.Web.Razor.Parser;
    using System.Web.Razor.Parser.SyntaxTree;
    using System.Web.Razor.Text;

    public class NancyRazorCSharpCodeParser : CSharpCodeParser
    {
        private bool modelStatementFound;
        private SourceLocation? endInheritsLocation;

        public NancyRazorCSharpCodeParser()
        {
            base.RazorKeywords.Add("model", base.WrapSimpleBlockParser(BlockType.Directive, new CodeParser.BlockParser(this.ParseModelStatement)));
        }

        protected override bool ParseInheritsStatement(CodeBlockInfo block)
        {
            endInheritsLocation = CurrentLocation;
            var result = ParseInheritsStatement(block);
            CheckForInheritsAndModelStatements();
            return result;
        }
        private void CheckForInheritsAndModelStatements()
        {
            if (modelStatementFound && endInheritsLocation.HasValue)
                base.OnError(endInheritsLocation.Value, string.Format(CultureInfo.CurrentCulture, "Cannot have both an @inherits statement and an @model statement."));
        }

        private bool ParseModelStatement(CodeBlockInfo block)
        {
            var currentLocation = CurrentLocation;
            var acceptedCharacters = RequireSingleWhiteSpace() ? AcceptedCharacters.None : AcceptedCharacters.Any;

            End(MetaCodeSpan.Create(base.Context, false, acceptedCharacters));

            if (modelStatementFound)
                OnError(currentLocation, string.Format(CultureInfo.CurrentCulture, "Only one @model statement is allowed."));
            
            modelStatementFound = true;
            Context.AcceptWhiteSpace(false);
            string modelTypeName = null;
            if (ParserHelpers.IsIdentifierStart(CurrentCharacter))
            {
                using (Context.StartTemporaryBuffer())
                {
                    Context.AcceptUntil(c => ParserHelpers.IsNewLine(c));
                    modelTypeName = Context.ContentBuffer.ToString();
                    Context.AcceptTemporaryBuffer();
                }
                Context.AcceptNewLine();
            }
            else
            {
                OnError(currentLocation, string.Format(CultureInfo.CurrentCulture, "@model must be followed by a type name."));
            }
            
            CheckForInheritsAndModelStatements();
            End(new ModelSpan(base.Context, modelTypeName));
            return false;
        }
    }
}