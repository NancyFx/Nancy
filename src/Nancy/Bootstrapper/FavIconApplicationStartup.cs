namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Application startup task that attempts to locate a favicon. The startup will first scan all
    /// folders in the path defined by the provided <see cref="IRootPathProvider"/> and if it cannot
    /// find one, it will fall back and use the default favicon that is embedded in the Nancy.dll file.
    /// </summary>
    public class FavIconApplicationStartup : IApplicationStartup
    {
        private static IRootPathProvider rootPathProvider;
        private static byte[] favIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavIconApplicationStartup"/> class, with the
        /// provided <see cref="IRootPathProvider"/> instance.
        /// </summary>
        /// <param name="rootPathProvider">The <see cref="IRootPathProvider"/> that should be used to scan for a favicon.</param>
        public FavIconApplicationStartup(IRootPathProvider rootPathProvider)
        {
            FavIconApplicationStartup.rootPathProvider = rootPathProvider;
        }

        /// <summary>
        /// Gets the default favicon
        /// </summary>
        /// <value>A byte array, containing a favicon.ico file.</value>
        public static byte[] FavIcon
        {
            get { return favIcon ?? (favIcon = ScanForFavIcon()); }
        }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
        }

        private static byte[] ExtractDefaultIcon()
        {
            var resourceStream =
                typeof(INancyEngine).Assembly.GetManifestResourceStream("Nancy.favicon.ico");

            if (resourceStream == null)
            {
                return null;
            }

            var result =
                new byte[resourceStream.Length];

            resourceStream.Read(result, 0, (int)resourceStream.Length);

            return result;
        }

        private static byte[] LocateIconOnFileSystem()
        {
            if (rootPathProvider == null)
            {
                return null;
            }

            var extensions = new[] { "ico", "png" };

            var locatedFavIcon = extensions.SelectMany(EnumerateFiles).FirstOrDefault();
            if (locatedFavIcon == null)
            {
                return null;
            }

            try
            {
                return File.ReadAllBytes(locatedFavIcon);
            }
            catch (Exception e)
            {
                if (!StaticConfiguration.DisableErrorTraces)
                {
                    throw new InvalidDataException("Unable to load favicon", e);
                }

                return null;
            }
        }

        private static IEnumerable<string> EnumerateFiles(string extension)
        {
            var rootPath = rootPathProvider.GetRootPath();
            var fileName = string.Concat("favicon.", extension);

            return Directory.EnumerateFiles(rootPath, fileName, SearchOption.AllDirectories);
        }

        private static byte[] ScanForFavIcon()
        {
            byte[] locatedIcon = null;

            try
            {
                locatedIcon = LocateIconOnFileSystem();
            }
            catch (Exception)
            {
            }

            return locatedIcon ?? ExtractDefaultIcon();
        }
    }
}
