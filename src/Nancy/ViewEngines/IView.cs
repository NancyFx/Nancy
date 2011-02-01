namespace Nancy.ViewEngines
{
    using System.IO;

    public interface IView
    {
        object Model { get; set; }

        TextWriter Writer { get; set; }

        void Execute();
    }
}
