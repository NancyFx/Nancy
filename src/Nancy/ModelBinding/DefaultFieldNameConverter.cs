namespace Nancy.ModelBinding
{
    /// <summary>
    /// Default field name converter
    /// Converts camel case to pascal case
    /// </summary>
    public class DefaultFieldNameConverter : IFieldNameConverter
    {
        /// <summary>
        /// Converts a field name to a property name
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Property name</returns>
        public string Convert(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return fieldName;
            }

            var first = fieldName.Substring(0, 1);
            var capitalFirst = first.ToUpperInvariant();

            if (first.Equals(capitalFirst))
            {
                return fieldName;
            }

            return string.Format("{0}{1}", capitalFirst, fieldName.Substring(1));
        }
    }
}