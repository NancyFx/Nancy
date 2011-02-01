namespace Nancy.ViewEngines
{
    using System.IO;

    public class StaticView : IView
    {
        private readonly TextReader contents;

        public StaticView(TextReader contents)
        {
            this.contents = contents;
        }

        public object Model
        {
            get { return null; }
            set
            {
            }
        }

        public TextWriter Writer { get; set; }

        public void Execute()
        {
            Writer.Write(contents.ReadToEnd());
            Writer.Flush();
        }
    }
}