namespace Nancy.ViewEngines.NDjango
{
    public class NDjangoViewEngine : ViewEngine
    {
        private readonly INDjangoViewCompiler compiler;

        public NDjangoViewEngine() : this(new AspNetTemplateLocator(), new NDjangoViewCompiler())
        {
        }

        public NDjangoViewEngine(IViewLocator viewTemplateLocator, INDjangoViewCompiler compiler)
            : base(viewTemplateLocator)
        {
            this.compiler = compiler;
        }

        protected override IView GetCompiledView<TModel>(ViewLocationResult result)
        {
            return compiler.GetCompiledView(result.Location);
        }
    }
}