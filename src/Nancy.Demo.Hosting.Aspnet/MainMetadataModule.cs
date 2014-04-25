namespace Nancy.Demo.Hosting.Aspnet
{
    using Nancy.Routing;

    public class MainMetadataModule : MetadataModule<MySuperRouteMetadata>
    {
        public MainMetadataModule()
        {
            Describe["NamedRoute"] = desc =>
                {
                    return new MySuperRouteMetadata(desc.Method, desc.Path)
                        {
                            SuperDescription = "Returns the string \"I am a named route!\""
                        };
                };
        }
    }

    public class MySuperRouteMetadata : MyRouteMetadata
    {
        public MySuperRouteMetadata(string method, string path)
            : base(method, path)
        {
        }

        public string SuperDescription { get; set; }
    }
}