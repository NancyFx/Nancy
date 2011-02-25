namespace Nancy.Tests.Fakes
{
    using Nancy.Routing;

    public class FakeRoute : Route
    {
        public bool ActionWasInvoked;

<<<<<<< HEAD
        public FakeRoute() 
            : base("GET", "/", null, x => new Response())
=======
        public FakeRoute()
            : this(new Response())
        {

        }

        public FakeRoute(Response response)
            : base("GET", "/", null, x => response)
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
        {
            this.Action = x => {
                this.ActionWasInvoked = true;
                return response;
            };
        }
    }
}