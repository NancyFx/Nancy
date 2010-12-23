namespace Nancy.ViewEngines.NDjango
{
    using System;
    using System.IO;
    using global::NDjango.Interfaces;

    public class TemplateLoader : ITemplateLoader
    {
        private readonly TextReader textReader;

        public TemplateLoader(TextReader textReader)
        {
            this.textReader = textReader;
        }

        public TextReader GetTemplate(string path)
        {
            return textReader;
        }

        public bool IsUpdated(string path, DateTime timestamp)
        {
            return true;
        }
    }
}