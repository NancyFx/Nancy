namespace Nancy.Bootstrapper
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Application startup task that attempts to locate a favicon. The startup will first scan all
    /// folders in the path defined by the provided <see cref="IRootPathProvider"/> and if it cannot
    /// fine one, it will fall back and use the default favicon that is embedded in the Nancy.dll file.
    /// </summary>
    public class FavIconStartup : IStartup
    {
        private readonly IRootPathProvider rootPathProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavIconStartup"/> class, with the
        /// provided <see cref="IRootPathProvider"/> instance.
        /// </summary>
        /// <param name="rootPathProvider">The <see cref="IRootPathProvider"/> that should be used to scan for a favicon.</param>
        public FavIconStartup(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
            FavIcon = this.ScanForFavIcon();
        }

        /// <summary>
        /// Gets the type registrations to register for this startup task
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations { get; private set; }

        /// <summary>
        /// Gets the collection registrations to register for this startup task
        /// </summary>
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; private set; }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; private set; }

        /// <summary>
        /// Gets the default favicon
        /// </summary>
        /// <value>A byte array, containing a favicon.ico file.</value>
        public static byte[] FavIcon { get; private set; }

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

        private byte[] ScanForFavIcon()
        {
            byte[] icon = null;
            var extensions = new[] {"ico", "png"};

            var locatedFavIcons = extensions.SelectMany(extension => Directory
                .EnumerateFiles(this.rootPathProvider.GetRootPath(), string.Concat("favicon.", extension), SearchOption.AllDirectories))
                .ToArray();

            if (locatedFavIcons.Any())
            {
                var image = 
                    Image.FromFile(locatedFavIcons.First());

                var converter = new ImageConverter();

                icon = (byte[])converter.ConvertTo(image, typeof(byte[]));
            }

            return icon ?? ExtractDefaultIcon();
        }
    }
}