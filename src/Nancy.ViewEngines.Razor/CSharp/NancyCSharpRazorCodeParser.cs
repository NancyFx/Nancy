namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.Globalization;
    using System.Web.Razor.Generator;
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
            this.MapDirectives(this.ModelDirective, "model");
        }

        protected virtual void ModelDirective()
        {
            this.AssertDirective("model");

            this.AcceptAndMoveNext();

            var endModelLocation = CurrentLocation;

            this.BaseTypeDirective("The 'model' keyword must be followed by a type name on the same line.", s => new ModelCodeGenerator(s));

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

        //private bool ParseModelStatement(CodeBlockInfo block)
        //{
        //    var currentLocation = this.CurrentLocation;
        //    var acceptedCharacters = this.RequireSingleWhiteSpace() ? AcceptedCharacters.None : AcceptedCharacters.Any;

        //    this.End(MetaCodeSpan.Create(this.Context, false, acceptedCharacters));

        //    if (this.modelStatementFound)
        //    {
        //        this.OnError(currentLocation, string.Format(CultureInfo.CurrentCulture, "Only one @model statement is allowed."));
        //    }
            
        //    this.modelStatementFound = true;
        //    this.Context.AcceptWhiteSpace(false);
        //    string modelTypeName = null;

        //    if (ParserHelpers.IsIdentifierStart(this.CurrentCharacter))
        //    {
        //        using (this.Context.StartTemporaryBuffer())
        //        {
        //            this.Context.AcceptUntil(ParserHelpers.IsNewLine);
        //            modelTypeName = this.Context.ContentBuffer.ToString();
        //            this.Context.AcceptTemporaryBuffer();
        //        }

        //        this.Context.AcceptNewLine();
        //    }
        //    else
        //    {
        //        this.OnError(currentLocation, string.Format(CultureInfo.CurrentCulture, "@model must be followed by a type name."));
        //    }
            
        //    this.CheckForInheritsAndModelStatements();
        //    this.End(new ModelSpan(this.Context, modelTypeName));

        //    return false;
        //}
    }

    public class ModelCodeGenerator : SetBaseTypeCodeGenerator
    {
        public ModelCodeGenerator(string baseType)
            : base(baseType)
        {
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}<{1}>",
                context.Host.DefaultBaseClass,
                baseType);
        }
    }
}