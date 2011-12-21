namespace Nancy.Diagnostics
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; set; }

        public DescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}