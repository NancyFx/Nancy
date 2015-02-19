namespace Nancy.ViewEngines.DotLiquid
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    using global::DotLiquid;

    public class DynamicDrop : Drop
    {
        private readonly dynamic model;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDrop"/> class.
        /// </summary>
        /// <param name="model">The view model.</param>
        public DynamicDrop(dynamic model)
        {
            this.model = GetValidModelType(model);
        }

        public override object BeforeMethod(string propertyName)
        {
            if (model == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            Type modelType = this.model.GetType();
            object value = null;
            if(modelType.Equals(typeof(Dictionary<string, object>)))
            {
                value = GetExpandoObjectValue(propertyName);
            }
            else if (modelType.Equals(typeof(DynamicDictionary)))
            {
                value = GetDynamicDictionaryObjectValue(propertyName);
            }
            else
            {
                value = GetPropertyValue(propertyName);
            }
            return value;
        }

        private object GetExpandoObjectValue(string propertyName)
        {
            return (!this.model.ContainsKey(propertyName)) ?
                null :
                this.model[propertyName];
        }

        private object GetDynamicDictionaryObjectValue(string propertyName)
        {
            DynamicDictionaryValue dictionaryValue = this.model[propertyName] as DynamicDictionaryValue;
            return dictionaryValue == null || !dictionaryValue.HasValue ? null : dictionaryValue.Value;
        }

        private object GetPropertyValue(string propertyName)
        {
            var property = this.model.GetType().GetProperty(propertyName);

            return (property == null) ?
                null : 
                property.GetValue(this.model, null);
        }

        private static dynamic GetValidModelType(dynamic model)
        {
            if (model == null)
            {
                return null;
            }

            return model.GetType().Equals(typeof(ExpandoObject))
                ? new Dictionary<string, object>(model, StaticConfiguration.CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase)
                : model;
        }
    }
}