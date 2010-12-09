using System.IO;

namespace Nancy.ViewEngines.Razor {
    public class ViewResult {
        public ViewResult(IView view, string location) {
            View = view;
            Location = location;
        }

        public IView View {
            get;
            private set;
        }

        public string Location {
            get;
            private set;
        }

        public void Execute(Stream stream) {
            // The caller needs to close the stream.
            var writer = new StreamWriter(stream);
            View.Writer = writer;
            View.Execute();
            writer.Flush();
        }
    }
}
