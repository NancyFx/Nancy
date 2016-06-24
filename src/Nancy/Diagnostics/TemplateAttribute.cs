namespace Nancy.Diagnostics
{
    using System;

    /// <summary>
    /// Attribute for defining an HTML template.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TemplateAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        public string Template { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateAttribute"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        public TemplateAttribute(string template)
        {
            this.Template = template;
        }
    }
}