namespace Nancy.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// This is a wrapper around the type 
    /// 'IEnumerable<Func<Type, IEnumerable<Type>, Type>>' and its 
    /// only purpose is to make Ninject happy which was throwing an exception 
    /// when constructor injecting this type.
    /// </summary>
    public class MetadataModuleConventions : IEnumerable<Func<Type, IEnumerable<Type>, Type>>
    {
        private readonly IEnumerable<Func<Type, IEnumerable<Type>, Type>> conventions;

        public MetadataModuleConventions(IEnumerable<Func<Type, IEnumerable<Type>, Type>> conventions)
        {
            this.conventions = conventions;
        }

        public IEnumerator<Func<Type, IEnumerable<Type>, Type>> GetEnumerator()
        {
            return this.conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
