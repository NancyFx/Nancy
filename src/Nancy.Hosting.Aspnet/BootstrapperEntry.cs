namespace Nancy.Hosting.Aspnet
{
    public sealed class BootstrapperEntry
    {
        public BootstrapperEntry(string assembly, string name)
        {
            Assembly = assembly;
            Name = name;
        }

        public string Assembly { get; private set; }

        public string Name { get; private set; }
    }
}
