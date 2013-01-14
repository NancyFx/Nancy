namespace Nancy.Demo.CustomModule
{
    public class MainModule : UglifiedNancyModule
    {
        [NancyRoute("GET", "/")]
        public dynamic Root(dynamic parameters)
        {
            return View["Index", new { Name = "Jimbo!" }];
        }

        public bool FilteredFilter(NancyContext context)
        {
            return false;
        }

        [NancyRoute("GET", "/filtered")]
        public dynamic Filtered(dynamic parameters)
        {
            return "This is filtered";
        }
    }
}