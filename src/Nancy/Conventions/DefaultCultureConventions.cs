namespace Nancy.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using Nancy.Session;

    public class DefaultCultureConventions : IConvention
    {
        /// <summary>
        /// Initialise culture conventions
        /// </summary>
        /// <param name="conventions"></param>
        public void Initialise(NancyConventions conventions)
        {
            this.ConfigureDefaultConventions(conventions);
        }

        /// <summary>
        /// Determine if culture conventions are valid
        /// </summary>
        /// <param name="conventions"></param>
        /// <returns></returns>
        public Tuple<bool, string> Validate(NancyConventions conventions)
        {
            if (conventions.CultureConventions == null)
            {
                return Tuple.Create(false, "The culture conventions cannot be null.");
            }

            return (conventions.CultureConventions.Count > 0) ?
               Tuple.Create(true, string.Empty) :
               Tuple.Create(false, "The culture conventions cannot be empty.");
        }

        /// <summary>
        /// Setup default conventions
        /// </summary>
        /// <param name="conventions"></param>
        private void ConfigureDefaultConventions(NancyConventions conventions)
        {
            conventions.CultureConventions = new List<Func<NancyContext, CultureInfo>>(6)
            {
                BuiltInCultureConventions.FormCulture,
                BuiltInCultureConventions.HeaderCulture,
                BuiltInCultureConventions.SessionCulture,
                BuiltInCultureConventions.CookieCulture,
                BuiltInCultureConventions.ThreadCulture
            };

        }

    }
}
