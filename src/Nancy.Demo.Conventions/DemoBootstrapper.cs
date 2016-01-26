namespace Nancy.Demo.Conventions
{
    using Nancy.Conventions;


    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(
           StaticContentConventionBuilder.AddDirectory("assets")
       );

        }
    }
}