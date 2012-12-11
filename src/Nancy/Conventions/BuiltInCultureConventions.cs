namespace Nancy.Conventions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Nancy.Session;

    /// <summary>
    /// Built in functions for determining current culture
    /// <seealso cref="DefaultCultureConventions"/>
    /// </summary>
    public static class BuiltInCultureConventions
    {
        /// <summary>
        /// Checks to see if the Form has a CurrentCulture key.
        /// </summary>
        /// <param name="context">NancyContext</param>
        /// <returns>CultureInfo if found in Form otherwise null</returns>
        public static CultureInfo FormCulture(NancyContext context)
        {
            if (context.Request.Form["CurrentCulture"] != null)
            {
                string cultureLetters = context.Request.Form["CurrentCulture"];
                if (!IsValidCultureInfoName(cultureLetters))
                    return null;

                return new CultureInfo(cultureLetters);
            }

            return null;
        }

        /// <summary>
        /// Checks to see if the first argument in the Path can be used to make a CultureInfo.
        /// </summary>
        /// <param name="context">NancyContext</param>
        /// <returns>CultureInfo if found in Path otherwise null</returns>
        public static CultureInfo PathCulture(NancyContext context)
        {
            var firstParameter =
                            context.Request.Url.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (firstParameter != null && IsValidCultureInfoName(firstParameter))
            {
                return new CultureInfo(firstParameter);
            }

            return null;
        }

        /// <summary>
        /// Checks to see if the AcceptLanguage in the Headers can be used to make a CultureInfo. Uses highest weighted if multiple defined.
        /// </summary>
        /// <param name="context">NancyContext</param>
        /// <returns>CultureInfo if found in Headers otherwise null</returns>
        public static CultureInfo HeaderCulture(NancyContext context)
        {
            if (context.Request.Headers.AcceptLanguage.Any())
            {
                var cultureLetters = context.Request.Headers.AcceptLanguage.First().Item1;
                if (!IsValidCultureInfoName(cultureLetters))
                    return null;

                return new CultureInfo(cultureLetters);
            }

            return null;
        }

        /// <summary>
        /// Checks to see if the Session has a CurrentCulture key
        /// </summary>
        /// <param name="context">NancyContext</param>
        /// <returns>CultureInfo if found in Session otherwise null</returns>
        public static CultureInfo SessionCulture(NancyContext context)
        {
            if (!(context.Request.Session is NullSessionProvider) && context.Request.Session["CurrentCulture"] != null)
            {
                return (CultureInfo)context.Request.Session["CurrentCulture"];
            }

            return null;
        }

        /// <summary>
        /// Checks to see if the Cookies has a CurrentCulture key
        /// </summary>
        /// <param name="context">NancyContext</param>
        /// <returns>CultureInfo if found in Cookies otherwise null</returns>
        public static CultureInfo CookieCulture(NancyContext context)
        {
            string cookieCulture = null;

            if (context.Request.Cookies.TryGetValue("CurrentCulture", out cookieCulture))
            {
                if (!IsValidCultureInfoName(cookieCulture))
                    return null;

                return new CultureInfo(cookieCulture);
            }

            return null;
        }

        /// <summary>
        /// Uses the Thread.CurrentThread.CurrentCulture
        /// </summary>
        /// <param name="context">NancyContext</param>
        /// <returns>CultureInfo from CurrentThread</returns>
        public static CultureInfo ThreadCulture(NancyContext context)
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        /// <summary>
        /// Validates culture name
        /// </summary>
        /// <param name="name">Culture name eg\en-GB</param>
        /// <returns>True/False if valid culture</returns>
        public static bool IsValidCultureInfoName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

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
