namespace Nancy.Demo.Hosting.Self.Models
{
    public class Index
    {
        public string Name { get; set; }

        public string Posted { get; set; }

        public Index()
        {
            this.Posted = "Nothing :-(";
        }
    }
}