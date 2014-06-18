namespace Nancy.Demo.Hosting.Aspnet.Metadata
{
    using Nancy.Metadata.Modules;

    public class MainMetadataModule : MetadataModule<MyUberRouteMetadata>
    {
        public MainMetadataModule()
        {
            Describe["NamedRoute"] = desc =>
                {
                    return new MyUberRouteMetadata(desc.Method, desc.Path)
                        {
                            SuperDescription = "Returns the string \"I am a named route!\"",
                            CodeSample = "Get[\"NamedRoute\", \"/namedRoute\"] = _ => \"I am a named route!\";"
                        };
                };
        }
    }
}