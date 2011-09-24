namespace Nancy.Conventions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the default static contents conventions.
    /// </summary>
    public class DefaultStaticContentsConventions : IConvention
    {
        /// <summary>
        /// Initialise any conventions this class "owns".
        /// </summary>
        /// <param name="conventions">Convention object instance.</param>
        public void Initialise(NancyConventions conventions)
        {
            conventions.StaticContentsConventions = new List<Func<NancyContext, string, Response>>
            {
                StaticContentConventionBuilder.AddDirectory("Content")
            };
        }

        /// <summary>
        /// Asserts that the conventions that this class "owns" are valid
        /// </summary>
        /// <param name="conventions">Conventions object instance.</param>
        /// <returns>Tuple containing true/false for valid/invalid, and any error messages.</returns>
        public Tuple<bool, string> Validate(NancyConventions conventions)
        {
            if (conventions.StaticContentsConventions == null)
            {
                return Tuple.Create(false, "The static contents conventions cannot be null.");
            }

            return (conventions.StaticContentsConventions.Count > 0) ?
                Tuple.Create(true, string.Empty) :
                Tuple.Create(false, "The static contents conventions cannot be empty.");
        }
    }
}