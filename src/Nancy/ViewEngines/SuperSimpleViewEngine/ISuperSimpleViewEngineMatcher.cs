namespace Nancy.ViewEngines.SuperSimpleViewEngine
{
    /// <summary>
    /// Matches and modifies the content of a rendered SuperSimpleViewEngine view.
    /// </summary>
    public interface ISuperSimpleViewEngineMatcher
    {
        /// <summary>
        /// Invokes the matcher on the content of the rendered view.
        /// </summary>
        /// <param name="content">The content of the rendered view.</param>
        /// <param name="model">The model that was passed to the view.</param>
        /// <param name="host">The <see cref="IViewEngineHost"/> host.</param>
        /// <returns>The modified version of the view.</returns>
        string Invoke(string content, dynamic model, IViewEngineHost host);
    }
}