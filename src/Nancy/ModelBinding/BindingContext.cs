namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class BindingContext
    {
        public NancyContext Context { get; set; }

        public Type DestinationType { get; set; }

        public object Model { get; set; }

        public IEnumerable<PropertyInfo> ValidModelProperties { get; set; }

        public IDictionary<string, string> FormFields { get; set; }

        public IEnumerable<ITypeConverter> TypeConverters { get; set; }
    }
}