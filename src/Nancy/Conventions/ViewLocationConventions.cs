namespace Nancy.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Nancy.ViewEngines;
    /// <summary>
    /// This is a wrapper around the type 
    /// 'IEnumerable<Func<string, object, ViewLocationContext, string>>' and its 
    /// only purpose is to make Ninject happy which was throwing an exception 
    /// when constructor injecting this type.
    /// </summary>
    public class ViewLocationConventions : IEnumerable<Func<string, object, ViewLocationContext, string>>
    {
        private readonly IEnumerable<Func<string, object, ViewLocationContext, string>> conventions;

        public ViewLocationConventions(IEnumerable<Func<string, object, ViewLocationContext, string>> conventions)
        {
            this.conventions = conventions;
        }

        public IEnumerator<Func<string, object, ViewLocationContext, string>> GetEnumerator()
        {
            return conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class StaticContentsConventions : IEnumerable<Func<NancyContext, string, Response>>
    {
        private readonly IEnumerable<Func<NancyContext, string, Response>> conventions;

        public StaticContentsConventions(IEnumerable<Func<NancyContext, string, Response>> conventions)
        {
            this.conventions = conventions;
        }

        public IEnumerator<Func<NancyContext, string, Response>> GetEnumerator()
        {
            return conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}