namespace Nancy.ViewEngines
{
    using System.IO;

    public class ViewLocationResult
    {
        public ViewLocationResult(string location, TextReader contents)
        {
            this.Location = location;
            this.Contents = contents;
        }

        public string Location { get; private set; }

        public TextReader Contents { get; private set; }
    }
}