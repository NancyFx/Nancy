namespace Nancy.ModelBinding.DefaultConverters
{
    using System;
    using Extensions;

    public class NumericConverter : ITypeConverter
    {
        public bool CanConvertTo(Type destinationType, BindingContext context)
        {
            return destinationType.IsNumeric();
        }

        public object Convert(string input, Type destinationType, BindingContext context)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return System.Convert.ChangeType(input, destinationType, context.Context.Culture);
        }
    }
}
