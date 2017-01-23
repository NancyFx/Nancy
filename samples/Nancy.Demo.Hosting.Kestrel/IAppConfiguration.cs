namespace Nancy.Demo.Hosting.Kestrel
{
    public interface IAppConfiguration
    {
        Logging Logging { get; }
        Smtp Smtp { get; }
    }
}