namespace Nancy.Conventions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Wires up the default conventions for the accept header coercion
    /// </summary>
    public class DefaultAcceptHeaderCoercionConventions : IConvention
    {
        /// <summary>
        /// Initialise any conventions this class "owns"
        /// </summary>
        /// <param name="conventions">Convention object instance</param>
        public void Initialise(NancyConventions conventions)
        {
            this.ConfigureDefaultConventions(conventions);
        }

        /// <summary>
        /// Asserts that the conventions that this class "owns" are valid
        /// </summary>
        /// <param name="conventions">Conventions object instance</param>
        /// <returns>
        /// Tuple containing true/false for valid/invalid, and any error messages
        /// </returns>
        public Tuple<bool, string> Validate(NancyConventions conventions)
        {
            if (conventions.AcceptHeaderCoercionConventions == null)
            {
                return Tuple.Create(false, "The accept header coercion conventions cannot be null.");
            }

            return Tuple.Create(true, string.Empty);
        }

        private void ConfigureDefaultConventions(NancyConventions conventions)
        {
            conventions.AcceptHeaderCoercionConventions = new List<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>>(2)
                                                              {
                                                                 BuiltInAcceptHeaderCoercions.BoostHtml,
                                                                 BuiltInAcceptHeaderCoercions.CoerceBlankAcceptHeader,
                                                              };
        }
    }
}
