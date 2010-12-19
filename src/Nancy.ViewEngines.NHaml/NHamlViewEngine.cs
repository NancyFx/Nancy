namespace Nancy.ViewEngines.NHaml
{
    public class NHamlViewEngine : ViewEngine
    {
        private readonly INHamlViewCompiler compiler;

        public NHamlViewEngine() : this(new AspNetTemplateLocator(), new NHamlViewCompiler())
        {
            compiler = new NHamlViewCompiler();
        }

        public NHamlViewEngine(IViewLocator viewTemplateLocator, INHamlViewCompiler compiler) : base(viewTemplateLocator)
        {
            this.compiler = compiler;
        }

        protected override IView GetCompiledView<TModel>(ViewLocationResult result)
        {
            return compiler.GetCompiledView<TModel>(result.Location);
        }
    }
}