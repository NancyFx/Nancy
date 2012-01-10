namespace Nancy.Routing
{
    public class OptionsRoute : Route
    {
        public OptionsRoute(string path) : base("OPTIONS", path, null, x => null)
        {            
        }
    }
}