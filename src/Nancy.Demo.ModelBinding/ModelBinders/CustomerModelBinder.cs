namespace Nancy.Demo.ModelBinding.ModelBinders
{
    using System;

    using Nancy.Demo.ModelBinding.Models;
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
        /// <param name="instance">Optional existing instance</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blackList">Blacklisted property names</param>
        /// <returns>Bound model</returns>
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
        {
            var customer = (instance as Customer) ?? new Customer();

            customer.Name = customer.Name ?? context.Request.Form["Name"];
            customer.RenewalDate = customer.RenewalDate == default(DateTime) ? context.Request.Form["RenewalDate"] : customer.RenewalDate;

            return customer;
        }
    }
}