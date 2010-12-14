namespace Nancy.ViewEngines.Razor
{
    using System.IO;

    public class ViewResult 
    {
        public ViewResult(IView view, string location)
        {
            this.View = view;
            this.Location = location;
        }

        public string Location { get; private set; }

        public IView View { get; private set; }

        public void Execute(Stream stream)
        {
            // The caller needs to close the stream.
            var writer = new StreamWriter(stream);
            View.Writer = writer;
            View.Execute();
            writer.Flush();
        }
    }
}