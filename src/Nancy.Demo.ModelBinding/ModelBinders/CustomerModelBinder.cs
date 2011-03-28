namespace Nancy.Demo.ModelBinding.ModelBinders
{
    using System;
    using Models;
    using Nancy.ModelBinding;

    /// <summary>
    /// Sample model binder that manually binds customer models
    /// </summary>
    public class CustomerModelBinder : IModelBinder
    {
        /// <summary>
        /// Whether the binder can bind to the given model type
        /// </summary>
        /// <param name="modelType">Required model type</param>
        /// <returns>True if binding is possible, false otherwise</returns>
        public bool CanBind(Type modelType)
        {
            return modelType == typeof(Customer);
        }

        /// <summary>
        /// Bind to the given model type
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="modelType">Model type to bind to</param>
        /// <returns>Bound model</returns>
        public object Bind(NancyContext context, Type modelType)
        {
            var customer = new Customer
                               {
                                   Name = context.Request.Form["Name"],
                                   RenewalDate = context.Request.Form["RenewalDate"]
                               };

            return customer;
        }
    }
}