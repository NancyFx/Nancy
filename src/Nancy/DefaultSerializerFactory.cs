namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Default implementation of the <see cref="ISerializerFactory"/> interface.
    /// </summary>
    /// <remarks>This implementation will ignore the default implementations (those found in the Nancy assembly) unless no other match could be made.</remarks>
    public class DefaultSerializerFactory : ISerializerFactory
    {
        private readonly IEnumerable<ISerializer> serializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSerializerFactory"/> class,
        /// with the provided <paramref name="serializers"/>.
        /// </summary>
        /// <param name="serializers">The <see cref="ISerializer"/> implementations that should be available to the factory.</param>
        public DefaultSerializerFactory(IEnumerable<ISerializer> serializers)
        {
            this.serializers = serializers;
        }

        /// <summary>
        /// Gets the <see cref="ISerializer"/> implementation that can serialize the provided <paramref name="mediaRange"/>.
        /// </summary>
        /// <param name="mediaRange">The <see cref="MediaRange"/> to get a serializer for.</param>
        /// <returns>An <see cref="ISerializer"/> instance, or <see langword="null" /> if not match was found.</returns>
        /// <exception cref="InvalidOperationException">If more than one <see cref="ISerializer"/> (not counting the default serializers) matched the provided media range.</exception>
        public ISerializer GetSerializer(MediaRange mediaRange)
        {
            var defaultSerializerForMediaRange =
                this.GetDefaultSerializerForMediaRange(mediaRange);

            var matches = this.serializers
                .Where(x => x != defaultSerializerForMediaRange)
                .Where(x => SafeCanSerialize(x, mediaRange)).ToArray();

            if (matches.Length > 1)
            {
                throw new InvalidOperationException(GetErrorMessage(matches, mediaRange));
            }

            return matches
                .Concat(new[] { defaultSerializerForMediaRange })
                .FirstOrDefault();
        }

        private ISerializer GetDefaultSerializerForMediaRange(MediaRange mediaRange)
        {
            try
            {
                return this.serializers
                    .Where(x => x.GetType().Assembly.Equals(typeof(INancyEngine).Assembly))
                    .SingleOrDefault(x => x.CanSerialize(mediaRange));
            }
            catch
            {
                return null;
            }
        }

        private static string GetErrorMessage(IEnumerable<ISerializer> matches, MediaRange mediaRange)
        {
            var details =
                string.Join("\n", matches.Select(x => string.Concat(" - ", x.GetType().FullName)));

            return string.Format("Multiple ISerializer implementations matched the '{0}' media range.\nThe following serializers matched \n\n{1}", mediaRange, details);
        }

        private static bool SafeCanSerialize(ISerializer serializer, MediaRange mediaRange)
        {
            try
            {
                return serializer.CanSerialize(mediaRange);
            }
            catch
            {
                return false;
            }
        }
    }
}