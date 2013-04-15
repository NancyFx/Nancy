namespace Nancy.Hosting.Self
{
    internal class NetShCommand
    {
        public string File { get; private set; }
        public string Args { get; private set; }

        public NetShCommand(string file, string args)
        {
            this.File = file;
            this.Args = args;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", File, Args);
        }
    }
}