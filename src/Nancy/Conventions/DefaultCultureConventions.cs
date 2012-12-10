
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
        public void Initialise(NancyConventions conventions)
        {
            this.ConfigureDefaultConventions(conventions);

        }

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


        private void ConfigureDefaultConventions(NancyConventions conventions)
        {
            conventions.CultureConventions = new List<Func<NancyContext, CultureInfo>>
            {
                (ctx) =>
                    {
                        if (ctx.Request.Form["CurrentCulture"] != null)
                        {
                            return (CultureInfo) ctx.Request.Form["CurrentCulture"];
                        }

                        return null;
                    },

                (ctx) =>
                    {
                        var firstParameter =
                            ctx.Request.Url.Path.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries).
                                FirstOrDefault();

                        if (firstParameter != null && IsValidCultureInfoName(firstParameter))
                        {
                            return new CultureInfo(firstParameter);
                        }

                        return null;
                    },
               (ctx) =>
                   {
                       if (!(ctx.Request.Session is NullSessionProvider) && ctx.Request.Session["CurrentCulutre"] != null)
                       {
                           return (CultureInfo)ctx.Request.Session["CurrentCulture"];
                       }

                       return null;
                   },

                (ctx) =>
                    {
                        string cookieCulture = null;

                        if (ctx.Request.Cookies.TryGetValue("CurrentCulture", out cookieCulture))
                        {
                          return  new CultureInfo(cookieCulture);
                        }

                        return null;
                    },
                    
             
                (ctx) => Thread.CurrentThread.CurrentCulture
            };

        }

        private static bool IsValidCultureInfoName(string name)
        {
            var validCulture = false;
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                if (culture.Name == name)
                {
                    validCulture = true;
                    break;
                }
            }
            return validCulture;
        }

    }
}
