using System.IO;

namespace Nancy.ViewEngines.Razor {
    public class ViewLocationResult {
        public ViewLocationResult(string location, TextReader contents) {
            Location = location;
            Contents = contents;
        }

        public string Location { get; private set; }
        public TextReader Contents { get; private set; }
    }
}
