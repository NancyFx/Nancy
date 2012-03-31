namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;

    public class HelperResult : IHtmlString
    {
        private readonly Action<TextWriter> _action;

        public HelperResult(Action<TextWriter> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            this._action = action;
        }

        public string ToHtmlString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            using (StringWriter stringWriter = new StringWriter((IFormatProvider)CultureInfo.InvariantCulture))
            {
                this._action((TextWriter)stringWriter);
                return stringWriter.ToString();
            }
        }

        public void WriteTo(TextWriter writer)
        {
            this._action(writer);
        }
    }

    /// <summary>
    /// A custom razor engine host responsible for decorating the existing code generators with nancy versions.
    /// </summary>
    public class NancyRazorEngineHost : RazorEngineHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NancyRazorEngineHost"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        public NancyRazorEngineHost(RazorCodeLanguage language)
            : base(language)
		{
            this.DefaultBaseClass = typeof(NancyRazorViewBase).FullName;
            this.DefaultNamespace = "RazorOutput";
            this.DefaultClassName = "RazorView";

            this.GeneratedClassContext = new GeneratedClassContext("Execute", "Write", "WriteLiteral", null, null, typeof(HelperResult).FullName, "DefineSection");
		}

        /// <summary>
        /// Decorates the code generator.
        /// </summary>
        /// <param name="incomingCodeGenerator">The incoming code generator.</param>
        /// <returns></returns>
		public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
		{
			if (incomingCodeGenerator is CSharpRazorCodeGenerator)
				return new CSharp.NancyCSharpRazorCodeGenerator(incomingCodeGenerator.ClassName, incomingCodeGenerator.RootNamespaceName, incomingCodeGenerator.SourceFileName, incomingCodeGenerator.Host);

            if (incomingCodeGenerator is VBRazorCodeGenerator)
                return new VisualBasic.NancyVisualBasicRazorCodeGenerator(incomingCodeGenerator.ClassName, incomingCodeGenerator.RootNamespaceName, incomingCodeGenerator.SourceFileName, incomingCodeGenerator.Host);

            return base.DecorateCodeGenerator(incomingCodeGenerator);
		}

        /// <summary>
        /// Decorates the code parser.
        /// </summary>
        /// <param name="incomingCodeParser">The incoming code parser.</param>
        /// <returns></returns>
		public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
		{
			if (incomingCodeParser is CSharpCodeParser)
                return new CSharp.NancyCSharpRazorCodeParser();

            if (incomingCodeParser is VBCodeParser)
                return new VisualBasic.NancyVisualBasicRazorCodeParser();

			return base.DecorateCodeParser(incomingCodeParser);
		}
    }
}
