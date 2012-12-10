
namespace Nancy.Culture
{
    using System.Globalization;
    using Nancy.Conventions;
    using System.Linq;

    public class DefaultCultureService : ICultureService
    {
        private readonly CultureConventions cultureConventions;

        public DefaultCultureService(CultureConventions cultureConventions)
        {
            this.cultureConventions = cultureConventions;
        }

        public CultureInfo DetermineCurrentCulture(NancyContext context)
        {
            CultureInfo culture = null;

            foreach (var func in this.cultureConventions)
            {
                culture = func(context);
                if (culture != null)
                {
                    break;
                }
            }

            return culture;
        }
    }
}
