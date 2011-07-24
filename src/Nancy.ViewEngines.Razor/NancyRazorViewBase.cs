namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    public abstract class NancyRazorViewBase
    {
        private StringBuilder contents;

        private string childBody;

        private IDictionary<string, string> childSections;

        public String Body { get; private set; }

        public IDictionary<string, string> SectionContents { get; set; }

        public string Layout { get; set; }

        public bool HasLayout
        {
            get
            {
                return !String.IsNullOrEmpty(this.Layout);
            }
        }

        public string Code { get; set; }

        public string Path { get; set; }

        public dynamic Model { get; set; }

        public HtmlHelpers Html { get; set; }

        public UrlHelpers Url { get; set; }

        public IDictionary<string, Action> Sections { get; set; }

        public abstract void Execute();

        protected NancyRazorViewBase()
        {
            this.Sections = new Dictionary<string, Action>();
            this.contents = new StringBuilder();
        }

        // Writes the results of expressions like: "@foo.Bar"
        public virtual void Write(object value)
        {
            WriteLiteral(HttpUtility.HtmlEncode(value));
        }

        // Writes literals like markup: "<p>Foo</p>"
        public virtual void WriteLiteral(object value)
        {
            contents.Append(value);
        }

        // Stores sections
        public virtual void DefineSection(string sectionName, Action action)
        {
            this.Sections.Add(sectionName, action);
        }

        public virtual object RenderSection(string sectionName)
        {
            return this.RenderSection(sectionName, true);
        }

        public virtual object RenderSection(string sectionName, bool required)
        {
            string sectionContent;

            var exists = this.childSections.TryGetValue(sectionName, out sectionContent);
            if (!exists && required)
            {
                throw new InvalidOperationException("Section name " + sectionName + " not found and is required.");
            }

            this.contents.Append(sectionContent ?? String.Empty);

            return null;
        }

        public virtual object RenderBody()
        {
            this.contents.Append(this.childBody);

            return null;
        }

        public void ExecuteView(string body, IDictionary<string, string> sectionContents)
        {
            this.childBody = body ?? string.Empty;
            this.childSections = sectionContents ?? new Dictionary<string, string>();

            this.Execute();

            this.Body = this.contents.ToString();

            this.SectionContents = new Dictionary<string, string>(this.Sections.Count);
            foreach (var section in this.Sections)
            {
                this.contents.Clear();
                section.Value.Invoke();
                this.SectionContents.Add(section.Key, this.contents.ToString());
            }
        }
    }
}