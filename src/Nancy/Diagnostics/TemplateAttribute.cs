namespace Nancy.Diagnostics
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TemplateAttribute : Attribute
    {
        public TemplateAttribute(string template)
        {
            this.Template = template;
        }

        public string Template { get; set; }
    }
}