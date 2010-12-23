namespace Nancy.ViewEngines.NHaml
{    
    using System.IO;
    using System.Text;
    using global::NHaml.TemplateResolution;

    public class ViewSource : IViewSource
    {
        private readonly TextReader textReader;

        public ViewSource(TextReader textReader)
        {
            this.textReader = textReader;
        }

        public StreamReader GetStreamReader()
        {
            var text = textReader.ReadToEnd();
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            var memoryStream = new MemoryStream(bytes);

            return new StreamReader(memoryStream);
        }      

        public string Path
        {
            //TODO:
            get { return "/Unknown"; }
        }

        public bool IsModified
        {
            get { return true; }
        }
    }
}