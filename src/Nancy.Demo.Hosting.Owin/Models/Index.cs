namespace Nancy.Demo.Hosting.Owin.Models
{
    public class Index
    {
        public Index()
        {
            this.Posted = "Nothing :-(";
        }

        public string Name { get; set; }

        public string Posted { get; set; }
    }
}