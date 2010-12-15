namespace Nancy.ViewEngines.NDjango
{
    using System.IO;

    public class NDjangoView : IView
    {
        private readonly TextReader reader;

        public NDjangoView(TextReader reader)
        {
            this.reader = reader;
        }

        public string Code { get; set; }

        public object Model { get; set; }

        public TextWriter Writer { get; set; }

        public void Execute()
        {
            var buffer = new char[4096];
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                Writer.Write(buffer, 0, count);
            }

            Writer.Flush();
        }
    }
}