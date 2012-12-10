namespace Nancy.Culture
{
    using System.Globalization;

    public interface ICultureService
    {
        CultureInfo DetermineCurrentCulture(NancyContext context);
    }
}
