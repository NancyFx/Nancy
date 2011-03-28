namespace Nancy.ModelBinding
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// <para>
    /// A simple default model binder.
    /// </para>
    /// <para>
    /// Uses the DynamicDictionary from Request.Form to attempt to
    /// populate the model object.
    /// </para>
    /// <para>
    /// Currently the model must either have a default constructor and public settable
    /// properties.
    /// </para>
    /// </summary>
    public class DefaultModelBinder : IModelBinder
    {
        /// <summary>
        /// Whether the binder can bind to the given model type
        /// </summary>
        /// <param name="modelType">Required model type</param>
        /// <returns>True if binding is possible, false otherwise</returns>
        public bool CanBind(Type modelType)
        {
            return true;
        }

        /// <summary>
        /// Bind to the given model type
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="modelType">Model type to bind to</param>
        /// <returns>Bound model</returns>
        public object Bind(NancyContext context, Type modelType)
        {
            // Currently this is *extremely* dumb and PoC only :-)
            // Models must have a default constructor and public settable properties
            // for each member. Also only supports very basic types and only works
            // if the input type is directly assignable to the type in the object.
            var model = Activator.CreateInstance(modelType);

            var modelProperties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite);
            DynamicDictionary formDictionary = context.Request.Form;

            foreach (var modelProperty in modelProperties)
            {
                var dictionaryItem = (DynamicDictionaryValue)formDictionary[modelProperty.Name];

                // TODO - Use type converters if they are available
                if (dictionaryItem.HasValue)
                {
                    if (modelProperty.PropertyType.IsAssignableFrom(dictionaryItem.Value.GetType()))
                    {
                        modelProperty.SetValue(model, dictionaryItem.Value, null);
                    }
                    else
                    {
                        //modelProperty.SetValue(model, dictionaryItem, null);
                    }
                }
            }

            return model;
        }
    }    
}