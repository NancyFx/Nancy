namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using Nancy.Helpers;

    /// <summary>
    /// Default base class for nancy razor views
    /// </summary>
    public abstract class NancyRazorViewBase : NancyRazorViewBase<dynamic>
    {
    }

    /// <summary>
    /// Base class for nancy razor views.
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    public abstract class NancyRazorViewBase<TModel> : INancyRazorView
    {
        private readonly StringBuilder contents;
        private string childBody;
        private IDictionary<string, string> childSections;

        /// <summary>
        /// Gets the Html helper.
        /// </summary>
        public HtmlHelpers<TModel> Html { get; private set; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TModel Model { get; private set; }

        /// <summary>
        /// Gets the Url helper.
        /// </summary>
        public UrlHelpers<TModel> Url { get; private set; }

        /// <summary>
        /// Non-model specific data for rendering in the response
        /// </summary>
        public dynamic ViewBag { get; private set; }

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
            get { return !String.IsNullOrEmpty(this.Layout); }
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

        /// <summary>
        /// Used to return text resources
        /// </summary>
        public dynamic Text
        {
            get
            {
                return this.RenderContext.TextResourceFinder;
            }
        }

        /// <summary>
        /// Executes the view.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Initializes the specified engine.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="renderContext">The render context.</param>
        /// <param name="model">The model.</param>
        public virtual void Initialize(RazorViewEngine engine, IRenderContext renderContext, object model)
        {
            var castedModel = default(TModel);

            if (model != null)
            {
                castedModel = (TModel)model;
            }

            this.RenderContext = renderContext;
            this.Html = new HtmlHelpers<TModel>(engine, renderContext, castedModel);
            this.Model = castedModel;
            this.Url = new UrlHelpers<TModel>(engine, renderContext);
            this.ViewBag = renderContext.Context.ViewBag;
        }

        protected IRenderContext RenderContext { get; set; }

        /// <summary>
        /// Gets the current <see cref="NancyContext"/> instance.
        /// </summary>
        /// <value>A <see cref="NancyContext"/> instance.</value>
        public NancyContext Context
        {
            get { return this.RenderContext.Context; }
        }

        /// <summary>
        /// Gets the current <see cref="Request"/> instance.
        /// </summary>
        /// <value>A <see cref="Request"/> instance.</value>
        public Request Request
        {
            get { return this.Context.Request; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyRazorViewBase"/> class.
        /// </summary>
        protected NancyRazorViewBase()
        {
            this.Sections = new Dictionary<string, Action>();
            this.contents = new StringBuilder();
        }

        /// <summary>
        /// Writes the results of expressions like: "@foo.Bar"
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void Write(object value)
        {
            WriteLiteral(HtmlEncode(value));
        }

        /// <summary>
        /// Writes literals like markup: "<p>Foo</p>"
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void WriteLiteral(object value)
        {
            contents.Append(value);
        }

        public virtual void WriteAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values)
        {
            var attributeValue = this.BuildAttribute(name, prefix, suffix, values);
            this.WriteLiteral(attributeValue);
        }

        public virtual void WriteAttributeTo(TextWriter writer, string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values)
        {
            var attributeValue = this.BuildAttribute(name, prefix, suffix, values);
            this.WriteLiteralTo(writer, attributeValue);
        }

        private string BuildAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix,
                                      params AttributeValue[] values)
        {
            var writtenAttribute = false;
            var attributeBuilder = new StringBuilder(prefix.Item1);

            foreach (var value in values)
            {
                if (this.ShouldWriteValue(value.Value.Item1))
                {
                    var stringValue = this.GetStringValue(value);
                    var valuePrefix = value.Prefix.Item1;

                    // encode anything that hasn't opted out of it
                    if (!(value.Value.Item1 is IHtmlString))
                    {
                        stringValue = HtmlEncode(stringValue);
                    }

                    if (!string.IsNullOrEmpty(valuePrefix))
                    {
                        attributeBuilder.Append(valuePrefix);
                    }

                    attributeBuilder.Append(stringValue);
                    writtenAttribute = true;
                }
            }

            attributeBuilder.Append(suffix.Item1);

            var renderAttribute = writtenAttribute || values.Length == 0;

            if (renderAttribute)
            {
                return attributeBuilder.ToString();
            }

            return string.Empty;
        }

        private string GetStringValue(AttributeValue value)
        {
            if (value.IsLiteral)
            {
                return (string)value.Value.Item1;
            }

            if (value.Value.Item1 is IHtmlString)
            {
                return ((IHtmlString)value.Value.Item1).ToHtmlString();
            }

            if (value.Value.Item1 is DynamicDictionaryValue)
            {
                var dynamicValue = (DynamicDictionaryValue)value.Value.Item1;
                return dynamicValue.HasValue ? dynamicValue.Value.ToString() : string.Empty;
            }

            return value.Value.Item1.ToString();
        }

        private bool ShouldWriteValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool)
            {
                var boolValue = (bool)value;

                return boolValue;
            }

            return true;
        }

        /// <summary>
        /// Writes the provided <paramref name="value"/> to the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> that should be written to.</param>
        /// <param name="value">The value that should be written.</param>
        public virtual void WriteTo(TextWriter writer, object value)
        {
            writer.Write(HtmlEncode(value));
        }

        /// <summary>
        /// Writes the provided <paramref name="value"/>, as a literal, to the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> that should be written to.</param>
        /// <param name="value">The value that should be written as a literal.</param>
        public virtual void WriteLiteralTo(TextWriter writer, object value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes the provided <paramref name="value"/> to the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> that should be written to.</param>
        /// <param name="value">The <see cref="HelperResult"/> that should be written.</param>
        public virtual void WriteTo(TextWriter writer, HelperResult value)
        {
            if (value != null)
            {
                value.WriteTo(writer);
            }
        }

        /// <summary>
        /// Writes the provided <paramref name="value"/>, as a literal, to the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> that should be written to.</param>
        /// <param name="value">The <see cref="HelperResult"/> that should be written as a literal.</param>
        public virtual void WriteLiteralTo(TextWriter writer, HelperResult value)
        {
            if (value != null)
            {
                value.WriteTo(writer);
            }
        }

        /// <summary>
        /// Stores sections
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="action">The action.</param>
        public virtual void DefineSection(string sectionName, Action action)
        {
            this.Sections.Add(sectionName, action);
        }

        /// <summary>
        /// Renders the section.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns></returns>
        public virtual object RenderSection(string sectionName)
        {
            return this.RenderSection(sectionName, true);
        }

        /// <summary>
        /// Renders the section.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
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

        /// <summary>
        /// Renders the body.
        /// </summary>
        /// <returns></returns>
        public virtual object RenderBody()
        {
            this.contents.Append(this.childBody);

            return null;
        }

        ///<summary>
        ///Indicates if a section is defined.
        ///</summary>
        public virtual bool IsSectionDefined(string sectionName)
        {
            return this.childSections.ContainsKey(sectionName);
        }

        public virtual string ResolveUrl(string url)
        {
            return this.RenderContext.ParsePath(url);
        }

        /// <summary>
        /// Executes the view.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="sectionContents">The section contents.</param>
        public void ExecuteView(string body, IDictionary<string, string> sectionContents)
        {
            this.childBody = body ?? string.Empty;
            this.childSections = sectionContents ?? new Dictionary<string, string>();

            try
            {
                this.Execute();
            }
            catch (NullReferenceException e)
            {
                throw new ViewRenderException("Unable to render the view.  Most likely the Model, or a property on the Model, is null", e);
            }

            this.Body = this.contents.ToString();

            this.SectionContents = new Dictionary<string, string>(this.Sections.Count);
            foreach (var section in this.Sections)
            {
                this.contents.Clear();
                try
                {
                    section.Value.Invoke();
                }
                catch (NullReferenceException e)
                {
                    throw new ViewRenderException(string.Format("A null reference was encountered while rendering the section {0}.  Does the section require a model? (maybe it wasn't passed in)", section.Key), e);
                }
                this.SectionContents.Add(section.Key, this.contents.ToString());
            }
        }

        /// <summary>
        /// Html encodes an object if required
        /// </summary>
        /// <param name="value">Object to potentially encode</param>
        /// <returns>String representation, encoded if necessary</returns>
        private string HtmlEncode(object value)
        {
            if (value == null)
            {
                return null;
            }

            var str = value as IHtmlString;

            var currentCulture = this.Context.Culture ?? CultureInfo.CurrentCulture;

            return str != null ? str.ToHtmlString() : HttpUtility.HtmlEncode(Convert.ToString(value, currentCulture));
        }
    }
}
