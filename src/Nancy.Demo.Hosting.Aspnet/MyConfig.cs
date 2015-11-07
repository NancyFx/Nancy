namespace Nancy.Demo.Hosting.Aspnet
{
    /// <summary>
    /// Sample custom configuration type. It is good practise (but not required)
    /// to make your config objects immutable.
    /// </summary>
    public class MyConfig
    { 
        public MyConfig(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}