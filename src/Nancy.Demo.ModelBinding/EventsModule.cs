namespace Nancy.Demo.ModelBinding
{
    using System;
    using System.Linq;
    using Database;
    using Models;
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

                    return Response.AsRedirect("/Events");
                };

            Get["/bindto"] = x =>
                {
                    var model = DB.Events.OrderBy(e => e.Time).ToArray();

                    return View["Events", model];
                };

            Post["/bindto"] = x =>
                {
                    Event model = new Event
                                    {
                                        Id = 1,
                                        Location = "NEC",
                                        Title = "NEC event",
                                        Time = DateTime.Now
                                    };

                    this.BindTo(model);

                    DB.Events.Add(model);

                    return Response.AsRedirect("/events/bindto");
                };
        }
    }
}