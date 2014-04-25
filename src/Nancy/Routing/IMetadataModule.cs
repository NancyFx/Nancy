namespace Nancy.Routing
{
    using System;

    public interface IMetadataModule
    {
        Type MetadataType { get; }

        object ApplyMetadata(RouteDescription description);
    }
}