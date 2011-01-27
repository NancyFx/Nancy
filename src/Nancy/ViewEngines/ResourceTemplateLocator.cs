using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Nancy.ViewEngines
{
    public class ResourceTemplateLocator : IViewLocator
    {
        public ViewLocationResult GetTemplateContents(string viewTemplate)
        {
            // We will find you!!!
            var rightAssembly = FindAssemblyResourceIsIn(viewTemplate);

            if (rightAssembly == null) throw new FileNotFoundException(string.Format("the view '{0}' was not found", viewTemplate), viewTemplate);

            var stream = rightAssembly.GetManifestResourceStream(viewTemplate);
            if (stream == null) throw new FileNotFoundException(string.Format("the view '{0}' was not found", viewTemplate), viewTemplate);

            return new ViewLocationResult(viewTemplate, new StreamReader(stream));
        }

        private static Assembly FindAssemblyResourceIsIn(string viewTemplate)
        {
            // Get the Resources in all assemblies that are loaded
            var resources = AppDomain.CurrentDomain
                                     .GetAssemblies()
                                     .ToList()
                                     .Select(assembly =>
                                     {
                                         try { return new { assembly, resources = assembly.GetManifestResourceNames() }; }
                                         catch (Exception) { return new { assembly, resources = new string[] { }}; }
                                     }).ToList();

            // now let's find the resource by name
            return (from r in resources
                    where r.resources.Contains(viewTemplate)
                    select r.assembly).FirstOrDefault();
        }
    }
}
