namespace Nancy.Demo.ModelBinding
{
    using System.Linq;

    using Nancy.Demo.ModelBinding.Database;
    using Nancy.Demo.ModelBinding.Models;
    using Nancy.ModelBinding;

    public class EventsModule : NancyModule
    {
        public EventsModule()
            : base("/events")
        {
            Get["/"] = x =>
                {
                    var model = DB.Events.OrderBy(e => e.Time).ToArray();

                    return View["Events", model];
                };

            Post["/"] = x =>
                {
                    Event model = this.Bind();
                    var model2 = this.Bind<Event>("Location"); // Blacklist location

                    DB.Events.Add(model);
                    DB.Events.Add(model2);

                    return this.Response.AsRedirect("/Events");
                };
        }
    }
}