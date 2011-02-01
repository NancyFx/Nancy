namespace Nancy.ViewEngines.NDjango
{
    public class NDjangoViewRegistry : IViewEngineRegistry
    {
        private readonly NDjangoViewEngine viewEngine;

        public NDjangoViewRegistry(IViewLocator viewLocator)
        {
            viewEngine = new NDjangoViewEngine(viewLocator);
        }

        public IViewEngine ViewEngine
        {
            get { return viewEngine; }
        }

       public Action<Stream> Execute<TModel>(string viewTemplate, TModel model)
       {
           var viewEngine = new NDjangoViewEngine();
           return stream =>
                      {
                          var result = viewEngine.RenderView(viewTemplate, model);
                          result.Execute(stream);
                      };
       }
   }
}
