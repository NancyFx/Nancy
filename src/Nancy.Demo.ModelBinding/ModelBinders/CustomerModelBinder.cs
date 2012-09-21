using System.Linq;

namespace Nancy.Demo.ModelBinding.ModelBinders
{
    using System;
    using Models;
    using Nancy.ModelBinding;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Sample model binder that manually binds customer models
    /// </summary>
    public class CustomerModelBinder : IModelBinder
    {

        private IList<PropertyInfo> boundProperties;

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
        /// <param name="blackList">Blacklisted property names</param>
        /// <returns>Bound model</returns>
        public object Bind(NancyContext context, Type modelType, params string[] blackList)
        {

            var customer = new Customer
                               {
                                   Name = context.Request.Form["Name"],
                                   RenewalDate = context.Request.Form["RenewalDate"]
                               };


            boundProperties = (from prop in customer.GetType().GetProperties()
                         where prop.Name == "Name" || prop.Name == "RenewalDate"
                         select prop).ToList();


            return customer;
        }

        /// <summary>
        /// Is a record of what properties have been bound by this binder during Bind()
        /// </summary>
        /// <returns>List of bound properties</returns>
        public IEnumerable<PropertyInfo> BoundProperties
        {
            get { return boundProperties; }
        }
    }
}