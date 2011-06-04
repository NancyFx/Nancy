namespace Nancy.ViewEngines.Spark.Descriptors
{
    using System;
    using global::Spark;

    public static class DescriptorFilterExtensions
    {
        public static void AddFilter(this ISparkServiceContainer target, IDescriptorFilter filter)
        {
            target.GetService<IDescriptorBuilder>().AddFilter(filter);
        }

        public static void AddFilter(this SparkViewEngineWrapper target, IDescriptorFilter filter)
        {
            target.DescriptorBuilder.AddFilter(filter);
        }

        public static void AddFilter(this IDescriptorBuilder target, IDescriptorFilter filter)
        {
            if (!(target is DefaultDescriptorBuilder))
            {
                throw new InvalidCastException("IDescriptorFilters may only be added to DefaultDescriptorBuilder");
            }

            ((DefaultDescriptorBuilder) target).AddFilter(filter);
        }

        public static void AddFilter(this DefaultDescriptorBuilder target, IDescriptorFilter filter)
        {
            target.Filters.Add(filter);
        }
    }
}