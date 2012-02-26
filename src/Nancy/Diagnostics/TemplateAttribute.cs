namespace Nancy.Diagnostics
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TemplateAttribute : Attribute
    {
        public string Template { get; set; }

        public TemplateAttribute(string template)
        {
            this.Template = template;
        }
    }
}