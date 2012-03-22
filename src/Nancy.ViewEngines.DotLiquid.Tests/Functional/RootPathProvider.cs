namespace Nancy.ViewEngines.DotLiquid.Tests.Functional
{
    using Testing;

    public class RootPathProvider : IRootPathProvider
    {
        /// <summary>
        /// Returns the root folder path of the current Nancy application.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the path of the root folder.</returns>
        public string GetRootPath()
        {
            return PathHelper.GetParent(typeof (RootPathProvider).Assembly.Location, 3);
        }
    }
}