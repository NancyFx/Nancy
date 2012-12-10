namespace Nancy.Culture
{
    using System;
    using System.Threading;
    using System.Globalization;
    using System.Linq;
    using Nancy.Session;


    public class DefaultCultureService : ICultureService
    {
        public CultureInfo CurrentCulture { get; set; }

        public CultureInfo DetermineCurrentCulture(NancyContext context)
        {
            //TODO: Possibly check IUserMapper for stored culture in a profile?

            string cookieCulture = null;

            var firstParameter = context.Request.Url.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (context.Request.Form["CurrentCulture"] != null)
            {
                CurrentCulture = (CultureInfo)context.Request.Form["CurrentCulture"];
            }
            else if (firstParameter != null)
            {
                CurrentCulture = new CultureInfo(firstParameter);
            }
            else if (!(context.Request.Session is NullSessionProvider) && context.Request.Session["CurrentCulutre"] != null)
            {
                CurrentCulture = (CultureInfo)context.Request.Session["CurrentCulture"];
            }
            else if (context.Request.Cookies.TryGetValue("CurrentCulture", out cookieCulture))
            {
                CurrentCulture = new CultureInfo(cookieCulture);
            }
            else
            {
                CurrentCulture = Thread.CurrentThread.CurrentCulture;
                CurrentCulture = new CultureInfo("de-DE");
            }

            return CurrentCulture;
        }
    }
}
