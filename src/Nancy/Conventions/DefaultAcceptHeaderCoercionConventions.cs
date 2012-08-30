using System;
using System.Collections.Generic;

namespace Nancy.Conventions
{
    public class DefaultAcceptHeaderCoercionConventions : IConvention
    {
        public void Initialise(NancyConventions conventions)
        {
            this.ConfigureDefaultConventions(conventions);
        }

        public Tuple<bool, string> Validate(NancyConventions conventions)
        {
            if (conventions.StaticContentsConventions == null)
            {
                return Tuple.Create(false, "The accept header coercion conventions cannot be null.");
            }

            return Tuple.Create(true, string.Empty);
        }

        private void ConfigureDefaultConventions(NancyConventions conventions)
        {
            conventions.AcceptHeaderCoercionConventions = new List<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>>(2)
                                                              {
                                                                  BuiltInAcceptHeaderCoercions.CoerceStupidBrowsers, 
                                                                  BuiltInAcceptHeaderCoercions.BoostHtml,
                                                                  BuiltInAcceptHeaderCoercions.CoerceBlankAcceptHeader,
                                                              };
        }
    }
}