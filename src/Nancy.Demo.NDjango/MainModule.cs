namespace Nancy.Demo.NDjango
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = parameters => {
                return "<a href='/ndjango-simple'>Simple view</a><br><a href='/ndjango-extends'>View with master page</a>";
            };

            Get["/ndjango-simple"] = x => {
                return View["ndjango", new RatPack { FirstName = "Michael" }];
            };

            Get["/ndjango-extends"] = x => {
                return View["with-master", new RatPack { FirstName = "Michael" }];
            };
        }
    }

    public class RatPack
    {
        public string FirstName { get; set; }
    }
}