namespace Nancy.ViewEngines.Razor
{
    public class RazorViewEngine : ViewEngine
    {
        private readonly IRazorViewCompiler compiler;

        public RazorViewEngine() : this(new AspNetTemplateLocator(), new RazorViewCompiler())
        {
        }

        public RazorViewEngine(IViewLocator viewTemplateLocator, IRazorViewCompiler compiler) : base(viewTemplateLocator)
        {
            this.compiler = compiler;
        }

        protected override IView GetCompiledView<TModel>(ViewLocationResult result)
        {
            return compiler.GetCompiledView(result.Contents);
        }
    }
}