namespace Nancy.ViewEngines.NHaml
{
    public class NHamlViewEngine : ViewEngine
    {
        public NHamlViewEngine() : base(new AspNetTemplateLocator(), new NHamlViewCompiler())
        {
        }
    }
}