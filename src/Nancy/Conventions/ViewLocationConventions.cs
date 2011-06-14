using System;
using Nancy.ViewEngines;

namespace Nancy.Conventions
{
    public class ViewLocationConventions
    {
        private readonly Func<string, object, ViewLocationContext, string>[] conventions;

        public ViewLocationConventions(Func<string, object, ViewLocationContext, string>[] conventions)
        {
            this.conventions = conventions;
        }

        public bool HasConventions()
        {
            return conventions.Length > 0;
        }

        public Func<string, object, ViewLocationContext, string>[] GetConventions() { return conventions; } 
    }
}