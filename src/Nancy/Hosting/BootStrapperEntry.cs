namespace Nancy.Hosting
{
    public sealed class BootStrapperEntry
    {
        public BootStrapperEntry(string assembly, string name)
        {
            Assembly = assembly;
            Name = name;
        }

        public string Assembly { get; private set; }

        public string Name { get; private set; }
    }
}
