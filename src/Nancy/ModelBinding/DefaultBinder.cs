namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
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
    public class DefaultBinder : IBinder
    {
        private readonly IEnumerable<ITypeConverter> typeConverters;

        private readonly IEnumerable<IBodyDeserializer> bodyDeserializers;

        public DefaultBinder(IEnumerable<ITypeConverter> typeConverters, IEnumerable<IBodyDeserializer> bodyDeserializers)
        {
            if (typeConverters == null)
            {
                throw new ArgumentNullException("typeConverters");
            }

            if (bodyDeserializers == null)
            {
                throw new ArgumentNullException("bodyDeserializers");
            }

            this.typeConverters = typeConverters;
            this.bodyDeserializers = bodyDeserializers;
        }

        /// <summary>
        /// Bind to the given model type
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="modelType">Model type to bind to</param>
        /// <returns>Bound model</returns>
        public object Bind(NancyContext context, Type modelType)
        {
            var result = this.DeserializeBody(context, modelType);

            if (result != null)
            {
                return result;
            }

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

        private object DeserializeBody(NancyContext context, Type modelType)
        {
            return null;
            //var contentType = context.Request.Headers["Content-Type"];

            //if (string.IsNullOrEmpty(contentType))

            //    foreach (var bodyDeserializer in bodyDeserializers.Where(b => b.CanDeserialize(contentType)))
            //    {

            //    }
        }
    }
}