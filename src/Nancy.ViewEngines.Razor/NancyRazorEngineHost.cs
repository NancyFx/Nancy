using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;

namespace Nancy.ViewEngines.Razor
{
    public class NancyRazorEngineHost : RazorEngineHost
    {
        private readonly IRazorConfiguration configuration;

        public NancyRazorEngineHost(RazorCodeLanguage language, IRazorConfiguration configuration)
            : base(language)
		{
            DefaultBaseClass = typeof(NancyRazorViewBase).FullName;
            DefaultNamespace = "RazorOutput";
            DefaultClassName = "RazorView";

            GeneratedClassContext = new GeneratedClassContext("Execute", "Write", "WriteLiteral", null, null, null, "DefineSection");

            NamespaceImports.Add("System");
            NamespaceImports.Add("System.IO");
            NamespaceImports.Add("System.Web");
            NamespaceImports.Add("Microsoft.CSharp.RuntimeBinder");

            var namespaces = configuration.GetDefaultNamespaces();
            if (namespaces != null)
            {
                foreach (var n in namespaces)
                    NamespaceImports.Add(n);
            }
		}

		public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
		{
			if (incomingCodeGenerator is CSharpRazorCodeGenerator)
				return new CSharp.NancyCSharpRazorCodeGenerator(incomingCodeGenerator.ClassName, incomingCodeGenerator.RootNamespaceName, incomingCodeGenerator.SourceFileName, incomingCodeGenerator.Host);

			return base.DecorateCodeGenerator(incomingCodeGenerator);
		}

		public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
		{
			if (incomingCodeParser is CSharpCodeParser)
                return new CSharp.NancyRazorCSharpCodeParser();

			return base.DecorateCodeParser(incomingCodeParser);
		}
    }
}
