namespace Nancy.ViewEngines.Razor
{
    using System.IO;
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using System.Web.Razor.Parser;

    public class NancyRazorCodeEngineHost : NancyRazorEngineHost
    {
        public NancyRazorCodeEngineHost(RazorCodeLanguage language, string viewLocation) : base(language)
        {
            this.DefaultBaseClass = typeof(NancyRazorCodeBase).FullName;
            this.DefaultClassName = this.GetClassName(viewLocation);
            this.StaticHelpers = true;
        }

        public override void PostProcessGeneratedCode(CodeGeneratorContext context)
        {
            base.PostProcessGeneratedCode(context);

            // Web code pages don't need an execute method so remove it.
            context.GeneratedClass.Members.Remove(context.TargetMethod);
        }

        protected string GetClassName(string viewLocation)
        {
            return ParserHelpers.SanitizeClassName(Path.GetFileNameWithoutExtension(viewLocation));
        }
    }
}