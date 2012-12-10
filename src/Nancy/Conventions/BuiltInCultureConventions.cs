namespace Nancy.Conventions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Nancy.Session;

    public static class BuiltInCultureConventions
    {
        public static CultureInfo FormCulture(NancyContext context)
        {
            if (context.Request.Form["CurrentCulture"] != null)
            {
                return (CultureInfo)context.Request.Form["CurrentCulture"];
            }

            return null;
        }

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

        public static CultureInfo SessionCulture(NancyContext context)
        {
            if (!(context.Request.Session is NullSessionProvider) && context.Request.Session["CurrentCulutre"] != null)
            {
                return (CultureInfo)context.Request.Session["CurrentCulture"];
            }

            return null;
        }

        public static CultureInfo CookieCulture(NancyContext context)
        {
            string cookieCulture = null;

            if (context.Request.Cookies.TryGetValue("CurrentCulture", out cookieCulture))
            {
                return new CultureInfo(cookieCulture);
            }

            return null;
        }

        public static CultureInfo ThreadCulture(NancyContext context)
        {
            return Thread.CurrentThread.CurrentCulture;
        }
    }
}
