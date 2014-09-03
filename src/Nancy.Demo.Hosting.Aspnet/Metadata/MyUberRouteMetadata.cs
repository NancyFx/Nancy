namespace Nancy.Demo.Hosting.Aspnet.Metadata
{
    public class MyUberRouteMetadata : MyRouteMetadata
    {
        public MyUberRouteMetadata(string method, string path)
            : base(method, path)
        {
        }

        public string SuperDescription { get; set; }
    }
}