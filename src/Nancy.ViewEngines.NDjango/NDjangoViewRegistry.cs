namespace Nancy.ViewEngines.NDjango
{
   using System;
   using System.IO;

   public class NDjangoViewRegistry : IViewEngineRegistry
   {
       public string Extension
       {
           get { return ".django"; }
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