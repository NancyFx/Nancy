namespace Nancy.ViewEngines.NHaml
{
    using global::NHaml;

    public class Template<T> : Template
    {
        public T Model { get; set; }
    }
}