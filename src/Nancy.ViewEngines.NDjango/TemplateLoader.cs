namespace Nancy.ViewEngines.NDjango
{
    using System;
    using System.IO;
    using global::NDjango.Interfaces;

    public class TemplateLoader : ITemplateLoader
    {
        private readonly ViewLocationResult result;

        public TemplateLoader(ViewLocationResult result)
        {
            this.result = result;
        }

        public TextReader GetTemplate(string name)
        {
            if (name == result.Name)
                return result.Contents();

            return result.AdditionalFile(name).Contents();
        }

        public bool IsUpdated(string path, DateTime timestamp)
        {
            return true;
        }
    }
}