using System.IO;
using System.Web;

namespace Nancy.ViewEngines.Razor {
    public abstract class RazorViewBase : IView {
        public TextWriter Writer { get; set; }

        // Writes the results of expressions like: "@foo.Bar"
        public virtual void Write(object value) {
            WriteLiteral(HttpUtility.HtmlEncode(value));
        }

        // Writes literals like markup: "<p>Foo</p>"
        public virtual void WriteLiteral(object value) {
            Writer.Write(value);
        }

        public abstract void Execute();

        public string Code { get; set; }

        public string Path { get; set; }

        public dynamic Model { get; set; }
    }
}
