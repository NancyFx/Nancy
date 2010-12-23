namespace Nancy.ViewEngines.NHaml
{
   using System;
   using System.IO;

   public class NHamlViewRegistry : IViewEngineRegistry
   {
      public string Extension
      {
         get { return ".haml"; }
      }

      public Func<string, object, Action<Stream>> Executor
      {
         get { return (name, model) => NHamlViewEngineExtensions.Haml(null, name, model); }
      }
   }
}