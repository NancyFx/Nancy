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

      public Func<string, object, Action<Stream>> Executor
      {
         get { return (name, model) => NDjangoViewEngineExtensions.Django(null, name, model); }
      }
   }
}