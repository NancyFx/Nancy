namespace Nancy.Testing
{
    using System;
    using System.IO;
    using System.Linq;

    public static class PathHelper
    {
        /// <summary>
        /// Traverses up a directory tree
        /// </summary>
        /// <param name="path">Start path.</param>
        /// <param name="levels">Levels to climb.</param>
        /// <returns>A <see cref="string"/> containing the new path.</returns>
        public static string GetParent(string path, int levels)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("path cannot be null or empty", "path");
            }

            if (levels < 0)
            {
                throw new ArgumentException("levels cannot be negative", "levels");
            }

            if (levels == 0)
            {
                return path;
            }

            var parts = path.Split(Path.DirectorySeparatorChar);

            if (parts.Length <= levels)
            {
                throw new InvalidOperationException("Cannot go up beyond the root.");
            }

            return
                parts.Take(parts.Length - levels).Aggregate(
                    (p1, p2) => String.Format("{0}{1}{2}", p1, Path.DirectorySeparatorChar, p2));
        }
    }
}