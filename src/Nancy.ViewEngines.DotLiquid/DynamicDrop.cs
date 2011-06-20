namespace Nancy.ViewEngines.DotLiquid
{
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
                return "[Model is null]";
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                return "[Invalid model property name]";
            }

            var value = (this.model.GetType().Equals(typeof(Dictionary<string, object>))) ?
                GetExpandoObjectValue(propertyName) :
                GetPropertyValue(propertyName);

            return value ?? string.Format("[Can't find :{0} in the model]", propertyName);
        }

        private object GetExpandoObjectValue(string propertyName)
        {
            return (!this.model.ContainsKey(propertyName)) ?
                null :
                this.model[propertyName];
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
                ? new Dictionary<string, object>(model)
                : model;
        }
    }
}