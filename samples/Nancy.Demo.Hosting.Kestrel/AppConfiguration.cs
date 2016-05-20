namespace Nancy.Demo.Hosting.Kestrel
{
    public class AppConfiguration
    {
        public Logging Logging { get; set; }
        public Smtp Smtp { get; set; }
    }
    
    public class LogLevel
    {
        public string Default { get; set; }
        public string System { get; set; }
        public string Microsoft { get; set; }
    }

    public class Logging
    {
        public bool IncludeScopes { get; set; }
        public LogLevel LogLevel { get; set; }
    }

    public class Smtp
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string Port { get; set; }
    }
}