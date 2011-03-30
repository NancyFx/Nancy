namespace Nancy.Testing.Fakes
{
    /// <summary>
    /// Fake root path provider - set the static <see cref="RootPath"/> property
    /// </summary>
    public class FakeRootPathProvider : IRootPathProvider
    {
        public static string RootPath { get; set; }

        /// <summary>
        /// Returns the root folder path of the current Nancy application.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the path of the root folder.</returns>
        string IRootPathProvider.GetRootPath()
        {
            return RootPath;
        }
    }
}