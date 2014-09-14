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
                    //TODO: GetAssembliesInDirectories().Aggregate(
                    throw new NotSupportedException(string.Format(
                                                "Unable to discover CLR Type for model by the name of {0}.\n\nTry using a fully qualified type name and ensure that the assembly is added to the configuration file.\n\nAppDomain Assemblies:\n\t{1}.\n\nCurrent ADATS assemblies:\n\t{2}.\n\nAssemblies in directories\n\t{3}",
                                                s,
                                                AppDomain.CurrentDomain.GetAssemblies().Select(a => a.FullName).Aggregate((n1, n2) => n1 + "\n\t" + n2),
                                                AppDomainAssemblyTypeScanner.Assemblies.Select(a => a.FullName).Aggregate((n1, n2) => n1 + "\n\t" + n2)
                                                //GetAssembliesInDirectories().Aggregate((n1, n2) => n1 + "\n\t" + n2))
                                                ));
                }

                return new CSharpModelCodeGenerator(modelType, modelType.FullName);
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