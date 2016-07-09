namespace Nancy.ViewEngines.Razor
{
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;
    using Nancy.ViewEngines.Razor.CSharp;

    /// <summary>
    /// A custom razor engine host responsible for decorating the existing code generators with nancy versions.
    /// </summary>
    public class NancyRazorEngineHost : RazorEngineHost
    {
        private readonly RazorAssemblyProvider razorAssemblyProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyRazorEngineHost"/> class.
        /// </summary>
        public NancyRazorEngineHost(RazorCodeLanguage language, RazorAssemblyProvider razorAssemblyProvider)
            : base(language)
        {
            this.razorAssemblyProvider = razorAssemblyProvider;
            this.DefaultBaseClass = typeof (NancyRazorViewBase).FullName;
            this.DefaultNamespace = "RazorOutput";
            this.DefaultClassName = "RazorView";

            var context = new GeneratedClassContext("Execute", "Write", "WriteLiteral", "WriteTo", "WriteLiteralTo",
                typeof (HelperResult).FullName, "DefineSection");
            context.ResolveUrlMethodName = "ResolveUrl";

            this.GeneratedClassContext = context;
        }

        /// <summary>
        /// Decorates the code parser.
        /// </summary>
        /// <param name="incomingCodeParser">The incoming code parser.</param>
        /// <returns></returns>
        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            if (incomingCodeParser is CSharpCodeParser)
            {
                return new NancyCSharpRazorCodeParser(this.razorAssemblyProvider);
            }

            return base.DecorateCodeParser(incomingCodeParser);
        }
    }
}