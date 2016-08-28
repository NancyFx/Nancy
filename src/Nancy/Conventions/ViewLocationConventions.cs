namespace Nancy.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Nancy.ViewEngines;

    /// <summary>
    /// This is a wrapper around the type
    /// <c>IEnumerable&lt;Func&lt;string, object, ViewLocationContext, string&gt;&gt;</c> and its
    /// only purpose is to make Ninject happy which was throwing an exception
    /// when constructor injecting this type.
    /// </summary>
    public class ViewLocationConventions : IEnumerable<Func<string, object, ViewLocationContext, string>>
    {
        private readonly IEnumerable<Func<string, object, ViewLocationContext, string>> conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocationConventions"/> class.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        public ViewLocationConventions(IEnumerable<Func<string, object, ViewLocationContext, string>> conventions)
        {
            this.conventions = conventions;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Func<string, object, ViewLocationContext, string>> GetEnumerator()
        {
            return conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}