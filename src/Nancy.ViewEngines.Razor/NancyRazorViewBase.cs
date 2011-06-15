namespace Nancy.ViewEngines.Razor
{
    using System.IO;
    using System.Web;

    public abstract class NancyRazorViewBase
    {
        public TextWriter Writer { get; set; }

        public string Code { get; set; }

        public string Path { get; set; }

        public dynamic Model { get; set; }

        public HtmlHelpers Html { get; set; }

        public abstract void Execute();

        // Writes the results of expressions like: "@foo.Bar"
        public virtual void Write(object value)
        {
            WriteLiteral(HttpUtility.HtmlEncode(value));
        }

        // Writes literals like markup: "<p>Foo</p>"
        public virtual void WriteLiteral(object value)
        {
            Writer.Write(value);
        }
    }
}