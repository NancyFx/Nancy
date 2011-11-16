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

        /// <summary>
        /// Gets the body.
        /// </summary>
        public String Body { get; private set; }

        /// <summary>
        /// Gets or sets the section contents.
        /// </summary>
        /// <value>
        /// The section contents.
        /// </value>
        public IDictionary<string, string> SectionContents { get; set; }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>
        /// The layout.
        /// </value>
        public string Layout { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has layout.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has layout; otherwise, <c>false</c>.
        /// </value>
        public bool HasLayout
        {
            get
            {
                return !String.IsNullOrEmpty(this.Layout);
            }
        }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public IDictionary<string, Action> Sections { get; set; }

        public abstract void Execute();

        public virtual void Initialize(RazorViewEngine engine, IRenderContext renderContext, object model)
        { }

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

    public abstract class NancyRazorViewBase<TModel> : NancyRazorViewBase
    {
        public HtmlHelpers<TModel> Html { get; private set; }

        public TModel Model { get; private set; }

        public UrlHelpers<TModel> Url { get; private set; }

        public override void Initialize(RazorViewEngine engine, IRenderContext renderContext, object model)
        {
            Html = new HtmlHelpers<TModel>(engine, renderContext, (TModel)model);
            Model = (TModel)model;
            Url = new UrlHelpers<TModel>(engine, renderContext);
        }
    }
}